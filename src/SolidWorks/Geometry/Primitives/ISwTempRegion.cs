// -*- coding: utf-8 -*-
// src/SolidWorks/Geometry/Primitives/ISwTempRegion.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件定义临时区域（Temp Region）的接口。
// 临时区域是用于构造几何体基元的临时面域，可参与拉伸、旋转等操作。
// ISwTempRegion 继承自 ISwPlanarRegion，提供区域的基本操作能力。
//*********************************************************************

using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Geometry.Primitives;
using Xarial.XCad.SolidWorks.Geometry.Curves;

namespace Xarial.XCad.SolidWorks.Geometry.Primitives
{
    public interface ISwTempRegion : ISwPlanarRegion
    {
    }
}
