// -*- coding: utf-8 -*-
// src/Base/IFont.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 字体接口，定义字体的名称、大小（米或点）、样式等属性，支持不同的字体表示方式。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xarial.XCad.Enums;

namespace Xarial.XCad
{
    /// <summary>
    /// Font
    /// 字体
    /// </summary>
    public interface IFont
    {
        /// <summary>
        /// Face name of the font
        /// 字体面孔名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Size of the font in meters if <see cref="SizeInPoints"/> is null
        /// 字体大小（米），当 <see cref="SizeInPoints"/> 为 null 时使用
        /// </summary>
        double? Size { get; }

        /// <summary>
        /// Size of the font in points if <see cref="Size"/> is null
        /// 字体大小（点），当 <see cref="Size"/> 为 null 时使用
        /// </summary>
        double? SizeInPoints { get; }

        /// <summary>
        /// Font style
        /// 字体样式
        /// </summary>
        FontStyle_e Style { get; }
    }
}
