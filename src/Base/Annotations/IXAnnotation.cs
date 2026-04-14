// -*- coding: utf-8 -*-
// src/Base/Annotations/IXAnnotation.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件定义标注（Annotation）的跨CAD平台基础接口。
// Annotation 是 CAD 工程图中用于传递工程信息的元素。
//
// 标注类型层次：
// - IXAnnotation：标注基接口
//   - IXDimension：尺寸标注
//   - IXNote：文本注释
//   - IXSymbol：符号（表面粗糙度、焊接符号等）
//   - IXTable：表格（孔图表、明细表等）
//   - IXSectionLine：剖切线
//   - IXDetailCircle：详图圈
//
// 标注通用属性：
// - Position：标注位置
// - Font：标注字体
// - Color：标注颜色
// - Layer：标注所在图层
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Geometry.Structures;

namespace Xarial.XCad.Annotations
{
    /// <summary>
    /// Represents the base interface of annotation (e.g.<see cref="IXDimension"/>, <see cref="IXNote"/> etc.)
    /// 标注的基础接口（如 <see cref="IXDimension"/>、<see cref="IXNote"/> 等）
    /// </summary>
    public interface IXAnnotation : IXSelObject, IHasColor, IHasLayer
    {
        /// <summary>
        /// Position of this annotation
        /// 标注的位置
        /// </summary>
        Point Position { get; set; }

        /// <summary>
        /// Font of this annotation
        /// 标注的字体
        /// </summary>
        IFont Font { get; set; }
    }
}
