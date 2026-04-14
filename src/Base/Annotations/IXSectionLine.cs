// -*- coding: utf-8 -*-
// src/Base/Annotations/IXSectionLine.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件定义剖面线（Section Line）的跨CAD平台接口。
// Section Line 是剖视图中的剖切线标注。
//
// Section Line 核心概念：
// 1. 剖切位置：定义剖切平面在哪里切割模型
// 2. 剖切方向：指示剖视图的投影方向
// 3. 箭头样式：剖切线的箭头显示方式
// 4. 剖切线类型：全剖、半剖、阶梯剖等
//
// 与视图的关系：
// - IXSectionLine 属于 IXSectionDrawingView
// - 剖视图会沿剖切线投影生成二维图形
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xarial.XCad.Geometry.Structures;

namespace Xarial.XCad.Annotations
{
    /// <summary>
    /// Represents the section line annotation of <see cref="Documents.IXSectionDrawingView"/>
    /// <see cref="Documents.IXSectionDrawingView"/> 的剖面线标注
    /// </summary>
    public interface IXSectionLine : IXAnnotation
    {
        /// <summary>
        /// Geometry of the line
        /// 剖面线的几何定义
        /// </summary>
        Line Definition { get; set; }
    }
}
