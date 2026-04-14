// -*- coding: utf-8 -*-
// src/Base/Documents/IXLayer.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件定义图层（Layer）的跨CAD平台接口。
// Layer 用于在工程图或建模环境中组织和管理图形元素。
//
// Layer 核心概念：
// 1. 组织功能：将相关的图形元素归类到同一图层
// 2. 可见性控制：控制整张图层的显示/隐藏
// 3. 颜色管理：图层可以具有默认颜色
// 4. 线型管理：图层可以具有默认线型
//
// Layer 属性：
// - Name：图层名称
// - Visible：图层可见性
// - Color：图层颜色
// - LineStyle：图层线型（可选）
// - LineWeight：图层线宽（可选）
//
// 实现接口：
// - IHasColor：图层具有颜色属性
// - IXTransaction：支持创建/修改事务
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xarial.XCad.Base;

namespace Xarial.XCad.Documents
{
    /// <summary>
    /// Represents layer for the entitites
    /// 表示实体图层
    /// </summary>
    /// <remarks>Entities which support layer are implementing <see cref="IHasLayer"/></remarks>
    public interface IXLayer : IXTransaction, IXObject, IHasColor
    {
        /// <summary>
        /// Name of the layer
        /// 图层名称
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Visibility of the layer
        /// 图层可见性
        /// </summary>
        bool Visible { get; set; }
    }
}
