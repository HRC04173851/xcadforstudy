// -*- coding: utf-8 -*-
// src/SwDocumentManager/Documents/SwDmPart.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 零件文档包装器实现，负责暴露配置以及零件特有的数据入口，支持虚拟零件文档。
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
    /// Part document contract.
    /// 零件文档约定。
    /// </summary>
    public interface ISwDmPart : ISwDmDocument3D, IXPart
    {
        new ISwDmPartConfigurationCollection Configurations { get; }
    }

    /// <summary>
    /// Part document wrapper for configurations and part-specific data.
    /// 零件文档包装器，负责暴露配置以及零件特有的数据入口。
    /// </summary>
    internal class SwDmPart : SwDmDocument3D, ISwDmPart
    {
        private readonly Lazy<SwDmPartConfigurationCollection> m_LazyConfigurations;

        /// <summary>
        /// Initializes the part wrapper and delays configuration creation until needed.
        /// 初始化零件包装器，并在需要时再延迟创建配置集合。
        /// </summary>
        internal SwDmPart(SwDmApplication dmApp, ISwDMDocument doc, bool isCreated,
            Action<ISwDmDocument> createHandler, Action<ISwDmDocument> closeHandler, bool? isReadOnly)
            : base(dmApp, doc, isCreated, createHandler, closeHandler, isReadOnly)
        {
            m_LazyConfigurations = new Lazy<SwDmPartConfigurationCollection>(() => new SwDmPartConfigurationCollection(this));
        }

        public IXBodyRepository Bodies => throw new NotImplementedException();

        IXPartConfigurationRepository IXPart.Configurations => m_LazyConfigurations.Value;

        public override ISwDmConfigurationCollection Configurations => m_LazyConfigurations.Value;

        ISwDmPartConfigurationCollection ISwDmPart.Configurations => m_LazyConfigurations.Value;

        protected override bool IsDocumentTypeCompatible(SwDmDocumentType docType) => docType == SwDmDocumentType.swDmDocumentPart;
    }

    /// <summary>
    /// Represents a virtual part embedded into another owning document.
    /// 表示嵌入在其他父文档中的虚拟零件。
    /// </summary>
    internal class SwDmVirtualPart : SwDmPart
    {
        private readonly SwDmDocument m_Owner;

        internal SwDmVirtualPart(SwDmApplication dmApp, ISwDMDocument doc, SwDmDocument owner, bool isCreated,
            Action<ISwDmDocument> createHandler, Action<ISwDmDocument> closeHandler, bool? isReadOnly) 
            : base(dmApp, doc, isCreated, createHandler, closeHandler, isReadOnly)
        {
            m_Owner = owner;
            m_Owner.Destroyed += OnOwnerDisposed;
        }

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
