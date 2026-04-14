// -*- coding: utf-8 -*-
// PropertyPage/Toolkit/Icons/TabIcon.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 选项卡图标实现，用于SOLIDWORKS属性管理器页面选项卡的图标显示。
//*********************************************************************

using System.Collections.Generic;
using System.Drawing;
using Xarial.XCad.SolidWorks.Base;
using Xarial.XCad.Toolkit.Base;
using Xarial.XCad.UI;

namespace Xarial.XCad.SolidWorks.UI.PropertyPage.Toolkit.Icons
{
    internal class TabIcon : IIcon
    {
        internal IXImage Icon { get; }

        public Color TransparencyKey => Color.White;

        public bool IsPermanent => false;

        public IconImageFormat_e Format => IconImageFormat_e.Bmp;

        internal TabIcon(IXImage icon)
        {
            Icon = icon;
            IconSizes = new IIconSpec[]
            {
                new IconSpec(Icon, new Size(16, 18))
            };
        }

        public IIconSpec[] IconSizes { get; }
    }
}