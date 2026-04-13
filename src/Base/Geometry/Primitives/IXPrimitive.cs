//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
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
