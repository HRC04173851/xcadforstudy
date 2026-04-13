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
using Xarial.XCad.SolidWorks.Base;
using Xarial.XCad.Toolkit.Base;
using Xarial.XCad.UI;

namespace Xarial.XCad.SolidWorks.UI
{
    /// <summary>
    /// 工具提示图标包装。
    /// 固定输出 16x16 BMP 图标用于提示气泡或小型 UI 元素。
    /// </summary>
    internal class TooltipIcon : IIcon
    {
        internal IXImage Icon { get; }

        public Color TransparencyKey => Color.White;

        public bool IsPermanent => false;

        public IconImageFormat_e Format => IconImageFormat_e.Bmp;

        internal TooltipIcon(IXImage icon)
        {
            Icon = icon;

            IconSizes = new IIconSpec[]
            {
                new IconSpec(Icon, new Size(16, 16))
            };
        }

        public IIconSpec[] IconSizes { get; }
    }
}
