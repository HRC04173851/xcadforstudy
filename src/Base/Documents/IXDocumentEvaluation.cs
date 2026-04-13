//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Geometry.Evaluation;

namespace Xarial.XCad.Documents
{
    /// <summary>
    /// Provides evaluation features to the see <see cref="IXDocument3D"/>
    /// 为 <see cref="IXDocument3D"/> 提供评估功能入口
    /// </summary>
    public interface IXDocumentEvaluation
    {
        /// <summary>
        /// Pre creates the 3D bounding box of the document
        /// 预创建文档三维包围盒评估
        /// </summary>
        /// <returns>Bounding box</returns>
        IXBoundingBox PreCreateBoundingBox();

        /// <summary>
        /// Pre creates the mass property evaluator for the document
        /// 预创建文档质量属性评估器
        /// </summary>
        /// <returns>Mass property</returns>
        IXMassProperty PreCreateMassProperty();

        /// <summary>
        /// Pre creates ray intersection calculator
        /// 预创建射线相交计算器
        /// </summary>
        /// <returns>Ray intersection</returns>
        IXRayIntersection PreCreateRayIntersection();

        /// <summary>
        /// Pre creates a geometry tessellation
        /// 预创建几何三角剖分评估
        /// </summary>
        /// <returns>Tesselation</returns>
        IXTessellation PreCreateTessellation();

        /// <summary>
        /// Pre creates collision detection utility
        /// 预创建碰撞检测评估器
        /// </summary>
        /// <returns>Collision detection</returns>
        IXCollisionDetection PreCreateCollisionDetection();

        /// <summary>
        /// Pre creates measure utility
        /// 预创建测量工具
        /// </summary>
        /// <returns>Measure utility</returns>
        IXMeasure PreCreateMeasure();
    }

    /// <summary>
    /// Provides the specific evaluation for <see cref="IXAssembly"/>
    /// </summary>
    public interface IXAssemblyEvaluation : IXDocumentEvaluation
    {
        /// <summary>
        /// Pre creates the 3D bounding box of the assembly
        /// </summary>
        /// <returns>Bounding box</returns>
        new IXAssemblyBoundingBox PreCreateBoundingBox();

        /// <summary>
        /// Pre creates mass properties of the assembly
        /// </summary>
        /// <returns>Mass property</returns>
        new IXAssemblyMassProperty PreCreateMassProperty();

        /// <summary>
        /// Pre creates ray intersection calculator for assembly
        /// </summary>
        /// <returns>Ray intersection</returns>
        new IXAssemblyRayIntersection PreCreateRayIntersection();

        /// <summary>
        /// Pre creates a geometry tessellation for assembly
        /// </summary>
        /// <returns>Tesselation</returns>
        new IXAssemblyTessellation PreCreateTessellation();

        /// <summary>
        /// Pre creates collision detection utility for assembly
        /// </summary>
        /// <returns>Collision detection</returns>
        new IXAssemblyCollisionDetection PreCreateCollisionDetection();
    }
}
