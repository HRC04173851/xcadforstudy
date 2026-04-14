// -*- coding: utf-8 -*-
// src/Base/Documents/IXPart.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件定义零件（Part）的跨CAD平台接口。
// 零件是 CAD 软件中创建三维几何模型的基本单元。
//
// 零件核心概念：
// 1. 几何体（Bodies）：零件包含一个或多个实体/曲面体
// 2. 特征（Features）：零件由特征树构建（拉伸、旋转、倒角等）
// 3. 草图（Sketch）：2D/3D 参数化草图，用于创建特征
// 4. 配置（Configurations）：同一零件的多种变体
//
// 零件与装配体的关系：
// - 零件是叶子节点：不能包含其他零件
// - 装配体是容器节点：可以包含零件和子装配体
// - 零件被引用到装配体中成为组件
//
// 零件类型：
// - 实体零件：包含有封闭体积的几何体
// - 曲面零件：包含没有封闭体积的片体
// - 混合零件：同时包含实体和曲面
//*********************************************************************

using Xarial.XCad.Documents.Delegates;
using Xarial.XCad.Geometry;

namespace Xarial.XCad.Documents
{
    /// <summary>
    /// Represents the part document
    /// <para>中文：表示零件文档</para>
    /// </summary>
    public interface IXPart : IXDocument3D
    {
        /// <inheritdoc/>
        new IXPartConfigurationRepository Configurations { get; }

        /// <summary>
        /// Bodies in this part document
        /// <para>中文：此零件文档中的实体集合</para>
        /// </summary>
        IXBodyRepository Bodies { get; }
    }
}