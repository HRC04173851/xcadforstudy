// -*- coding: utf-8 -*-
// src/Base/Sketch/IXSketchBlockDefinition.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件定义草图块定义（Sketch Block Definition）的跨CAD平台接口。
// Block Definition 是块的模板，定义了块的几何形状和结构。
//
// Block 核心概念：
// 1. 定义与实例分离：Definition 是模板，Instance 是具体使用
// 2. 插入点：块插入到草图时的参考点（Origin）
// 3. 实体集合：块定义包含的所有草图实体
// 4. 实例引用：同一个定义可以创建多个实例
//
// Block 实例化：
// - IXSketchBlockInstance 是 Definition 的具体使用
// - 修改 Definition 会影响所有 Instance
// - 修改 Instance 只影响该实例
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Features;
using Xarial.XCad.Geometry.Structures;

namespace Xarial.XCad.Sketch
{
    /// <summary>
    /// Represents the defintion of <see cref="IXSketchBlockInstance"/>
    /// 表示 <see cref="IXSketchBlockInstance"/> 的块定义
    /// </summary>
    public interface IXSketchBlockDefinition : IXFeature
    {
        /// <summary>
        /// Insertion point of the sketch block definition
        /// 草图块定义的插入点
        /// </summary>
        Point InsertionPoint { get; }

        /// <summary>
        /// All instances of this sketch block defintion
        /// 该草图块定义的所有实例
        /// </summary>
        IEnumerable<IXSketchBlockInstance> Instances { get; }

        /// <summary>
        /// Entities of this sketch block definition
        /// 草图块定义包含的实体集合
        /// </summary>
        IXSketchEntityRepository Entities { get; }
    }
}
