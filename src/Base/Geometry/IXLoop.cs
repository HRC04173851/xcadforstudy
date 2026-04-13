//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Geometry.Curves;
using Xarial.XCad.Geometry.Wires;

namespace Xarial.XCad.Geometry
{
    /// <summary>
    /// Represents the connected and closed list of <see cref="IXCurve"/>
    /// 表示由曲线段首尾相接形成的闭合环（Loop）
    /// </summary>
    public interface IXLoop : IXSelObject, IXWireEntity
    {
        /// <summary>
        /// Connected and closed segments of this loop
        /// 构成该闭合环的连接线段集合
        /// </summary>
        IXSegment[] Segments { get; set; }
    }
}
