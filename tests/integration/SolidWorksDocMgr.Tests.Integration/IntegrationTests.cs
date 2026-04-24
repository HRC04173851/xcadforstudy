// -*- coding: utf-8 -*-
// tests/integration/SolidWorksDocMgr.Tests.Integration/IntegrationTests.cs
using NUnit.Framework;
using SolidWorks.Interop.swdocumentmgr;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xarial.XCad.Documents.Extensions;
using Xarial.XCad.SolidWorks;
using Xarial.XCad.SolidWorks.Documents;
using Xarial.XCad.SolidWorks.Enums;
using Xarial.XCad.SwDocumentManager;
using Xarial.XCad.SwDocumentManager.Documents;

namespace SolidWorksDocMgr.Tests.Integration
{
    /// <summary>
    /// 文档包装器，用于管理 SOLIDWORKS Document Manager 文档的生命周期。
    /// 实现 IDisposable 接口以确保文档在使用后正确关闭。
    /// </summary>
    public class DocumentWrapper : IDisposable
    {
        private readonly ISwDmApplication m_App;
        public ISwDmDocument Document { get; }

        private bool m_IsDisposed;

        internal DocumentWrapper(ISwDmApplication app, ISwDmDocument model)
        {
            m_App = app;
            Document = model;
            m_IsDisposed = false;
        }

        /// <summary>
        /// 关闭文档并释放资源。
        /// </summary>
        public void Dispose()
        {
            if (!m_IsDisposed)
            {
                Document.Close();
                m_IsDisposed = true;
            }
        }
    }

    /// <summary>
    /// SOLIDWORKS Document Manager 集成测试的基类。
    /// 提供文档打开、环境配置和资源清理等通用功能。
    /// 所有集成测试类都应继承此类以获得一致的测试环境。
    /// </summary>
    [TestFixture]
    [RequiresThread(System.Threading.ApartmentState.STA)]
    public abstract class IntegrationTests
    {
        private readonly string m_DataFolder;
        private SwVersion_e? SW_VERSION = SwVersion_e.Sw2022;

        protected ISwDmApplication m_App;

        private List<IDisposable> m_Disposables;

        public IntegrationTests()
        {
            // 从环境变量 XCAD_TEST_DATA 获取测试数据文件夹路径
            // 优先尝试用户级环境变量，然后尝试机器级环境变量
            m_DataFolder = Environment.GetEnvironmentVariable("XCAD_TEST_DATA", EnvironmentVariableTarget.User);

            if (string.IsNullOrEmpty(m_DataFolder))
            {
                m_DataFolder = Environment.GetEnvironmentVariable("XCAD_TEST_DATA", EnvironmentVariableTarget.Machine);
            }
        }

        /// <summary>
        /// 测试初始化方法，在所有测试之前执行一次。
        /// 创建 SOLIDWORKS Document Manager 应用程序实例。
        /// </summary>
        [OneTimeSetUp]
        public void Setup()
        {
            // 从环境变量获取 SW_DM_KEY 许可证密钥
            var dmKey = Environment.GetEnvironmentVariable("SW_DM_KEY", EnvironmentVariableTarget.User);

            if (string.IsNullOrEmpty(dmKey))
            {
                dmKey = Environment.GetEnvironmentVariable("SW_DM_KEY", EnvironmentVariableTarget.Machine);
            }

            m_App = SwDmApplicationFactory.Create(dmKey);

            m_Disposables = new List<IDisposable>();
        }

        /// <summary>
        /// 获取测试文件的完整路径。如果传入的是绝对路径则直接返回，
        /// 否则将其与测试数据文件夹路径组合。
        /// </summary>
        /// <param name="name">文件名或完整路径</param>
        /// <returns>文件的完整路径</returns>
        protected string GetFilePath(string name)
        {
            string filePath;

            if (Path.IsPathRooted(name))
            {
                filePath = name;
            }
            else
            {
                filePath = Path.Combine(m_DataFolder, name);
            }

            return filePath;
        }

