//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Xarial.XCad
{
    /// <summary>
    /// Identifies the visual object which can have color
    /// 标识可设置颜色的视觉对象
    /// </summary>
    public interface IHasColor : IXObject
    {
        /// <summary>
        /// Color of visual object
        /// 视觉对象的颜色
        /// </summary>
        Color? Color { get; set; }
    }
}
