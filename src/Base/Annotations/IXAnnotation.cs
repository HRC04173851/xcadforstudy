//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
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
