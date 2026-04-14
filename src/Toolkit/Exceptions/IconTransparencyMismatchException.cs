// -*- coding: utf-8 -*-
// src/Toolkit/Exceptions/IconTransparencyMismatchException.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件实现异常类 IconTransparencyMismatchException。
// 当图标组中部分图标的透明键与组内其它图标不一致时抛出。
// 确保图标组在渲染时使用统一的透明处理方式。
//*********************************************************************

using System;

namespace Xarial.XCad.Toolkit.Exceptions
{
    /// <summary>
    /// Exception indicates that the transparency key <see cref="Base.IIcon.TransparencyKey"/> is different for
    /// some icons in the icons group passed to <see cref="Utils.IconsConverter.ConvertIconsGroup(Base.IIcon[])"/>
    /// <para>异常指示传入图标组中部分图标的透明键（Transparency Key）与组内其它图标不一致。</para>
    /// </summary>
    public class IconTransparencyMismatchException : InvalidOperationException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IconTransparencyMismatchException"/> class.
        /// <para>初始化 <see cref="IconTransparencyMismatchException"/> 类的新实例。</para>
        /// </summary>
        /// <param name="index">Index of icon with mismatched transparency.<para>透明键不匹配的图标索引。</para></param>
        public IconTransparencyMismatchException(int index)
            : base($"Transparency color of icon at index {index} doesn't match the group transparency")
        {
        }
    }
}