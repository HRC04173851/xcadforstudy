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
using Xarial.XCad.Features;
using Xarial.XCad.Geometry.Structures;
using Xarial.XCad.UI;

namespace Xarial.XCad.Sketch
{
    /// <summary>
    /// Represents sketch picture
    /// 表示草图图片
    /// </summary>
    public interface IXSketchPicture : IXSketchEntity, IXFeature
    {
        /// <summary>
        /// Image of this picture
        /// 该图片实体的图像数据
        /// </summary>
        IXImage Image { get; set; }

        /// <summary>
        /// Boundary of this picture
        /// 图片边界矩形
        /// </summary>
        Rect2D Boundary { get; set; }
    }
}
