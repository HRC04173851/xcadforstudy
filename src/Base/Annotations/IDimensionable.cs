// -*- coding: utf-8 -*-
// src/Base/Annotations/IDimensionable.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件定义可标注接口（IDimensionable）。
// 实现此接口的对象可以包含尺寸标注。
//
// 实现此接口的对象：
// - IXFeature：特征可以标注尺寸
// - IXComponent：组件可以标注尺寸
// - IXBody：几何体可以标注尺寸
// - IXDrawingView：视图可以标注尺寸
//
// 使用方式：
// - 通过 IDimensionable.Dimensions 获取对象关联的尺寸
// - 可以添加、删除、修改尺寸
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.Annotations
{
    /// <summary>
    /// Indicates that this object can have dimensions
    /// 表示该对象可以包含尺寸标注
    /// </summary>
    public interface IDimensionable
    {
        /// <summary>
        /// Dimensions repository
        /// 尺寸集合
        /// </summary>
        IXDimensionRepository Dimensions { get; }
    }
}
