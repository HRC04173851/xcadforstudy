// -*- coding: utf-8 -*-
// src/Base/Geometry/Structures/PrincipalAxesOfInertia.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 表示主惯性轴，由三个互相垂直的主轴方向向量Ix、Iy、Iz组成，用于描述实体的主惯性方向。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Geometry.Evaluation;

namespace Xarial.XCad.Geometry.Structures
{
    /// <summary>
    /// Principal axes of inertia of the solid geometry used in <see cref="IXMassProperty"/>
    /// 用于 <see cref="IXMassProperty"/> 的实体主惯性轴
    /// </summary>
    public class PrincipalAxesOfInertia
    {
        /// <summary>
        /// X direction
        /// 第一主轴方向
        /// </summary>
        public Vector Ix { get; }

        /// <summary>
        /// Y direction
        /// 第二主轴方向
        /// </summary>
        public Vector Iy { get; }

        /// <summary>
        /// Z direction
        /// 第三主轴方向
        /// </summary>
        public Vector Iz { get; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public PrincipalAxesOfInertia(Vector ix, Vector iy, Vector iz)
        {
            Ix = ix;
            Iy = iy;
            Iz = iz;
        }
    }
}
