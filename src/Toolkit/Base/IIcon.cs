// -*- coding: utf-8 -*-
// src/Toolkit/Base/IIcon.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件定义图标接口 IIcon，包含图标格式、尺寸和透明键等属性。
// 用于标准化图标对象的创建和属性访问，支持永久/临时图标管理。
//*********************************************************************

using System.Collections.Generic;
using System.Drawing;

namespace Xarial.XCad.Toolkit.Base
{
    /// <summary>
    /// Format of the image icon
    /// <para>图像图标的格式。</para>
    /// </summary>
    public enum IconImageFormat_e
    {
        /// <summary>
        /// .bmp
        /// <para>位图格式（.bmp）。</para>
        /// </summary>
        Bmp,

        /// <summary>
        /// .png
        /// <para>便携式网络图形格式（.png）。</para>
        /// </summary>
        Png,

        /// <summary>
        /// .jpeg
        /// <para>联合图片专家组格式（.jpeg）。</para>
        /// </summary>
        Jpeg
    }

    /// <summary>
    /// Represents the specific icon descriptor
    /// <para>表示特定的图标描述符。</para>
    /// </summary>
    public interface IIcon
    {
        /// <summary>
        /// Indicates that this icon is permanent and should not be removed on dispose
        /// <para>指示此图标是永久的，在释放（Dispose）时不应被移除。</para>
        /// </summary>
        bool IsPermanent { get; }

        /// <summary>
        /// Transparency key to be applied to transparent color
        /// <para>应用于透明颜色的透明键（即表示透明色的掩码颜色）。</para>
        /// </summary>
        Color TransparencyKey { get; }

        /// <summary>
        /// List of required icon sizes
        /// <para>所需图标大小（尺寸）的列表。</para>
        /// </summary>
        /// <returns>图标规范（IIconSpec）数组。<para>返回图标尺寸规范的数组。</para></returns>
        IIconSpec[] IconSizes { get; }

        /// <summary>
        /// Image format
        /// <para>图像格式。</para>
        /// </summary>
        IconImageFormat_e Format { get; }
    }
}