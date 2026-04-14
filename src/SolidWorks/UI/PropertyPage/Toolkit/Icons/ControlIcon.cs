// -*- coding: utf-8 -*-
// PropertyPage/Toolkit/Icons/ControlIcon.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xiral Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 控件图标实现，用于属性管理器页面中控件的图标显示和管理。
//*********************************************************************

using System.Collections.Generic;
using System.Drawing;
using Xarial.XCad.SolidWorks.Base;
using Xarial.XCad.SolidWorks.Utils;
using Xarial.XCad.Toolkit.Base;
using Xarial.XCad.UI;

namespace Xarial.XCad.SolidWorks.UI.PropertyPage.Toolkit.Icons
{
    internal class ControlIcon : IIcon
    {
        internal IXImage Icon { get; }
        internal IXImage Mask { get; }

        public Color TransparencyKey => Color.White;

        public bool IsPermanent => false;

        public IconImageFormat_e Format => IconImageFormat_e.Bmp;

        private readonly Size m_Size;

        internal ControlIcon(IXImage icon)
            : this(icon, new Size(24, 24))
        {
        }

        internal ControlIcon(IXImage icon, Size size)
        {
            Icon = icon;
            m_Size = size;

            IconSizes = new IIconSpec[]
            {
                new IconSpec(Icon, m_Size),
                new IconSpec(Icon, m_Size, CreateMask)
            };
        }

        public IIconSpec[] IconSizes { get; }
        
        private void CreateMask(ref byte r, ref byte g, ref byte b, ref byte a)
        {
            var mask = (byte)(255 - a);
            r = mask;
            g = mask;
            b = mask;
            a = 255;
        }
    }
}