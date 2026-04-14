// -*- coding: utf-8 -*-
// src/SolidWorks/Documents/SwPart.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件实现 SolidWorks 零件文档（Part Document）的封装。
// 零件文档是 SOLIDWORKS 中创建三维几何模型的基本文档类型。
//
// 零件文档核心功能：
// 1. 实体/曲面体管理（Bodies）- 访问零件中的所有几何体
// 2. 配置管理（Configurations）- 支持零件的多配置
// 3. 草图管理 - 创建和编辑2D/3D草图
// 4. 特征管理 - 通过 FeatureManager 访问所有特征
//
// 零件 vs 装配体 vs 工程图：
// - Part（零件）：独立的零件模型，包含几何体和特征
// - Assembly（装配体）：多个零件的装配，有组件层级
// - Drawing（工程图）：零件/装配体的二维投影图纸
//
// 与 SwDocument3D 的关系：
// SwPart 继承自 SwDocument3D，添加了零件特有的：
// - Bodies 属性：管理零件的实体/曲面体集合
// - Configurations：零件级别的配置集合
//*********************************************************************

using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Collections.Generic;
using System.Linq;
using Xarial.XCad.Base;
using Xarial.XCad.Documents;
using Xarial.XCad.Documents.Delegates;
using Xarial.XCad.Geometry;
using Xarial.XCad.Geometry.Structures;
using Xarial.XCad.SolidWorks.Documents.EventHandlers;
using Xarial.XCad.SolidWorks.Geometry;
using Xarial.XCad.Utils.Diagnostics;

namespace Xarial.XCad.SolidWorks.Documents
{
    /// <summary>
    /// Represents a SolidWorks part document.
    /// <para>中文：表示 SolidWorks 零件文档的接口。</para>
    /// </summary>
    public interface ISwPart : ISwDocument3D, IXPart 
    {
        /// <summary>
        /// Gets the underlying SolidWorks <see cref="IPartDoc"/> COM object.
        /// <para>中文：获取底层 SolidWorks <see cref="IPartDoc"/> COM 对象。</para>
        /// </summary>
        IPartDoc Part { get; }

        /// <summary>
        /// Gets the part-specific configuration collection.
        /// <para>中文：获取零件文档专用的配置集合。</para>
        /// </summary>
        new ISwPartConfigurationCollection Configurations { get; }
    }

    /// <summary>
    /// SolidWorks part document implementation.
    /// <para>中文：SolidWorks 零件文档的实现类。</para>
    /// </summary>
    internal class SwPart : SwDocument3D, ISwPart
    {
        // Explicit interface: expose part configuration collection via xCAD base interface
        // 中文：显式接口：通过 xCAD 基础接口公开零件配置集合
        IXPartConfigurationRepository IXPart.Configurations => (IXPartConfigurationRepository)Configurations;

        /// <summary>
        /// Gets the underlying SolidWorks part document COM object.
        /// <para>中文：获取底层 SolidWorks 零件文档 COM 对象。</para>
        /// </summary>
        public IPartDoc Part => Model as IPartDoc;

        /// <summary>
        /// Gets the body repository containing all solid/surface bodies in this part.
        /// <para>中文：获取包含该零件所有实体/曲面体的几何体存储库。</para>
        /// </summary>
        public IXBodyRepository Bodies { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="SwPart"/>.
        /// <para>中文：初始化 <see cref="SwPart"/> 的新实例，创建几何体集合和评估对象。</para>
        /// </summary>
        internal SwPart(IPartDoc part, SwApplication app, IXLogger logger, bool isCreated)
            : base((IModelDoc2)part, app, logger, isCreated)
        {
            Bodies = new SwPartBodyCollection(this);
            Evaluation = new SwPartEvaluation(this);
        }

        /// <summary>
        /// Gets the SolidWorks document type identifier for a part document.
        /// <para>中文：获取零件文档对应的 SolidWorks 文档类型标识符。</para>
        /// </summary>
        internal protected override swDocumentTypes_e? DocumentType => swDocumentTypes_e.swDocPART;

        /// <summary>
        /// Part documents are never opened in lightweight mode.
        /// <para>中文：零件文档不支持轻量化模式，始终返回 false。</para>
        /// </summary>
        protected override bool IsLightweightMode => false;

        /// <summary>
        /// Part documents do not support rapid (detailing) mode.
        /// <para>中文：零件文档不支持快速模式（详图模式），始终返回 false。</para>
        /// </summary>
        protected override bool IsRapidMode => false;

        /// <summary>
        /// Creates the annotation collection for a 3D document.
        /// <para>中文：为3D文档创建注解集合。</para>
        /// </summary>
        protected override SwAnnotationCollection CreateAnnotations() => new SwDocument3DAnnotationCollection(this);

        // Explicit interface: cast the base configuration collection to the part-specific type
        // 中文：显式接口：将基础配置集合强制转换为零件专用类型
        ISwPartConfigurationCollection ISwPart.Configurations => (ISwPartConfigurationCollection)Configurations;

        /// <summary>
        /// Gets the document evaluation object for mass properties, bounding box, etc.
        /// <para>中文：获取文档评估对象，用于质量属性、包围盒等分析。</para>
        /// </summary>
        public override IXDocumentEvaluation Evaluation { get; }

        /// <summary>
        /// Creates the part-specific configuration collection.
        /// <para>中文：创建零件文档专用的配置集合。</para>
        /// </summary>
        protected override SwConfigurationCollection CreateConfigurations()
            => new SwPartConfigurationCollection(this, OwnerApplication);

        /// <summary>
        /// Returns true only if the given document type is a part document.
        /// <para>中文：仅当给定的文档类型为零件文档时返回 true。</para>
        /// </summary>
        protected override bool IsDocumentTypeCompatible(swDocumentTypes_e docType) => docType == swDocumentTypes_e.swDocPART;
    }

    /// <summary>
    /// Collection of solid/surface bodies in a SolidWorks part document.
    /// <para>中文：SolidWorks 零件文档中实体/曲面体的几何体集合。</para>
    /// </summary>
    internal class SwPartBodyCollection : SwBodyCollection
    {
        // Reference to the owning part document
        // 中文：对所属零件文档的引用
        private SwPart m_Part;

        /// <summary>
        /// Initializes a new instance bound to the given part document.
        /// <para>中文：初始化绑定到指定零件文档的新实例。</para>
        /// </summary>
        public SwPartBodyCollection(SwPart rootDoc) : base(rootDoc)
        {
            m_Part = rootDoc;
        }

        /// <summary>
        /// Returns the SolidWorks bodies of the specified type from the part document.
        /// <para>中文：从零件文档中返回指定类型的 SolidWorks 几何体。</para>
        /// </summary>
        protected override IEnumerable<IBody2> SelectSwBodies(swBodyType_e bodyType)
            // GetBodies2: second parameter false = include all bodies (not just visible)
            // 中文：GetBodies2 第二个参数为 false 表示包含所有几何体（不仅限于可见体）
            => (m_Part.Part.GetBodies2((int)bodyType, false) as object[])?.Cast<IBody2>();
    }
}