// -*- coding: utf-8 -*-
// src/Toolkit/Services/IIconsCreator.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件定义图标创建器接口 IIconsCreator。
// 根据图标定义创建图像资源，支持单个图标和图标组转换。
// 是图像资源生成服务的基础接口。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Toolkit.Base;

namespace Xarial.XCad.Toolkit.Services
{
    /// <summary>
    /// Creates images from icons
    /// <para>根据图标定义创建图像资源。</para>
    /// </summary>
    public interface IIconsCreator : IDisposable
    {
        /// <summary>
        /// Creates image from the icon in all sizes
        /// <para>根据单个图标创建其所有尺寸的图像文件。</para>
        /// </summary>
        /// <param name="icon">Icon<para>图标定义对象。</para></param>
        /// <param name="folder">Custom folder, if empty - default folder is used<para>自定义输出目录；为空时使用默认目录。</para></param>
        /// <returns>Paths to icons of all sizes<para>返回各尺寸图标文件路径集合。</para></returns>
        IImageCollection ConvertIcon(IIcon icon, string folder = "");

        /// <summary>
        /// Creates group of images from the input icons
        /// <para>将输入图标集合合成为一组图像资源。</para>
        /// </summary>
        /// <param name="icons">Icons to group<para>要组合处理的图标集合。</para></param>
        ///<inheritdoc/>
        IImageCollection ConvertIconsGroup(IIcon[] icons, string folder = "");
    }
}