        /// <summary>
        /// 更新 SOLIDWORKS 程序集中的参考路径。
        /// 此方法用于设置测试环境，确保装配体文件之间的引用路径正确。
        /// </summary>
        /// <param name="destPath">目标装配体目录路径</param>
        /// <param name="assmRelPaths">需要更新的装配体相对路径数组</param>
        protected void UpdateSwReferences(string destPath, params string[] assmRelPaths)
        {
            Process prc;

            // 在后台模式下启动 SOLIDWORKS 以更新引用
            using (var app = SwApplicationFactory.Create(SW_VERSION,
                            Xarial.XCad.Enums.ApplicationState_e.Background
                            | Xarial.XCad.Enums.ApplicationState_e.Silent
                            | Xarial.XCad.Enums.ApplicationState_e.Safe))
            {
                prc = app.Process;

                foreach (var assmPath in assmRelPaths)
                {
                    // 打开每个装配体，强制重建并保存
                    // 这会更新内部的依赖路径信息
                    using (var doc = (ISwDocument)app.Documents.Open(Path.Combine(destPath, assmPath)))
                    {
                        doc.Model.ForceRebuild3(false);
                        doc.Save();
                        // GetDependencies 返回交错数组：路径、名称、路径、名称...
                        // 所以需要过滤掉名称部分（索引为奇数的元素）
                        var deps = (doc.Model.Extension.GetDependencies(false, false, false, false, false) as string[]).Where((item, index) => index % 2 != 0).ToArray();

                        // 验证所有依赖都符合预期（相对路径或目标路径内的引用）
                        if (!deps.All(d => d.Contains("^") || d.StartsWith(destPath, StringComparison.CurrentCultureIgnoreCase)))
                        {
                            throw new Exception("Failed to setup source assemblies");
                        }
                    }
                }

                app.Close();
            }

            // 终止 SOLIDWORKS 进程以释放资源
            prc.Kill();
        }

        /// <summary>
        /// 递归复制目录及其所有文件和子目录。
        /// </summary>
        /// <param name="srcPath">源目录路径</param>
        /// <param name="destPath">目标目录路径</param>
        protected void CopyDirectory(string srcPath, string destPath)
        {
            foreach (var srcFile in Directory.GetFiles(srcPath, "*.*", SearchOption.AllDirectories))
            {
                var relPath = srcFile.Substring(srcPath.Length + 1);
                var destFilePath = Path.Combine(destPath, relPath);
                var destDir = Path.GetDirectoryName(destFilePath);

                if (!Directory.Exists(destDir))
                {
                    Directory.CreateDirectory(destDir);
                }

                File.Copy(srcFile, destFilePath);
            }
        }

        /// <summary>
        /// 打开测试数据文档并返回文档包装器。
        /// 包装器会自动管理文档的生命周期，在 using 块结束时自动关闭。
        /// </summary>
        /// <param name="name">文档名称或路径</param>
        /// <param name="readOnly">是否以只读模式打开，默认为 true</param>
        /// <returns>文档包装器对象</returns>
        protected DocumentWrapper OpenDataDocument(string name, bool readOnly = true)
        {
            var filePath = GetFilePath(name);

            var doc = (ISwDmDocument)m_App.Documents.Open(filePath,
                readOnly
                ? Xarial.XCad.Documents.Enums.DocumentState_e.ReadOnly
                : Xarial.XCad.Documents.Enums.DocumentState_e.Default);

            if (doc != null)
            {
                var docWrapper = new DocumentWrapper(m_App, doc);
                m_Disposables.Add(docWrapper);
                return docWrapper;
            }
            else
            {
                throw new NullReferenceException($"Failed to open the the data document at '{filePath}'");
            }
        }

        /// <summary>
        /// 每个测试方法结束后的清理工作。
        /// 关闭所有已打开的文档包装器并清除文档集合。
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            foreach (var disp in m_Disposables)
            {
                try
                {
                    disp.Dispose();
                }
                catch
                {
                }
            }

            m_Disposables.Clear();

            // 关闭应用程序中所有剩余的文档
            while (m_App.Documents.Any())
            {
                m_App.Documents.First().Close();
            }
        }

        /// <summary>
        /// 所有测试结束后的最终清理工作。
        /// 目前为空实现，预留用于可能的全局清理操作。
        /// </summary>
        [OneTimeTearDown]
        public void FinalTearDown()
        {
        }
    }
}
