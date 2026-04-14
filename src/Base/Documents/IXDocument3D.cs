// -*- coding: utf-8 -*-
// src/Base/Documents/IXDocument3D.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义三维文档接口，表示装配体或零件文档，
// 提供评估、图形、三维视图和配置管理功能。
//*********************************************************************

using Xarial.XCad.Base;
using Xarial.XCad.Geometry;
using Xarial.XCad.Geometry.Structures;
using Xarial.XCad.UI;

namespace Xarial.XCad.Documents
{
    /// <summary>
    /// Represents 3D document (assembly or part)
    /// <para>中文：表示三维文档（装配体或零件）</para>
    /// </summary>
    public interface IXDocument3D : IXDocument, IXObjectContainer
    {
        /// <summary>
        /// Access to the document's evaluation features
        /// <para>中文：访问文档的评估功能（如质量属性、干涉检查等）</para>
        /// </summary>
        IXDocumentEvaluation Evaluation { get; }

        /// <summary>
        /// Access the document's graphics features
        /// <para>中文：访问文档的图形功能</para>
        /// </summary>
        IXDocumentGraphics Graphics { get; }

        /// <summary>
        /// Returns 3D views collection
        /// <para>中文：返回三维模型视图集合</para>
        /// </summary>
        new IXModelView3DRepository ModelViews { get; }

        /// <summary>
        /// Returns configurations of this document
        /// <para>中文：返回此文档的配置集合</para>
        /// </summary>
        IXConfigurationRepository Configurations { get; }

        /// <summary>
        /// <see cref="IXDocument3D"/> specific save as operation
        /// <para>中文：三维文档专用的另存为操作</para>
        /// </summary>
        new IXDocument3DSaveOperation PreCreateSaveAsOperation(string filePath);
    }
}