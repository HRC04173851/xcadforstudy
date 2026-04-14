// -*- coding: utf-8 -*-
// src/Base/Geometry/Wires/IXLine.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义直线段(IXLine)的接口，继承自IXSegment接口，包含几何定义属性
//*********************************************************************

using Xarial.XCad.Geometry.Structures;

namespace Xarial.XCad.Geometry.Wires
{
    /// <summary>
    /// Represents a line segment
    /// 表示直线段
    /// </summary>
    public interface IXLine : IXSegment
    {
        /// <summary>
        /// Geometry of this line
        /// 该直线段的几何定义
        /// </summary>
        Line Geometry { get; set; }
    }
}
