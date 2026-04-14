// -*- coding: utf-8 -*-
// src/Base/Geometry/Wires/IXWireEntity.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义线框实体的基接口(IXWireEntity)，继承自IXTransaction和IXObject，是点和线段的公共基类
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Base;

namespace Xarial.XCad.Geometry.Wires
{
    /// <summary>
    /// Represents the common entity for <see cref="IXPoint"/> and <see cref="IXSegment"/>
    /// 表示 <see cref="IXPoint"/> 与 <see cref="IXSegment"/> 的通用线框实体
    /// </summary>
    public interface IXWireEntity : IXTransaction, IXObject
    {
    }
}
