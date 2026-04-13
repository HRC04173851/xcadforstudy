//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Xarial.XCad.Geometry.Structures
{
    /// <summary>
    /// Represents axis - direction through the point
    /// 表示空间中的轴线（通过参考点并沿指定方向延伸）
    /// </summary>
    [DebuggerDisplay("{" + nameof(Point) + "} - {" + nameof(Direction) + "}")]
    public class Axis
    {
        /// <summary>
        /// Reference point of this axis
        /// 该轴线的参考点（轴线上一点）
        /// </summary>
        public Point Point { get; set; }

        /// <summary>
        /// Direction of this axis
        /// 该轴线的方向向量
        /// </summary>
        public Vector Direction { get; set; }

        /// <summary>
        /// Default constructor
        /// 默认构造函数
        /// </summary>
        public Axis() 
        {
        }

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="refPt">Reference point of this exis（轴线参考点）</param>
        /// <param name="dir">Direction of the exis（轴线方向）</param>
        public Axis(Point refPt, Vector dir) 
        {
            Point = refPt;
            Direction = dir;
        }
    }
}
