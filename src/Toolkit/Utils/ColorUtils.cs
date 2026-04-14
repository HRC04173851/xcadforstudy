// -*- coding: utf-8 -*-
// src/Toolkit/Utils/ColorUtils.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件实现 Color 与 Win32 颜色值之间转换的工具类 ColorUtils。
// 支持 COLORREF 格式的转换和灰度值计算。
// 用于 CAD 图形渲染中的颜色处理。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Xarial.XCad.Toolkit.Utils
{
    /// <summary>
    /// Utility to convert between the .NET Color and Win32 color
    /// <para>用于在 .NET `Color` 与 Win32 颜色值之间转换。</para>
    /// </summary>
    public static class ColorUtils
    {
        /// <summary>
        /// Creates a Win32 color
        /// <para>生成 Win32 颜色值（COLORREF）。</para>
        /// </summary>
        /// <param name="color">Input color<para>输入颜色。</para></param>
        /// <returns>Wind32 color<para>返回 Win32 颜色值。</para></returns>
        public static int ToColorRef(Color color)
            => (color.R << 0) | (color.G << 8) | (color.B << 16);

        /// <summary>
        /// Converts Win32 color to .NET color
        /// <para>将 Win32 颜色值转换为 .NET 颜色。</para>
        /// </summary>
        /// <param name="colorRef">Input color<para>输入 Win32 颜色值。</para></param>
        /// <returns>Converted color<para>转换后的 .NET 颜色。</para></returns>
        public static Color FromColorRef(int colorRef) 
        {
            int r = colorRef & 0x000000FF;
            int g = (colorRef & 0x0000FF00) >> 8;
            int b = (colorRef & 0x00FF0000) >> 16;

            return Color.FromArgb(r, g, b);
        }

        /// <summary>
        /// Converts the pix to grayscale
        /// <para>将像素转换为灰度值。</para>
        /// </summary>
        /// <param name="r">Red component of the pixel<para>像素红色分量。</para></param>
        /// <param name="g">Green component of the pixex<para>像素绿色分量。</para></param>
        /// <param name="b">Blue component of the pixel<para>像素蓝色分量。</para></param>
        public static void ConvertPixelToGrayscale(ref byte r, ref byte g, ref byte b)
        {
            var pixel = (byte)((r + g + b) / 3);

            r = pixel;
            g = pixel;
            b = pixel;
        }
    }
}
