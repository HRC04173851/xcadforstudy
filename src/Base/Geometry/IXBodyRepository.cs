// -*- coding: utf-8 -*-
// src/Base/Geometry/IXBodyRepository.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件定义几何体集合仓储（Body Repository）的接口。
// Body Repository 提供对文档中所有几何体的统一访问接口。
//
// 几何体类型：
// - SolidBody：实体几何体，有封闭体积
// - SheetBody：片体/曲面体，没有封闭体积
// - HybridBody：混合体，包含实体和曲面
//
// Repository 模式：
// - IXRepository&lt;T&gt;：标准仓储接口，提供 Find、FindAll、Create 等操作
// - IXBodyRepository：几何体专用仓储，继承自 IXRepository&lt;IXBody&gt;
//
// 使用场景：
// - 遍历零件中的所有几何体
// - 按名称或类型查找几何体
// - 添加/删除几何体
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Base;

namespace Xarial.XCad.Geometry
{
    /// <summary>
    /// Represents the collection of bodies
    /// 表示几何体集合仓储
    /// </summary>
    public interface IXBodyRepository : IXRepository<IXBody>
    {
    }
}
