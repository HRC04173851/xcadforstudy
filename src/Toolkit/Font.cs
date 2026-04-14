// -*- coding: utf-8 -*-
// src/Toolkit/Font.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件实现 xCAD 字体描述符的默认实现类 Font。
// 包含字体名称、尺寸（文档单位和磅值）、样式等属性。
// 用于在 CAD 文档中创建和管理字体样式。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xarial.XCad.Enums;

namespace Xarial.XCad.Toolkit
{
    /// <summary>
    /// Default implementation of xCAD font descriptor.
    /// <para>xCAD 字体描述对象的默认实现。</para>
    /// </summary>
    public class Font : IFont
    {
        /// <summary>
        /// Font family name.
        /// <para>字体名称。</para>
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Font size in model/document units.
        /// <para>以模型或文档单位表示的字体大小。</para>
        /// </summary>
        public double? Size { get; }
        /// <summary>
        /// Font size in points.
        /// <para>以磅值（pt）表示的字体大小。</para>
        /// </summary>
        public double? SizeInPoints { get; }
        /// <summary>
        /// Font style flags.
        /// <para>字体样式标志。</para>
        /// </summary>
        public FontStyle_e Style { get; }

        /// <summary>
        /// Initializes font descriptor.
        /// <para>初始化字体描述对象。</para>
        /// </summary>
        public Font(string name, double? size, double? sizeInPoints, FontStyle_e style)
        {
            Name = name;
            Size = size;
            SizeInPoints = sizeInPoints;
            Style = style;
        }
    }
}
