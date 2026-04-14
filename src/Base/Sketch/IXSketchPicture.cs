// -*- coding: utf-8 -*-
// src/Base/Sketch/IXSketchPicture.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件定义草图图片（Sketch Picture）的跨CAD平台接口。
// Sketch Picture 是嵌入到草图中的图像元素。
//
// Sketch Picture 核心概念：
// 1. 图像数据：存储图片的二进制数据（IXImage）
// 2. 边界框：图片在草图中的位置和尺寸（Rect2D）
// 3. 插入点：图片插入时的参考点
//
// 使用场景：
// - 将徽标、公司标志等图片嵌入工程图
// - 将扫描图像作为草图参考
// - 创建带有图片注释的技术文档
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xarial.XCad.Features;
using Xarial.XCad.Geometry.Structures;
using Xarial.XCad.UI;

namespace Xarial.XCad.Sketch
{
    /// <summary>
    /// Represents sketch picture
    /// 表示草图图片
    /// </summary>
    public interface IXSketchPicture : IXSketchEntity, IXFeature
    {
        /// <summary>
        /// Image of this picture
        /// 该图片实体的图像数据
        /// </summary>
        IXImage Image { get; set; }

        /// <summary>
        /// Boundary of this picture
        /// 图片边界矩形
        /// </summary>
        Rect2D Boundary { get; set; }
    }
}
