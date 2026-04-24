// -*- coding: utf-8 -*-
// tests/integration/SolidWorks.Tests.Integration/IntegrationTests.cs

using NUnit.Framework;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Xarial.XCad.Enums;
using Xarial.XCad.SolidWorks;
using Xarial.XCad.SolidWorks.Enums;
using Environment = System.Environment;

namespace SolidWorks.Tests.Integration
{
    /// <summary>
    /// IntegrationTests 是所有 SOLIDWORKS 集成测试的抽象基类。
    /// 提供启动/关闭 SOLIDWORKS 实例、打开测试文档、创建新文档等辅助方法。
    /// 测试需要在 STA（单线程单元）模式下运行，以兼容 COM 对象。
    /// </summary>
    [TestFixture]
    [RequiresThread(System.Threading.ApartmentState.STA)]
    public abstract class IntegrationTests
    {
        /// <summary>
        /// DocumentWrapper 封装已打开的 SOLIDWORKS 文档，确保在 using 块结束时正确关闭文档。
        /// 这是一个内部辅助类，管理文档的生命周期。
        /// </summary>
        private class DocumentWrapper : IDisposable
        {
            private readonly ISldWorks m_App;
            private readonly IModelDoc2 m_Model;

            private bool m_IsDisposed;

            internal DocumentWrapper(ISldWorks app, IModelDoc2 model)
            {
                m_App = app;
                m_Model = model;
                m_IsDisposed = false;
            }

            /// <summary>
            /// 关闭文档，仅在未 dispose 的情况下执行关闭操作。
            /// </summary>
            public void Dispose()
            {
                if (!m_IsDisposed)
                {
                    m_App.CloseDoc(m_Model.GetTitle());
                    m_IsDisposed = true;
                }
            }
        }

        private const int SW_PRC_ID = -1;

        // 测试数据文件夹路径，从环境变量 XCAD_TEST_DATA 读取
        private readonly string m_DataFolder;
        private SwVersion_e? SW_VERSION = SwVersion_e.Sw2022;

        // SOLIDWORKS 应用程序实例
        protected ISwApplication m_App;
        private Process m_Process;
        private ISldWorks m_SwApp;

        // 已打开文档的追踪列表，用于测试结束后清理
        private List<IDisposable> m_Disposables;

        // 是否需要在测试结束后关闭 SOLIDWORKS 实例
        private bool m_CloseSw;

        /// <summary>
        /// 构造函数：从环境变量读取测试数据文件夹路径。
        /// 支持 User 和 Machine 两个级别的环境变量。
        /// </summary>
        public IntegrationTests()
        {
            m_DataFolder = Environment.GetEnvironmentVariable("XCAD_TEST_DATA", EnvironmentVariableTarget.User);

            if (string.IsNullOrEmpty(m_DataFolder))
            {
                m_DataFolder = Environment.GetEnvironmentVariable("XCAD_TEST_DATA", EnvironmentVariableTarget.Machine);
            }
        }

        /// <summary>
        /// OneTimeSetUp：在所有测试运行前执行一次。
        /// 创建 SOLIDWORKS 后台实例（SW_PRC_ID < 0 表示创建新实例）。
        /// </summary>
        [OneTimeSetUp]
        public void Setup()
        {
            // SW_PRC_ID < 0 表示创建新的后台 SOLIDWORKS 实例
            if (SW_PRC_ID < 0)
            {
                IReadOnlyList<string> m_DisabledStartupAddIns;

                // 禁用所有启动加载项以避免干扰测试
                SwApplicationFactory.DisableAllAddInsStartup(out m_DisabledStartupAddIns);

                // 创建后台、安全、静默模式的 SOLIDWORKS 实例
                m_App = SwApplicationFactory.Create(SW_VERSION,
                    ApplicationState_e.Background
                    | ApplicationState_e.Safe
                    | ApplicationState_e.Silent);

                // 恢复之前禁用的启动加载项
                if (m_DisabledStartupAddIns?.Any() == true)
                {
                    SwApplicationFactory.EnableAddInsStartup(m_DisabledStartupAddIns);
                }

                m_CloseSw = true;
            }
            // SW_PRC_ID == 0 表示附加到已运行的 SOLIDWORKS 进程
            else if (SW_PRC_ID == 0)
            {
                var prc = Process.GetProcessesByName("SLDWORKS").First();
                m_App = SwApplicationFactory.FromProcess(prc);
            }
            // SW_PRC_ID > 0 表示附加到指定进程 ID 的 SOLIDWORKS 实例
            else
            {
                var prc = Process.GetProcessById(SW_PRC_ID);
                m_App = SwApplicationFactory.FromProcess(prc);
            }

            m_SwApp = m_App.Sw;
            m_Process = m_App.Process;
            m_Disposables = new List<IDisposable>();
        }

