//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Base;
using Xarial.XCad.Documents;
using Xarial.XCad.Geometry.Structures;

namespace Xarial.XCad.Geometry.Evaluation
{
    /// <summary>
    /// Evaluates mass properties of the document
    /// 评估文档的质量属性（Mass Properties）
    /// </summary>
    public interface IXMassProperty : IEvaluation
    {
        /// <summary>
        /// Center of Gravity
        /// 质心
        /// </summary>
        Point CenterOfGravity { get; }

        /// <summary>
        /// Surface area
        /// 表面积
        /// </summary>
        double SurfaceArea { get; }

        /// <summary>
        /// Volume
        /// 体积
        /// </summary>
        double Volume { get; }

        /// <summary>
        /// Mass
        /// 质量
        /// </summary>
        double Mass { get; }

        /// <summary>
        /// Density
        /// 密度
        /// </summary>
        double Density { get; }

        /// <summary>
        /// Principal axes of inertia
        /// 主惯性轴
        /// </summary>
        PrincipalAxesOfInertia PrincipalAxesOfInertia { get; }

        /// <summary>
        /// Principal moment of inertia
        /// 主惯性矩
        /// </summary>
        PrincipalMomentOfInertia PrincipalMomentOfInertia { get; }

        /// <summary>
        /// Moment of inertia
        /// 惯性矩
        /// </summary>
        MomentOfInertia MomentOfInertia { get; }
    }

    /// <summary>
    /// Evaluates mass properties of the assembly
    /// </summary>
    public interface IXAssemblyMassProperty : IXMassProperty, IAssemblyEvaluation
    {
    }
}