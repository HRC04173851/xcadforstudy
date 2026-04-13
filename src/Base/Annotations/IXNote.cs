//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
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
