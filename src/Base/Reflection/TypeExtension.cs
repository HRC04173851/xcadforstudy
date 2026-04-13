//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Linq;

namespace Xarial.XCad.Reflection
{
    /// <summary>
    /// Additional methods for <see cref="Type"/>
    /// <see cref="Type"/> 的附加扩展方法
    /// </summary>
    public static class TypeExtension
    {
        /// <summary>
        /// Attempts to get the attribute from the type, all parent types and interfaces
        /// 尝试从类型、所有父类型及接口中获取特性
        /// </summary>
        /// <typeparam name="TAtt">Type of the attribute</typeparam>
        /// <param name="type">Type to get attribute from</param>
        /// <param name="attProc">Handler to process the attribute</param>
        /// <returns>True if attribute exists</returns>
        public static bool TryGetAttribute<TAtt>(this Type type, Action<TAtt> attProc)
            where TAtt : Attribute
        {
            var atts = type.GetCustomAttributes(typeof(TAtt), true).
                Union(type.GetInterfaces().
                SelectMany(interfaceType => interfaceType.GetCustomAttributes(typeof(TAtt), true))).
                Distinct();

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