//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Geometry.Structures;

namespace Xarial.XCad.Features
{
    /// <summary>
    /// Represents the coordinate system feature
    /// 表示坐标系特征
    /// </summary>
    public interface IXCoordinateSystem : IXFeature
    {
        /// <summary>
        /// Transformation of this coordinate system
        /// 该坐标系相对于全局坐标系的变换矩阵
        /// </summary>
        TransformMatrix Transform { get; }
    }
}
