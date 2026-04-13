//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xarial.XCad.Geometry.Structures;

namespace Xarial.XCad.Annotations
{
    /// <summary>
    /// Represents the detail cricle annotation of <see cref="Documents.IXDetailedDrawingView"/>
    /// <see cref="Documents.IXDetailedDrawingView"/> 的局部放大圆标注
    /// </summary>
    public interface IXDetailCircle : IXAnnotation
    {
        /// <summary>
        /// Geometry of the detail circle
        /// 局部放大圆的几何定义
        /// </summary>
        Circle Definition { get; set; }
    }
}
