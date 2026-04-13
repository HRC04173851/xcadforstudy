//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using SolidWorks.Interop.swdocumentmgr;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading;
using Xarial.XCad.Base;
using Xarial.XCad.Base.Enums;
using Xarial.XCad.Delegates;
using Xarial.XCad.Documents;
using Xarial.XCad.Enums;
using Xarial.XCad.Geometry;
using Xarial.XCad.Services;
using Xarial.XCad.SwDocumentManager.Documents;

namespace Xarial.XCad.SwDocumentManager
{
    /// <summary>
    /// Document Manager application wrapper exposed through xCAD.
    /// 通过 xCAD 暴露的 Document Manager 应用程序包装器，用于把底层 COM API 映射为统一的 xCAD 应用入口。
    /// </summary>
    public interface ISwDmApplication : IXApplication
    {
        /// <summary>
        /// Underlying SOLIDWORKS Document Manager COM application.
        /// 底层的 SOLIDWORKS Document Manager COM 应用对象。
        /// </summary>
        ISwDMApplication SwDocMgr { get; }

        /// <summary>
        /// License key required to establish the Document Manager session.
        /// 建立 Document Manager 会话所需的许可证密钥。
        /// </summary>
        SecureString LicenseKey { get; set; }

        new ISwDmDocumentCollection Documents { get; }
        new ISwDmVersion Version { get; }
    }

    /// <summary>
    /// Implements lazy connection, lifecycle management, and document ownership for the Document Manager environment.
    /// 实现 Document Manager 环境的延迟连接、生命周期管理以及文档仓库的统一持有逻辑。
    /// </summary>
    internal class SwDmApplication : ISwDmApplication
    {
        #region Not Supported        

        public event ApplicationStartingDelegate Starting { add => throw new NotSupportedException(); remove => throw new NotSupportedException(); }
        public event ApplicationIdleDelegate Idle { add => throw new NotSupportedException(); remove => throw new NotSupportedException(); }

        public ApplicationState_e State { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
        public bool IsAlive => throw new NotSupportedException();
        public Rectangle WindowRectangle => throw new NotSupportedException();
        public IntPtr WindowHandle => throw new NotSupportedException();
        public Process Process => throw new NotSupportedException();
        public IXMemoryGeometryBuilder MemoryGeometryBuilder => throw new NotSupportedException();
        public IXProgress CreateProgress() => throw new NotSupportedException();
        public IXMacro OpenMacro(string path) => throw new NotSupportedException();
        public MessageBoxResult_e ShowMessageBox(string msg, MessageBoxIcon_e icon = MessageBoxIcon_e.Info, MessageBoxButtons_e buttons = MessageBoxButtons_e.Ok) => throw new NotSupportedException();
        public void ShowTooltip(ITooltipSpec spec) => throw new NotSupportedException();
        public IXObjectTracker CreateObjectTracker(string name) => throw new NotSupportedException();
        public IXApplicationOptions Options => throw new NotSupportedException();
        public IXMaterialsDatabaseRepository MaterialDatabases => throw new NotSupportedException();
        #endregion

        IXDocumentRepository IXApplication.Documents => Documents;

        IXVersion IXApplication.Version
        {
            get => Version;
            set => throw new Exception("This property is read-only"); 
        }

        public ISwDmVersion Version => SwDmApplicationFactory.CreateVersion((SwDmVersion_e)SwDocMgr.GetLatestSupportedFileVersion());

        public ISwDmDocumentCollection Documents { get; }

        public bool IsCommitted => m_Creator.IsCreated;

        public ISwDMApplication SwDocMgr => m_Creator.Element;

        public SecureString LicenseKey 
        {
            get 
            {
                if (!IsCommitted)
                {
                    return m_Creator.CachedProperties.Get<SecureString>();
                }
                else 
                {
                    throw new NotSupportedException("This property is only available on creation of application");
                }
            }
            set 
            {
                if (!IsCommitted)
                {
                    m_Creator.CachedProperties.Set(value);
                }
                else 
                {
                    throw new NotSupportedException("");
                }
            }
        }

        private bool m_IsDisposed;
        private bool m_IsClosed;

        private readonly IElementCreator<ISwDMApplication> m_Creator;

        internal SwDmApplication(ISwDMApplication dmApp, bool isCreated) 
        {
            m_Creator = new ElementCreator<ISwDMApplication>(CreateApplication, dmApp, isCreated);
            Documents = new SwDmDocumentCollection(this);
        }

        /// <summary>
        /// Creates the native Document Manager application when the pre-created xCAD wrapper is committed.
        /// 当预创建的 xCAD 包装器被提交时，再真正创建底层 Document Manager 应用对象。
        /// </summary>
        private ISwDMApplication CreateApplication(CancellationToken cancellationToken)
        {
            var licKey = LicenseKey;
            LicenseKey = null;
            return SwDmApplicationFactory.ConnectToDm(licKey);
        }

        /// <summary>
        /// Closes all opened document wrappers before releasing the application COM object.
        /// 在释放应用程序 COM 对象之前，先关闭当前包装器管理的全部已打开文档。
        /// </summary>
        public void Close()
        {
            if (!m_IsClosed)
            {
                m_IsClosed = true;

                foreach (var doc in Documents.ToArray())
                {
                    if (doc.IsCommitted && doc.IsAlive)
                    {
                        doc.Close();
                    }
                }

                Dispose();
            }
        }

        /// <summary>
        /// Releases COM resources owned by the application wrapper.
        /// 释放应用包装器持有的 COM 资源，避免 Document Manager 句柄泄漏。
        /// </summary>
        public void Dispose()
        {
            if (!m_IsDisposed)
            {
                m_IsDisposed = true;

                if (!m_IsClosed)
                {
                    Close();
                }

                if (Marshal.IsComObject(SwDocMgr))
                {
                    Marshal.ReleaseComObject(SwDocMgr);
                }

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        /// <summary>
        /// Finalizes the lazy application creation.
        /// 完成延迟创建流程，使预创建对象转为可用的 Document Manager 应用实例。
        /// </summary>
        public void Commit(CancellationToken cancellationToken) 
            => m_Creator.Create(cancellationToken);
    }

    /// <summary>
    /// Convenience extensions for version comparison.
    /// 便捷的版本比较扩展，便于按 SOLIDWORKS 年代版本做能力分支判断。
    /// </summary>
    public static class SwDmApplicationExtension
    {
        public static bool IsVersionNewerOrEqual(this ISwDmApplication app, SwDmVersion_e version) 
            => app.Version.IsVersionNewerOrEqual(version);
    }
}
