// -*- coding: utf-8 -*-
// src/Base/Annotations/IXDetailCircle.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件定义详图圈（Detail Circle）的跨CAD平台接口。
// Detail Circle 是详图视图周围的圆圈标注。
//
// Detail Circle 核心概念：
// 1. 边界标识：圈出需要放大的区域
// 2. 关联详图：指向对应的详图视图
// 3. 圆样式：圆圈的大小、线型等样式
//
// 与视图的关系：
// - IXDetailCircle 属于 IXDetailedDrawingView
// - 详图视图显示被圆圈圈中区域的放大细节
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
    /// Represents the detail cricle annotation of <see cref="Documents.IXDetailedDrawingView"/>
    /// <see cref="Documents.IXDetailedDrawingView"/> 的局部放大圆标注
    /// </summary>
    public interface IXDetailCircle : IXAnnotation
    {
        /// <summary>
        /// Geometry of the detail circle
        /// 局部放大圆的几何定义
        /// </summary>
        Circle Definition { get; set; }
    }
}
