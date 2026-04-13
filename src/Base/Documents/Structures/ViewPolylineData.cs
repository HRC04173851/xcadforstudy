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
using Xarial.XCad.Geometry;
using Xarial.XCad.Geometry.Structures;

namespace Xarial.XCad.Documents.Structures
{
    /// <summary>
    /// Represents the polyline information of <see cref="IXDrawingView"/>
    /// 表示 <see cref="IXDrawingView"/> 的多段线信息
    /// </summary>
    public class ViewPolylineData
    {
        /// <summary>
        /// Tesselation points of the polyline
        /// 多段线离散点（剖分点）
        /// </summary>
        public Point[] Points { get; }

        /// <summary>
        /// Correposnidng 3D model entity
        /// 对应的三维模型实体
        /// </summary>
        public IXEntity Entity { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        public ViewPolylineData(IXEntity entity, Point[] points)
        {
            Entity = entity;
            Points = points;
        }
    }
}
