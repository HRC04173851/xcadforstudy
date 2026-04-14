// -*- coding: utf-8 -*-
// src/SolidWorks/Features/CustomFeature/Toolkit/MacroFeatureIconInfo.cs

//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Drawing;
using System.IO;
using Xarial.XCad.SolidWorks.Base;
using Xarial.XCad.Toolkit.Base;

namespace Xarial.XCad.SolidWorks.Features.CustomFeature.Toolkit
{
    /// <summary>
    /// 宏特性图标信息工具类
    /// <para>提供图标路径、大小和格式的常量及获取方法</para>
    /// </summary>
    /// <remarks>
    /// 宏特性图标存储在公共应用程序数据目录下，
    /// 每个特性类型有独立的子文件夹。图标支持多种状态和分辨率。
    /// </remarks>
    internal static class MacroFeatureIconInfo
    {
        /// <summary>
        /// 默认图标文件夹路径模板
        /// </summary>
        /// <remarks>
        /// {0} 会被替换为类型的完整名称。
        /// 完整路径：%CommonApplicationData%\Xarial\xCad.Sw\{类型全名}\Icons\
        /// </remarks>
        internal const string DEFAULT_ICON_FOLDER = "Xarial\\xCad.Sw\\{0}\\Icons";

        /// <summary>
        /// 普通状态图标名称
        /// </summary>
        internal const string RegularName = "Regular";

        /// <summary>
        /// 抑制状态图标名称
        /// </summary>
        internal const string SuppressedName = "Suppressed";

        /// <summary>
        /// 高亮状态图标名称
        /// </summary>
        internal const string HighlightedName = "Highlighted";

        /// <summary>
        /// 图标图像格式
        /// </summary>
        internal static IconImageFormat_e Format { get; } = IconImageFormat_e.Bmp;

        /// <summary>
        /// 标准图标尺寸
        /// </summary>
        internal static Size Size { get; } = new Size(16, 18);

        /// <summary>
        /// 高分辨率小图标尺寸
        /// </summary>
        internal static Size SizeHighResSmall { get; } = new Size(20, 20);

        /// <summary>
        /// 高分辨率中图标尺寸
        /// </summary>
        internal static Size SizeHighResMedium { get; } = new Size(32, 32);

        /// <summary>
        /// 高分辨率大图标尺寸
        /// </summary>
        internal static Size SizeHighResLarge { get; } = new Size(40, 40);

        /// <summary>
        /// 获取宏特性图标的存储位置
        /// </summary>
        /// <param name="macroFeatType">宏特性类型</param>
        /// <returns>图标文件夹的完整路径</returns>
        /// <remarks>
        /// 默认情况下，每个特性类型有独立的图标文件夹。
        /// 路径格式：%CommonApplicationData%\Xarial\xCad.Sw\{类型全名}\Icons\
        /// </remarks>
        internal static string GetLocation(Type macroFeatType)
        {
            var iconFolderName = "";

            //macroFeatType.TryGetAttribute<FeatureIconAttribute>(a => iconFolderName = a.IconFolderName);

            // 如果没有指定自定义文件夹，使用默认路径
            if (string.IsNullOrEmpty(iconFolderName))
            {
                iconFolderName = string.Format(DEFAULT_ICON_FOLDER, macroFeatType.FullName);
            }

            // 组合完整路径
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                iconFolderName);
        }

        /// <summary>
        /// 获取宏特性图标文件路径数组
        /// </summary>
        /// <param name="macroFeatType">宏特性类型</param>
        /// <param name="highRes">是否包含高分辨率图标</param>
        /// <returns>图标文件路径数组</returns>
        /// <remarks>
        /// 返回的数组包含多种状态的图标：
        /// <list type="bullet">
        /// <item><description>普通状态（Regular）</description></item>
        /// <item><description>抑制状态（Suppressed）</description></item>
        /// <item><description>高亮状态（Highlighted）</description></item>
        /// </list>
        /// 如果 highRes 为 true，还会返回三种分辨率（小、中、大）的版本。
        /// </remarks>
        internal static string[] GetIcons(Type macroFeatType, bool highRes)
        {
            var loc = GetLocation(macroFeatType);

            if (highRes)
            {
                // 返回高分辨率图标（3种分辨率 x 3种状态 = 9个文件）
                return new string[]
                {
                    Path.Combine(loc, IconSpec.CreateFileName(RegularName, SizeHighResSmall, Format)),
                    Path.Combine(loc, IconSpec.CreateFileName(SuppressedName, SizeHighResSmall, Format)),
                    Path.Combine(loc, IconSpec.CreateFileName(HighlightedName, SizeHighResSmall, Format)),
                    Path.Combine(loc, IconSpec.CreateFileName(RegularName, SizeHighResMedium, Format)),
                    Path.Combine(loc, IconSpec.CreateFileName(SuppressedName, SizeHighResMedium, Format)),
                    Path.Combine(loc, IconSpec.CreateFileName(HighlightedName, SizeHighResMedium, Format)),
                    Path.Combine(loc, IconSpec.CreateFileName(RegularName, SizeHighResLarge, Format)),
                    Path.Combine(loc, IconSpec.CreateFileName(SuppressedName, SizeHighResLarge, Format)),
                    Path.Combine(loc, IconSpec.CreateFileName(HighlightedName, SizeHighResLarge, Format))
                };
            }
            else
            {
                // 返回标准分辨率图标（3个文件）
                return new string[]
                {
                    Path.Combine(loc, IconSpec.CreateFileName(RegularName, Size, Format)),
                    Path.Combine(loc, IconSpec.CreateFileName(SuppressedName, Size, Format)),
                    Path.Combine(loc, IconSpec.CreateFileName(HighlightedName, Size, Format))
                };
            }
        }
    }
}