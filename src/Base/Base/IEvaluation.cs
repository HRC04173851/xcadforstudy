// -*- coding: utf-8 -*-
// IEvaluation.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义几何求值功能接口，支持相对于变换矩阵的几何计算、用户单位/系统单位切换、
// 可见实体筛选、精确/近似计算模式，以及程序集级别的几何求值扩展
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Documents;
using Xarial.XCad.Geometry;
using Xarial.XCad.Geometry.Structures;

namespace Xarial.XCad.Base
{
    /// <summary>
    /// This interfaces represents the geometrical evaluation features
    /// 此接口表示几何求値功能
    /// </summary>
    public interface IEvaluation : IXTransaction, IDisposable
    {
        /// <summary>
        /// Evaluates geometry relative to the specified transformation matrix
        /// 相对于指定变换矩阵进行几何求値
        /// </summary>
        TransformMatrix RelativeTo { get; set; }

        /// <summary>
        /// True to use user specific units, false to use system units
        /// true 使用用户自定义单位，false 使用系统单位
        /// </summary>
        bool UserUnits { get; set; }

        /// <summary>
        /// True to only use visible bodies or components
        /// true 仅对可见实体或组件进行求値
        /// </summary>
        bool VisibleOnly { get; set; }

        /// <summary>
        /// Bodies to include into the evaluation
        /// 包含在求値范围内的实体
        /// </summary>
        IXBody[] Scope { get; set; }

        /// <summary>
        /// True to calculate precise data, false to calculate approximate data
        /// </summary>
        bool Precise { get; set; }
    }

    /// <summary>
    /// Assembly specific geometrical evaluation feature
    /// </summary>
    public interface IAssemblyEvaluation : IEvaluation
    {
        /// <summary>
        /// Components to include into the evaluation
        /// </summary>
        new IXComponent[] Scope { get; set; }
    }
}