        /// <summary>
        /// 将文件名转换为完整的测试数据文件路径。
        /// 如果是绝对路径则直接返回，否则与测试数据文件夹合并。
        /// </summary>
        /// <param name="name">文件名或完整路径</param>
        /// <returns>完整的文件路径</returns>
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
        /// 打开测试数据文档的辅助方法。
        /// 支持只读模式、轻量级加载模式，以及通过 specEditor 回调自定义文档规格。
        /// 打开后自动解析所有轻量级组件（ResolveAllLightWeightComponents）。
        /// </summary>
        /// <param name="name">文档文件名或路径</param>
        /// <param name="readOnly">是否只读打开</param>
        /// <param name="specEditor">可选的文档规格编辑器回调</param>
        /// <returns>文档包装器对象，使用完后自动关闭文档</returns>
        protected IDisposable OpenDataDocument(string name, bool readOnly = true, Action<IDocumentSpecification> specEditor = null)
        {
            var filePath = GetFilePath(name);

            // 创建文档打开规格
            var spec = (IDocumentSpecification)m_SwApp.GetOpenDocSpec(filePath);
            spec.ReadOnly = readOnly;
            spec.LightWeight = false;
            spec.UseLightWeightDefault = false;
            specEditor?.Invoke(spec);

            // 打开文档
            var model = m_SwApp.OpenDoc7(spec);

            if (model != null)
            {
                // 如果不是轻量级加载，且是装配体，则解析所有轻量级组件
                if (!spec.LightWeight)
                {
                    if (model is IAssemblyDoc)
                    {
                        (model as IAssemblyDoc).ResolveAllLightWeightComponents(false);
                    }
                }

                // 创建文档包装器并追踪
                var docWrapper = new DocumentWrapper(m_SwApp, model);
                m_Disposables.Add(docWrapper);
                return docWrapper;
            }
            else
            {
                throw new NullReferenceException($"Failed to open the the data document at '{filePath}'");
            }
        }

        /// <summary>
        /// 创建新 SOLIDWORKS 文档的辅助方法。
        /// 临时设置"始终使用默认模板"选项以获取默认模板路径。
        /// </summary>
        /// <param name="docType">文档类型（零件、装配体、工程图等）</param>
        /// <returns>文档包装器对象，使用完后自动关闭文档</returns>
        protected IDisposable NewDocument(swDocumentTypes_e docType)
        {
            // 保存当前用户偏好设置
            var useDefTemplates = m_SwApp.GetUserPreferenceToggle((int)swUserPreferenceToggle_e.swAlwaysUseDefaultTemplates);

            try
            {
                // 临时启用默认模板选项
                m_SwApp.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swAlwaysUseDefaultTemplates, true);

                // 获取默认模板路径
                var defTemplatePath = m_SwApp.GetDocumentTemplate(
                    (int)docType, "", (int)swDwgPaperSizes_e.swDwgPapersUserDefined, 0.1, 0.1);

                if (string.IsNullOrEmpty(defTemplatePath))
                {
                    throw new Exception("Default template is not found");
                }

                // 创建新文档
                var model = (IModelDoc2)m_SwApp.NewDocument(defTemplatePath, (int)swDwgPaperSizes_e.swDwgPapersUserDefined, 0.1, 0.1);

                if (model != null)
                {
                    var docWrapper = new DocumentWrapper(m_SwApp, model);
                    m_Disposables.Add(docWrapper);
                    return docWrapper;
                }
                else
                {
                    throw new NullReferenceException($"Failed to create new document from '{defTemplatePath}'");
                }
            }
            finally
            {
                // 恢复用户偏好设置
                m_SwApp.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swAlwaysUseDefaultTemplates, useDefTemplates);
            }
        }

        /// <summary>
        /// 比较两个双精度浮点数是否在指定精度下相等。
        /// 使用四舍五入和百分比容差进行比较。
        /// </summary>
        protected void AssertCompareDoubles(double actual, double expected, int digits = 8)
            => Assert.That(Math.Round(actual, digits), Is.EqualTo(Math.Round(expected, digits)).Within(0.000001).Percent);

        /// <summary>
        /// 比较两个双精度浮点数数组是否在指定精度下相等。
        /// </summary>
        protected void AssertCompareDoubleArray(double[] actual, double[] expected, int digits = 8, double percent = 0.000001)
        {
            if (actual.Length == expected.Length)
            {
                for (int i = 0; i < actual.Length; i++)
                {
                    Assert.That(Math.Round(actual[i], digits), Is.EqualTo(Math.Round(expected[i], digits)).Within(percent).Percent);
                }
            }
            else
            {
                Assert.Fail("Arrays size mismatch");
            }
        }

        /// <summary>
        /// TearDown：每个测试方法执行完毕后调用。
        /// 清理所有测试中打开的文档。
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            Debug.Print("Unit Tests: Disposing test disposables");

            // 逐一 dispose 所有追踪的文档，忽略可能的异常
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
        }

        /// <summary>
        /// OneTimeTearDown：所有测试执行完毕后调用一次。
        /// 关闭 SOLIDWORKS 实例（如果是由测试启动的）。
        /// </summary>
        [OneTimeTearDown]
        public void FinalTearDown()
        {
            Debug.Print($"Unit Tests: Closing SOLIDWORKS instance: {m_CloseSw}");

            if (m_CloseSw)
            {
                m_Process.Kill();
            }
        }
    }
}
