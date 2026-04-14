// -*- coding: utf-8 -*-
// SwApplicationFactory.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// SolidWorks应用程序工厂类，用于创建和连接SolidWorks应用程序实例
//*********************************************************************

using SolidWorks.Interop.sldworks;
using System;
using System.IO;
using System.Linq;
using Xarial.XCad.SolidWorks.Enums;
using Xarial.XCad.Toolkit.Windows;
using System.Diagnostics;
using System.Threading;
using System.Collections.Generic;
using Microsoft.Win32;
using Xarial.XCad.Toolkit;
using Xarial.XCad.Enums;
using Xarial.XCad.Utils.Diagnostics;

namespace Xarial.XCad.SolidWorks
{
    /// <summary>
    /// Factory for creating <see cref="ISwApplication"/>
    /// <para>中文：用于创建 SolidWorks 应用程序实例的工厂类</para>
    /// </summary>
    public class SwApplicationFactory
    {
        /// <summary>
        /// SolidWorks command-line launch arguments
        /// <para>中文：SolidWorks 命令行启动参数常量定义</para>
        /// </summary>
        internal static class CommandLineArguments
        {
            /// <summary>
            /// Bypasses the Tools/Options settings
            /// <para>中文：以安全模式启动，绕过工具/选项设置</para>
            /// </summary>
            public const string SafeMode = "/SWSafeMode /SWDisableExitApp";

            /// <summary>
            /// Runs SOLIDWORKS in background model via SOLIDWORKS Task Scheduler (requires SOLIDWORKS Professional or higher)
            /// <para>中文：通过任务计划程序以后台模式运行 SolidWorks（需要 Professional 版本或更高）</para>
            /// </summary>
            public const string BackgroundMode = "/b";

            /// <summary>
            /// Suppresses all popup messages, including the splash screen
            /// <para>中文：静默模式，抑制所有弹出消息（包括启动画面）</para>
            /// </summary>
            public const string SilentMode = "/r";
        }

        internal const string PROG_ID_TEMPLATE = "SldWorks.Application.{0}";
        
        private const string ADDINS_STARTUP_REG_KEY = @"Software\SolidWorks\AddInsStartup";

        /// <summary>
        /// Disables all startup add-ins
        /// <para>中文：禁用所有在 SolidWorks 启动时自动加载的插件，并返回已禁用的插件 GUID 列表</para>
        /// </summary>
        /// <param name="disabledAddInGuids">Guids of the disabled add-ins</param>
        /// <remarks>Call the <see cref="EnableAddInsStartup(IReadOnlyList{string})"/> to restore the add-ins</remarks>
        public static void DisableAllAddInsStartup(out IReadOnlyList<string> disabledAddInGuids)
        {
            const int DISABLE_VAL = 0;
            const int ENABLE_VAL = 1;

            var localDisabledAddInGuids = new List<string>();

            // 打开注册表中的插件启动键（可写模式）
            var addinsStartup = Registry.CurrentUser.OpenSubKey(ADDINS_STARTUP_REG_KEY, true);

            if (addinsStartup != null)
            {
                var addInKeyNames = addinsStartup.GetSubKeyNames();

                if (addInKeyNames != null)
                {
                    foreach (var addInKeyName in addInKeyNames)
                    {
                        var addInKey = addinsStartup.OpenSubKey(addInKeyName, true);

                        int enableVal;

                        if (int.TryParse(addInKey.GetValue("")?.ToString(), out enableVal))
                        {
                            var loadOnStartup = enableVal == ENABLE_VAL;

                            if (loadOnStartup)
                            {
                                // 将启动值设为 0 以禁用该插件，并记录其 GUID
                                addInKey.SetValue("", DISABLE_VAL);
                                localDisabledAddInGuids.Add(addInKeyName);
                            }
                        }
                    }
                }
            }

            disabledAddInGuids = localDisabledAddInGuids;
        }

        /// <summary>
        /// Enables the add-ins at startup
        /// <para>中文：恢复指定插件在 SolidWorks 启动时自动加载</para>
        /// </summary>
        /// <param name="addInGuids">Add-in guids</param>
        public static void EnableAddInsStartup(IReadOnlyList<string> addInGuids)
        {
            const int ENABLE_VAL = 1;

            var addinsStartup = Registry.CurrentUser.OpenSubKey(ADDINS_STARTUP_REG_KEY, true);

            foreach (var addInKeyName in addInGuids)
            {
                var addInKey = addinsStartup.OpenSubKey(addInKeyName, true);

                // 将启动值恢复为 1，使该插件重新在启动时加载
                addInKey.SetValue("", ENABLE_VAL);
            }
        }

        /// <summary>
        /// Pre-creates a template for SOLIDWORKS application
        /// <para>中文：预创建 SolidWorks 应用程序模板实例（尚未提交/启动），可在 Commit 前设置版本和状态</para>
        /// </summary>
        /// <returns></returns>
        public static ISwApplication PreCreate() => new SwApplication();

