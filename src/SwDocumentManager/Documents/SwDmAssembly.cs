//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using SolidWorks.Interop.swdocumentmgr;
using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Documents;
using Xarial.XCad.Documents.Delegates;
using Xarial.XCad.Geometry;

namespace Xarial.XCad.SwDocumentManager.Documents
{
    /// <summary>
    /// Assembly document contract for the Document Manager backend.
    /// 面向 Document Manager 后端的装配体文档约定。
    /// </summary>
    public interface ISwDmAssembly : ISwDmDocument3D, IXAssembly
    {
        new ISwDmAssemblyConfigurationCollection Configurations { get; }
    }

    /// <summary>
    /// Assembly document wrapper that exposes configuration-level assembly data through xCAD.
    /// 装配体文档包装器，通过 xCAD 暴露装配体的配置、组件树等离线数据。
    /// </summary>
    internal class SwDmAssembly : SwDmDocument3D, ISwDmAssembly
    {
        #region Not Supported
        public event ComponentInsertedDelegate ComponentInserted { add => throw new NotSupportedException(); remove => throw new NotSupportedException(); }
        public event ComponentDeletingDelegate ComponentDeleting { add => throw new NotSupportedException(); remove => throw new NotSupportedException(); }
        public event ComponentDeletedDelegate ComponentDeleted { add => throw new NotSupportedException(); remove => throw new NotSupportedException(); }

        IXAssemblyEvaluation IXAssembly.Evaluation => throw new NotSupportedException();
        IXComponent IXAssembly.EditingComponent => throw new NotSupportedException();
        #endregion

        private readonly Lazy<SwDmAssemblyConfigurationCollection> m_LazyConfigurations;

        /// <summary>
        /// Initializes the assembly document and defers configuration repository creation until needed.
        /// 初始化装配体文档，并延迟创建配置仓库以减少不必要的 COM 访问。
        /// </summary>
        public SwDmAssembly(SwDmApplication dmApp, ISwDMDocument doc, bool isCreated,
            Action<ISwDmDocument> createHandler, Action<ISwDmDocument> closeHandler, bool? isReadOnly)
            : base(dmApp, doc, isCreated, createHandler, closeHandler, isReadOnly)
        {
            m_LazyConfigurations = new Lazy<SwDmAssemblyConfigurationCollection>(() => new SwDmAssemblyConfigurationCollection(this));
        }

        IXAssemblyConfigurationRepository IXAssembly.Configurations => ((ISwDmAssembly)this).Configurations;

        ISwDmAssemblyConfigurationCollection ISwDmAssembly.Configurations => m_LazyConfigurations.Value;

        public override ISwDmConfigurationCollection Configurations => ((ISwDmAssembly)this).Configurations;

        protected override bool IsDocumentTypeCompatible(SwDmDocumentType docType) => docType == SwDmDocumentType.swDmDocumentAssembly;
    }

    /// <summary>
    /// Represents a virtual assembly embedded inside another assembly document.
    /// 表示嵌入在父装配体中的虚拟装配体文档。
    /// </summary>
    internal class SwDmVirtualAssembly : SwDmAssembly
    {
        private readonly SwDmDocument m_Owner;

        public SwDmVirtualAssembly(SwDmApplication dmApp, ISwDMDocument doc, SwDmDocument owner, bool isCreated,
            Action<ISwDmDocument> createHandler, Action<ISwDmDocument> closeHandler, bool? isReadOnly) 
            : base(dmApp, doc, isCreated, createHandler, closeHandler, isReadOnly)
        {
            m_Owner = owner;
            m_Owner.Destroyed += OnOwnerDisposed;
        }

        /// <summary>
        /// Closes the virtual document when the owning parent document is destroyed.
        /// 当所属父文档被销毁时，联动关闭该虚拟文档。
        /// </summary>
        private void OnOwnerDisposed(IXDocument owner)
        {
            this.Close();
        }

        public override string Title
        {
            get => SwDmVirtualDocumentHelper.GetTitle(base.Title);
            set => base.Title = value; 
        }

        public override bool IsDirty
        {
            get => base.IsDirty;
            set
            {
                base.IsDirty = value;

                if (value)
                {
                    m_Owner.IsDirty = true;
                }
            }
        }
    }
}
