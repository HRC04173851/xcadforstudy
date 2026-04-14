// -*- coding: utf-8 -*-
// src/Base/Geometry/Evaluation/IXCollisionDetection.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 碰撞检测接口，用于检测几何体或装配组件之间是否发生干涉，并返回碰撞体和碰撞体积的几何信息。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xarial.XCad.Base;
using Xarial.XCad.Documents;

namespace Xarial.XCad.Geometry.Evaluation
{
    /// <summary>
    /// Represents the result of the collision
    /// 表示干涉/碰撞检测结果
    /// </summary>
    public interface IXCollisionResult
    {
        /// <summary>
        /// Bodies that are involved in collision
        /// 发生碰撞的几何体集合
        /// </summary>
        IXBody[] CollidedBodies { get; }

        /// <summary>
        /// Represents the geometry of the collision
        /// 碰撞区域对应的几何体（体积）
        /// </summary>
        IXMemoryBody[] CollisionVolume { get; }
    }

    /// <summary>
    /// Represents the result of the collision in the assembly
    /// </summary>
    public interface IXAssemblyCollisionResult : IXCollisionResult
    {
        /// <summary>
        /// Components which are involved in the collision
        /// </summary>
        IXComponent[] CollidedComponents { get; }
    }

    /// <summary>
    /// Represents collision detection between elements
    /// 表示元素间碰撞检测评估
    /// </summary>
    public interface IXCollisionDetection : IEvaluation
    {
        /// <summary>
        /// Collision results
        /// 碰撞结果集合
        /// </summary>
        IXCollisionResult[] Results { get; }
    }

    /// <summary>
    /// Represents collision detection between components in the assembly
    /// </summary>
    public interface IXAssemblyCollisionDetection : IXCollisionDetection, IAssemblyEvaluation
    {
        /// <inheritdoc/>
        new IXAssemblyCollisionResult[] Results { get; }
    }
}