        /// <summary>
        /// Returns all installed SOLIDWORKS versions
        /// <para>中文：枚举并返回当前系统上已安装的所有 SolidWorks 版本</para>
        /// </summary>
        /// <returns>Enumerates versions</returns>
        public static IEnumerable<ISwVersion> GetInstalledVersions()
        {
            foreach (var versCand in Enum.GetValues(typeof(SwVersion_e)).Cast<SwVersion_e>())
            {
                // 通过注册表 ProgID 检测对应版本是否已安装
                var progId = string.Format(PROG_ID_TEMPLATE, (int)versCand);
                var swAppRegKey = Registry.ClassesRoot.OpenSubKey(progId);

                if (swAppRegKey != null)
                {
                    var isInstalled = false;

                    try
                    {
                        FindSwPathFromRegKey(swAppRegKey);
                        isInstalled = true;
                    }
                    catch
                    {
                    }

                    if (isInstalled)
                    {
                        yield return CreateVersion(versCand);
                    }
                }
            }
        }

        /// <summary>
        /// Creates <see cref="ISwApplication"/> from SOLIDWORKS pointer
        /// <para>中文：从现有 SolidWorks COM 指针创建应用程序包装实例</para>
        /// </summary>
        /// <param name="app">Pointer to SOLIDWORKS application</param>
        /// <returns>Instance of <see cref="ISwApplication"/></returns>
        public static ISwApplication FromPointer(ISldWorks app)
            => FromPointer(app, new ServiceCollection());

        /// <inheritdoc cref="FromPointer(ISldWorks)"/>
        /// <param name="services">Custom serives</param>
        public static ISwApplication FromPointer(ISldWorks app, IXServiceCollection services)
            => new SwApplication(app, services);

        /// <summary>
        /// Creates instance of SOLIDWORKS from SLDWORKS.exe process
        /// <para>中文：通过正在运行的 SLDWORKS.exe 进程获取 SolidWorks COM 对象并创建应用程序包装实例</para>
        /// </summary>
        /// <param name="process">SLDWORKS.exe process</param>
        /// <returns>Pointer to <see cref="ISwApplication"/></returns>
        public static ISwApplication FromProcess(Process process)
            => FromProcess(process, new ServiceCollection());

        /// <inheritdoc cref="FromProcess(Process)"/>
        /// <param name="services">Custom serives</param>
        public static ISwApplication FromProcess(Process process, IXServiceCollection services)
        {
            // 通过 ROT（运行对象表）按 Moniker 名称获取 SolidWorks COM 对象
            var app = RotHelper.TryGetComObjectByMonikerName<ISldWorks>(GetMonikerName(process), new TraceLogger("xCAD.SwApplication"));

            if (app != null)
            {
                return FromPointer(app, services);
            }
            else
            {
                throw new Exception($"Cannot access SOLIDWORKS application at process {process.Id}");
            }
        }

        /// <summary>
        /// Starts new application
        /// <para>中文：启动一个新的 SolidWorks 应用程序实例</para>
        /// </summary>
        /// <param name="vers">Version or null for the latest</param>
        /// <param name="state">State of the application</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Created application</returns>
        public static ISwApplication Create(SwVersion_e? vers = null,
            ApplicationState_e state = ApplicationState_e.Default,
            CancellationToken cancellationToken = default)
        {
            var app = PreCreate();

            // 设置目标版本和启动状态后提交（真正启动应用程序）
            app.Version = vers.HasValue ? CreateVersion(vers.Value) : null;
            app.State = state;

            app.Commit(cancellationToken);

            return app;
        }

        /// <summary>
        /// Creates instance of SOLIDWORKS version from the major version
        /// <para>中文：根据主版本号枚举值创建对应的 SolidWorks 版本对象</para>
        /// </summary>
        /// <param name="vers"></param>
        /// <returns></returns>
        public static ISwVersion CreateVersion(SwVersion_e vers) => new SwVersion(new Version((int)vers, 0), 0, 0);

        // 根据进程 ID 构造 SolidWorks ROT Moniker 名称
        internal static string GetMonikerName(Process process) => $"SolidWorks_PID_{process.Id}";

        /// <summary>
        /// Reads the SolidWorks executable path from the registry key for the given ProgID.
        /// <para>中文：从指定 ProgID 对应的注册表键中读取 SolidWorks 可执行文件路径。</para>
        /// </summary>
        internal static string FindSwPathFromRegKey(RegistryKey swAppRegKey)
        {
            var clsidKey = swAppRegKey.OpenSubKey("CLSID", false);

            if (clsidKey == null)
            {
                throw new NullReferenceException($"Incorrect registry value, CLSID is missing");
            }

            var clsid = (string)clsidKey.GetValue("");

            if (clsid == null)
            {
                throw new NullReferenceException($"Incorrect registry value, LocalServer32 is missing");
            }

            // 通过 CLSID 在注册表中找到 LocalServer32 键获取可执行文件路径
            var localServerKey = Registry.ClassesRoot.OpenSubKey(
                $"CLSID\\{clsid}\\LocalServer32", false);

            if (localServerKey == null) 
            {
                throw new Exception("Failed to find the class id in the registry. Make sure that application is running as x64 bit process (including 'Prefer 32-bit' option is unchecked in the project settings)");
            }

            var swAppPath = (string)localServerKey.GetValue("");

            if (!File.Exists(swAppPath))
            {
                throw new FileNotFoundException($"Path to SOLIDWORKS executable does not exist: {swAppPath}");
            }

            return swAppPath;
        }
    }
}