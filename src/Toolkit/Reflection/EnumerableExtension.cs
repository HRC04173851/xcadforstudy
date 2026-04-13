//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System.Collections.Generic;
using System.Linq;

namespace Xarial.XCad.Utils.Reflection
{
    /// <summary>
    /// Provides helper extensions for enumerable sequences.
    /// <para>为可枚举序列提供辅助扩展方法。</para>
    /// </summary>
    public static class EnumerableExtension
    {
        /// <summary>
        /// Returns original sequence or an empty sequence when source is null.
        /// <para>当源序列为 `null` 时返回空序列，否则返回原始序列。</para>
        /// </summary>
        public static IEnumerable<T> ValueOrEmpty<T>(this IEnumerable<T> enumer)
        {
            if (enumer != null)
            {
                return enumer;
            }
            else
            {
                return Enumerable.Empty<T>();
            }
        }
    }
}