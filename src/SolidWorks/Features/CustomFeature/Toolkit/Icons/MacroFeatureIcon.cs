// -*- coding: utf-8 -*-
// src/SolidWorks/Features/CustomFeature/Toolkit/Icons/MacroFeatureIcon.cs

﻿//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Drawing;
using Xarial.XCad.SolidWorks.Base;
using Xarial.XCad.Toolkit.Base;
using Xarial.XCad.Toolkit.Utils;
using Xarial.XCad.UI;

namespace Xarial.XCad.SolidWorks.Features.CustomFeature.Toolkit.Icons
{
    /// <summary>
    /// 宏特性图标类
    /// <para>表示宏特性的图标及其尺寸规格</para>
    /// </summary>
    internal class MacroFeatureIcon : IIcon
    {
        /// <summary>基础名称（如 Regular、Suppressed、Highlighted）</summary>
        protected readonly string m_BaseName;
        /// <summary>图标图像</summary>
        protected readonly IXImage m_Icon;

        /// <summary>透明色键 - 白色被用作透明色</summary>
        public Color TransparencyKey => Color.White;

        /// <summary>图标是永久性的，不会被自动清理</summary>
        public bool IsPermanent => true;

        /// <summary>图标图像格式</summary>
        public IconImageFormat_e Format => IconImageFormat_e.Bmp;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="icon">图标图像</param>
        /// <param name="baseName">基础名称</param>
        internal MacroFeatureIcon(IXImage icon, string baseName)
        {
            m_BaseName = baseName;
            m_Icon = icon;

            IconSizes = new IIconSpec[]
            {
                new IconSpec(m_Icon, MacroFeatureIconInfo.Size, 0, m_BaseName)
            };
        }

        /// <summary>图标尺寸规格数组</summary>
        public virtual IIconSpec[] IconSizes { get; }
    }

    /// <summary>
    /// 抑制状态的宏特性图标 - 自动将图标转换为灰度显示
    /// </summary>
    internal class MacroFeatureSuppressedIcon : MacroFeatureIcon
    {
        internal MacroFeatureSuppressedIcon(IXImage icon, string baseName) : base(icon, baseName)
        {
            IconSizes = new IIconSpec[]
            {
                new IconSpec(m_Icon, MacroFeatureIconInfo.Size, ConvertPixelToGrayscale, 0, m_BaseName)
            };
        }

        public override IIconSpec[] IconSizes { get; }

        /// <summary>将像素转换为灰度 - 用于创建抑制状态的图标视觉效果</summary>
        protected void ConvertPixelToGrayscale(ref byte r, ref byte g, ref byte b, ref byte a)
            => ColorUtils.ConvertPixelToGrayscale(ref r, ref g, ref b);
    }
}