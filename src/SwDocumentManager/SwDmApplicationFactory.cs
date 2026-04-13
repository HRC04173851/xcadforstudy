//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using Microsoft.Win32;
using SolidWorks.Interop.swdocumentmgr;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using Xarial.XCad.Base;
using Xarial.XCad.SwDocumentManager.Exceptions;

namespace Xarial.XCad.SwDocumentManager
{
    /// <summary>
    /// Provides a factory to create instance of the <see cref="ISwDmApplication"/>
    /// 提供用于创建 <see cref="ISwDmApplication"/> 的工厂，屏蔽注册表、COM 和许可证连接细节。
    /// </summary>
    public static class SwDmApplicationFactory
    {
        private const string DM_CLASS_FACT_PROG_ID = "SwDocumentMgr.SwDMClassFactory";

        /// <summary>
        /// Pre-creates application
        /// 预创建应用程序包装器，此时尚未真正连接到 Document Manager。
        /// </summary>
        /// <returns>xCAD application / xCAD 应用对象</returns>
        public static ISwDmApplication PreCreate() => new SwDmApplication(null, false);

        /// <summary>
        /// Returns all installed SOLIDWORKS Document Manager versions
        /// 返回本机已安装的 SOLIDWORKS Document Manager 版本。
        /// </summary>
        /// <returns>Enumerates versions / 返回版本枚举</returns>
        /// <remarks>Latest supported file version of the application also depends on the version of the Document Manager license key
        /// 应用程序可支持的最新文件版本还受 Document Manager 许可证版本限制。</remarks>
        public static IEnumerable<ISwDmVersion> GetInstalledVersions()
        {
            var swDmAppRegKey = Registry.ClassesRoot.OpenSubKey($"{DM_CLASS_FACT_PROG_ID}\\CLSID");

            if (swDmAppRegKey != null)
            {
                var clsid = (string)swDmAppRegKey.GetValue("");

                var swDmAppClsidRegKey = Registry.ClassesRoot.OpenSubKey($"CLSID\\{clsid}\\InprocServer32");

                if (swDmAppClsidRegKey != null) 
                {
                    var dmDllPath = (string)swDmAppClsidRegKey.GetValue("");

                    if (File.Exists(dmDllPath))
                    {
                        var majorVers = FileVersionInfo.GetVersionInfo(dmDllPath).FileMajorPart;

                        //only support SW 2000 or newer
                        // 仅支持从 SOLIDWORKS 2000 开始的版本号映射。
                        if (majorVers >= 8) 
                        {
                            var dmVersList = ((SwDmVersion_e[])Enum.GetValues(typeof(SwDmVersion_e))).ToList();

                            var dmVersInd = dmVersList.IndexOf(SwDmVersion_e.Sw2000) + (majorVers - 8);

                            if (dmVersInd < dmVersList.Count)
                            {
                                var dmVers = dmVersList[dmVersInd];

                                yield return CreateVersion(dmVers);
                            }
                            else 
                            {
                                throw new NotSupportedException($"File versions {majorVers} cannot be converted to Document Manager version");
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Creates an instance of the application from the Document Manager key
        /// 根据 Document Manager 许可证密钥创建应用实例。
        /// </summary>
        /// <param name="dmKey">Document manager key / Document Manager 许可证密钥</param>
        /// <returns>xCAD application / xCAD 应用对象</returns>
        public static ISwDmApplication Create(string dmKey) 
        {
            var app = PreCreate();
            app.LicenseKey = new NetworkCredential("", dmKey).SecurePassword;
            app.Commit();

            return app;
        }

        /// <summary>
        /// Creates instance of the application from the COM pointer
        /// 使用现成的 COM 指针包装出 xCAD 应用实例。
        /// </summary>
        /// <param name="app">Pointer to the application / 应用程序 COM 指针</param>
        /// <returns>xCAD application / xCAD 应用对象</returns>
        public static ISwDmApplication FromPointer(ISwDMApplication app) => new SwDmApplication(app, true);

        /// <summary>
        /// Connects to the native Document Manager COM server using the secure license key.
        /// 使用安全字符串形式的许可证密钥连接到底层 Document Manager COM 服务器。
        /// </summary>
        internal static ISwDMApplication ConnectToDm(SecureString dmKeySecure) 
        {
            ISwDMClassFactory classFactory = null;

            var classFactoryType = Type.GetTypeFromProgID(DM_CLASS_FACT_PROG_ID);

            if (classFactoryType != null)
            {
                classFactory = Activator.CreateInstance(classFactoryType) as ISwDMClassFactory;
            }

            if (classFactory != null)
            {
                var dmKey = new NetworkCredential("", dmKeySecure).Password;

                try
                {
                    var dmApp = classFactory.GetApplication(dmKey);

                    if (dmApp != null)
                    {
                        var testVers = dmApp.GetLatestSupportedFileVersion();
                        return dmApp;
                    }
                    else 
                    {
                        throw new NullReferenceException("Application is null");
                    }
                }
                catch (Exception ex)
                {
                    throw new SwDmConnectFailedException(ex);
                }
            }
            else
            {
                throw new SwDmSdkNotInstalledException();
            }
        }

        /// <summary>
        /// Creates a version of the application
        /// 根据主版本号创建 xCAD 的版本对象。
        /// </summary>
        /// <param name="major">Major version / 主版本号</param>
        /// <returns>Version / 版本对象</returns>
        public static ISwDmVersion CreateVersion(SwDmVersion_e major) => new SwDmVersion(new Version((int)major, 0));
    }
}
