// -*- coding: utf-8 -*-
// src/Base/Documents/IXDrawing.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件定义工程图（Drawing）的跨CAD平台接口。
// 工程图是 CAD 软件中用于生成二维工程图纸的文档类型。
//
// 工程图核心概念：
// 1. Sheets（图纸）：一张工程图可以包含多张图纸
// 2. Views（视图）：每张图纸可以包含多个视图（标准视图、剖视图等）
// 3. Layers（图层）：用于组织和管理图纸上的图形元素
// 4. Annotations（注解）：尺寸、公差、符号等工程信息
//
// 工程图文档结构：
// - IXDrawing：工程图文档接口
//   - IXSheetRepository：图纸集合
//   - IXLayerRepository：图层集合
//   - IXDrawingOptions：工程图选项
//
// 与 3D 文档的关系：
// - 工程图依赖零件或装配体模型
// - 通过视图投影生成二维图形
// - 支持从 3D 模型自动生成尺寸
//*********************************************************************

using Xarial.XCad.Documents.Structures;

namespace Xarial.XCad.Documents
{
    /// <summary>
    /// Represents the drawing (2D draft)
    /// <para>中文：表示工程图文档（二维制图）</para>
    /// </summary>
    public interface IXDrawing : IXDocument
    {
        /// <summary>
        /// Sheets on this drawing
        /// <para>中文：此工程图中的图纸集合</para>
        /// </summary>
        IXSheetRepository Sheets { get; }

        /// <summary>
        /// Drawing layers
        /// <para>中文：工程图图层集合</para>
        /// </summary>
        IXLayerRepository Layers { get; }

        /// <summary>
        /// Drawing specific options
        /// <para>中文：工程图专有选项</para>
        /// </summary>
        new IXDrawingOptions Options { get; }

        /// <summary>
        /// <see cref="IXDrawing"/> specific save as operation
        /// <para>中文：工程图专用的另存为操作</para>
        /// </summary>
        new IXDrawingSaveOperation PreCreateSaveAsOperation(string filePath);
    }
}