// -*- coding: utf-8 -*-
// src/SolidWorks/Features/CustomFeature/Toolkit/Icons/MacroFeatureHighResIcon.cs

//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System.Collections.Generic;
using System.Drawing;
using Xarial.XCad.SolidWorks.Base;
using Xarial.XCad.Toolkit.Base;
using Xarial.XCad.UI;

namespace Xarial.XCad.SolidWorks.Features.CustomFeature.Toolkit.Icons
{
    /// <summary>
    /// 高分辨率宏特性图标
    /// <para>支持多种分辨率（小、中、大）以适应不同的显示需求</para>
    /// </summary>
    internal class MacroFeatureHighResIcon : MacroFeatureIcon
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="icon">图标图像</param>
        /// <param name="baseName">基础名称</param>
        internal MacroFeatureHighResIcon(IXImage icon, string baseName) : base(icon, baseName)
        {
            // 创建三种分辨率的图标规格
            IconSizes =  new IIconSpec[]
            {
                new IconSpec(m_Icon, MacroFeatureIconInfo.SizeHighResSmall, 0, m_BaseName),
                new IconSpec(m_Icon, MacroFeatureIconInfo.SizeHighResMedium, 0, m_BaseName),
                new IconSpec(m_Icon, MacroFeatureIconInfo.SizeHighResLarge, 0, m_BaseName)
            };
        }

        public override IIconSpec[] IconSizes { get; }
    }

    /// <summary>
    /// 高分辨率抑制状态图标
    /// <para>同时支持多种分辨率和灰度转换</para>
    /// </summary>
    internal class MacroFeatureSuppressedHighResIcon : MacroFeatureSuppressedIcon
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="icon">图标图像</param>
        /// <param name="baseName">基础名称</param>
        internal MacroFeatureSuppressedHighResIcon(IXImage icon, string baseName) : base(icon, baseName)
        {
            // 创建三种分辨率的灰度图标规格
            IconSizes = new IIconSpec[]
            {
                new IconSpec(m_Icon, MacroFeatureIconInfo.SizeHighResSmall, ConvertPixelToGrayscale, 0, m_BaseName),
                new IconSpec(m_Icon, MacroFeatureIconInfo.SizeHighResMedium, ConvertPixelToGrayscale, 0, m_BaseName),
                new IconSpec(m_Icon, MacroFeatureIconInfo.SizeHighResLarge, ConvertPixelToGrayscale, 0, m_BaseName)
            };
        }

        public override IIconSpec[] IconSizes { get; }
    }
}