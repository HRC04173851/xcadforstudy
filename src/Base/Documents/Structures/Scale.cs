// -*- coding: utf-8 -*-
// src/Base/Documents/Structures/Scale.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 表示比例尺结构，包含分子和分母，并提供转换为小数值的扩展方法
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Xarial.XCad.Documents.Structures
{
    /// <summary>
    /// Represents the scale
    /// 表示比例尺
    /// </summary>
    [DebuggerDisplay("{" + nameof(Numerator) + "}" + ":{" + nameof(Denominator) + "}")]
    public class Scale
    {
        /// <summary>
        /// Numerator of this scale
        /// 比例尺分子
        /// </summary>
        public double Numerator { get; }

        /// <summary>
        /// Denominator of this scale
        /// 比例尺分母
        /// </summary>
        public double Denominator { get; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Scale(double numerator, double denominator)
        {
            Numerator = numerator;
            Denominator = denominator;
        }
    }

    /// <summary>
    /// Extensions of <see cref="Scale"/>
    /// <see cref="Scale"/> 的扩展方法
    /// </summary>
    public static class ScaleExtension 
    {
        /// <summary>
        /// Returns the scale as double value
        /// </summary>
        /// <param name="scale">Scale</param>
        /// <returns>Scale</returns>
        public static double AsDouble(this Scale scale)
            => scale.Numerator / scale.Denominator;
    }
}
