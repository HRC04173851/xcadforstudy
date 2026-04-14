// -*- coding: utf-8 -*-
// src/Base/Geometry/Wires/IXPoint.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义点实体(IXPoint)的接口，继承自IXWireEntity接口，表示空间中的点
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Base;
using Xarial.XCad.Geometry.Structures;

namespace Xarial.XCad.Geometry.Wires
{
    /// <summary>
    /// Represents the point entity
    /// 表示点实体
    /// </summary>
    public interface IXPoint : IXWireEntity
    {
        /// <summary>
        /// Coodinate of the point
        /// 点坐标
        /// </summary>
        Point Coordinate { get; set; }
    }
}
