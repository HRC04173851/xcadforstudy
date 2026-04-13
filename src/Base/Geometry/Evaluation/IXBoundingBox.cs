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
    /// Represents the bounding box of the geometrical object
    /// 表示几何对象的包围盒评估结果
    /// </summary>
    public interface IXBoundingBox : IEvaluation
    {
        /// <summary>
        /// True to automatically find the best orientation to fit
        /// 为 true 时自动搜索最优朝向以获得更紧凑包围盒
        /// </summary>
        bool BestFit { get; set; }

        /// <summary>
        /// Bounding box data
        /// 包围盒数据
        /// </summary>
        Box3D Box { get; }
    }

    /// <summary>
    /// Bounding box specific to the assembly
    /// 装配体上下文专用包围盒评估
    /// </summary>
    public interface IXAssemblyBoundingBox : IXBoundingBox, IAssemblyEvaluation
    {
    }
}
