// -*- coding: utf-8 -*-
// src/Base/Annotations/IXNote.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件定义文本注释（Note）的跨CAD平台接口。
// Note 是工程图中用于添加自由文本的标注类型。
//
// Note 核心属性：
// - Text：注释的文本内容
// - Box：注释的边界框
// - Angle：注释的旋转角度
// - TextJustification：文本对齐方式
//
// Note 与其他标注的区别：
// - Note：自由文本，不关联任何几何对象
// - IXDimension：关联几何对象的尺寸标注
// - IXSymbol：符号标注（表面粗糙度、焊接符号等）
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Enums;
using Xarial.XCad.Geometry.Structures;

namespace Xarial.XCad.Annotations
{
    /// <summary>
    /// Represents the note annotation
    /// 文字注释标注
    /// </summary>
    public interface IXNote : IXAnnotation
    {
        /// <summary>
        /// Boundary of this note
        /// 注释的边界框
        /// </summary>
        Box3D Box { get; }

        /// <summary>
        /// Text of the note
        /// 注释的文本内容
        /// </summary>
        string Text { get; set; }

        /// <summary>
        /// Angle of this note in radians
        /// 注释的旋转角度（弧度）
        /// </summary>
        double Angle { get; set; }

        /// <summary>
        /// Text justification of the note
        /// 注释的文本对齐方式
        /// </summary>
        TextJustification_e TextJustification { get; set; }
    }
}
