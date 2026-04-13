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
    /// Represents the section line annotation of <see cref="Documents.IXSectionDrawingView"/>
    /// <see cref="Documents.IXSectionDrawingView"/> 的剖面线标注
    /// </summary>
    public interface IXSectionLine : IXAnnotation
    {
        /// <summary>
        /// Geometry of the line
        /// 剖面线的几何定义
        /// </summary>
        Line Definition { get; set; }
    }
}
