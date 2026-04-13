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
    /// Represents thickness
    /// 表示厚度/边距定义
    /// </summary>
    /// <remarks>Usually used to define margin and padding. 常用于界面布局中的外边距与内边距。</remarks>
    [DebuggerDisplay("{" + nameof(Left) + "} {" + nameof(Right) + "} {" + nameof(Top) + "} {" + nameof(Bottom) + "}")]
    public class Thickness
    {
        /// <summary>
        /// Left width
        /// 左侧厚度
        /// </summary>
        public double Left { get; }

        /// <summary>
        /// Right width
        /// 右侧厚度
        /// </summary>
        public double Right { get; }

        /// <summary>
        /// Toip width
        /// 上侧厚度
        /// </summary>
        public double Top { get; }

        /// <summary>
        /// Bottom width
        /// 下侧厚度
        /// </summary>
        public double Bottom { get; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Thickness(double left, double right, double top, double bottom) 
        {
            Left = left;
            Right = right;
            Top = top;
            Bottom = bottom;
        }

        /// <summary>
        /// Constructor for universal thickness
        /// </summary>
        /// <param name="width">Width</param>
        public Thickness(double width) : this(width, width, width, width)
        {
        }
    }
}
