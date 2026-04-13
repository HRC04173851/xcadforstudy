//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.Utils
{
    /// <summary>
    /// Numeric utilities
    /// 数值工具类
    /// </summary>
    public static class Numeric
    {
        /// <summary>
        /// Default tolerance for numbers
        /// 数值默认容差
        /// </summary>
        public const double DEFAULT_NUMERIC_TOLERANCE = 1E-12;

        /// <summary>
        /// Default tolerance for length
        /// 长度默认容差
        /// </summary>
        public const double DEFAULT_LENGTH_TOLERANCE = 1E-8;

        /// <summary>
        /// Default tolerance for angle
        /// 角度默认容差
        /// </summary>
        public const double DEFAULT_ANGLE_TOLERANCE = 1E-6;

        /// <summary>
        /// Compares two numbers with tolerance
        /// 在容差范围内比较两个数值
        /// </summary>
        /// <param name="d1">First number</param>
        /// <param name="d2">Second number</param>
        /// <param name="tol">Tolerance</param>
        /// <returns>True if equal</returns>
        public static bool Compare(double d1, double d2, double tol = DEFAULT_NUMERIC_TOLERANCE) => Math.Abs(d1 - d2) < tol;
    }
}
