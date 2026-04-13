//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Linq;
using System.Reflection;

namespace Xarial.XCad.Utils.Reflection
{
    /// <summary>
    /// Provides extension methods for <see cref="Assembly"/>.
    /// <para>为 <see cref="Assembly"/> 提供扩展方法。</para>
    /// </summary>
    public static class AssemblyExtension
    {
        /// <summary>
        /// Tries to get attribute from the assembly
        /// <para>尝试从程序集获取指定特性。</para>
        /// </summary>
        /// <typeparam name="TAtt">Type of attribute to get<para>要获取的特性类型。</para></typeparam>
        /// <param name="assm">Assembly<para>目标程序集。</para></param>
        /// <param name="attProc">Action to process attribute<para>用于处理特性的回调操作。</para></param>
        /// <returns>True if attribute exists<para>如果特性存在则返回 `true`。</para></returns>
        public static bool TryGetAttribute<TAtt>(this Assembly assm, Action<TAtt> attProc)
            where TAtt : Attribute
        {
            var atts = assm.GetCustomAttributes(typeof(TAtt), true);

            if (atts != null && atts.Any())
            {
                var att = atts.First() as TAtt;
                attProc?.Invoke(att);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}