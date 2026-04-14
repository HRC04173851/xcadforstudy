// -*- coding: utf-8 -*-
// src/Base/Geometry/Structures/Circle.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 表示圆几何元素，由中心轴线和直径定义，用于描述二维圆轮廓的三维几何信息。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xarial.XCad.Geometry.Structures
{
    /// <summary>
    /// Represents the circle geometry
    /// 表示圆几何（圆轮廓）
    /// </summary>
    public class Circle
    {
        /// <summary>
        /// Diameter of the circle
        /// 圆的直径
        /// </summary>
        public double Diameter { get; set; }

        /// <summary>
        /// Axis perpendicular to the circle's plane
        /// 与圆所在平面垂直的中心轴
        /// </summary>
        public Axis CenterAxis { get; set; }

        /// <summary>
        /// Default constructor
        /// 默认构造函数
        /// </summary>
        public Circle() 
        {
        }

        /// <summary>
        /// Constructor with geometry
        /// 带几何参数的构造函数
        /// </summary>
        /// <param name="centerAxis">Axis（圆心轴）</param>
        /// <param name="diam">Diameter（直径）</param>
        public Circle(Axis centerAxis, double diam) 
        {
            CenterAxis = centerAxis;
            Diameter = diam;
        }
    }
}
