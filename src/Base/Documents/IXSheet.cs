// -*- coding: utf-8 -*-
// src/Base/Documents/IXSheet.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件定义工程图纸（Sheet）的跨CAD平台接口。
// Sheet 是工程图文档中的基本页面单位。
//
// Sheet 核心属性：
// - Name：图纸名称（如"Sheet1"、"A3-1"）
// - DrawingViews：图纸上的视图集合
// - Sketch：图纸的草图空间（用于手动绘制）
// - Scale：图纸比例
// - PaperSize：纸张大小（A4、A3 等）
//
// Sheet 与 Drawing 的关系：
// - IXDrawing：工程图文档（可包含多张图纸）
// - IXSheet：单张图纸（属于某个 Drawing）
//
// 图纸格式：
// - 标准图纸：A4、A3、A2、A1、A0
// - 自定义图纸：用户定义的尺寸
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Base;
using Xarial.XCad.Documents.Delegates;
using Xarial.XCad.Documents.Structures;
using Xarial.XCad.Features;
using Xarial.XCad.UI;

namespace Xarial.XCad.Documents
{
    /// <summary>
    /// Represents the drawing sheet
    /// <para>中文：表示工程图中的图纸</para>
    /// </summary>
    public interface IXSheet : IXSelObject, IXTransaction
    {
        /// <summary>
        /// Name of the sheet
        /// <para>中文：图纸名称</para>
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Drawings views on this sheet
        /// <para>中文：此图纸上的工程图视图集合</para>
        /// </summary>
        IXDrawingViewRepository DrawingViews { get; }

        /// <summary>
        /// Sketch space of this sheet
        /// <para>中文：此图纸的草图空间</para>
        /// </summary>
        IXSketch2D Sketch { get; }

        /// <summary>
        /// Preview of this drawing sheet
        /// <para>中文：此工程图图纸的预览图像</para>
        /// </summary>
        IXImage Preview { get; }

        /// <summary>
        /// Represents scale of this sheet
        /// <para>中文：表示此图纸的比例</para>
        /// </summary>
        Scale Scale { get; set; }

        /// <summary>
        /// Represents paper of this sheet
        /// <para>中文：表示此图纸的纸张大小</para>
        /// </summary>
        PaperSize PaperSize { get; set; }

        /// <summary>
        /// Creates a copy of this sheet
        /// <para>中文：创建此图纸的副本</para>
        /// </summary>
        /// <param name="targetDrawing">Drawing where to copy sheet to</param>
        /// <returns>Cloned sheet</returns>
        IXSheet Clone(IXDrawing targetDrawing);
    }
}
