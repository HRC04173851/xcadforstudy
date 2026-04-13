//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xarial.XCad.Geometry.Structures
{
    /// <summary>
    /// Represents the line element
    /// 表示线段几何元素
    /// </summary>
    public class Line
    {
        /// <summary>
        /// Start point of the line
        /// 线段起点
        /// </summary>
        public Point StartPoint { get; set; }

        /// <summary>
        /// End point of the line
        /// 线段终点
        /// </summary>
        public Point EndPoint { get; set; }

        /// <summary>
        /// Default constructor
        /// 默认构造函数
        /// </summary>
        public Line() 
        {
        }

        /// <summary>
        /// Constructor with input coordinates
        /// 使用输入端点构造线段
        /// </summary>
        /// <param name="startPt">Start point（起点）</param>
        /// <param name="endPt">End point（终点）</param>
        public Line(Point startPt, Point endPt) 
        {
            StartPoint = startPt;
            EndPoint = endPt;
        }
    }
}
