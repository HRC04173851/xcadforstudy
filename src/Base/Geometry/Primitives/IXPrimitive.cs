// -*- coding: utf-8 -*-
// src/Base/Geometry/Primitives/IXPrimitive.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义所有几何基元的基接口，继承自事务接口，提供几何体集合属性，是xCAD几何模块中最顶层的抽象定义。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Base;
using Xarial.XCad.Geometry.Structures;
using Xarial.XCad.Geometry.Wires;

namespace Xarial.XCad.Geometry.Primitives
{
    /// <summary>
    /// Represents the 3D geometry of a primitive
    /// 表示三维几何基元
    /// </summary>
    public interface IXPrimitive : IXTransaction
    {
        /// <summary>
        /// Bodies associated with this primitive
        /// 与该几何基元关联的几何体集合
        /// </summary>
        IXBody[] Bodies { get; }
    }
}
