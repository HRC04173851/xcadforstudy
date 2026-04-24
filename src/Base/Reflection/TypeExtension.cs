// -*- coding: utf-8 -*-
// src/Base/Reflection/TypeExtension.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 提供Type类型的扩展方法，支持从类型、所有父类型及接口中递归获取特性（Attribute），
// 便于在反射时查找继承层次中的属性信息。
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
            // 获取类型本身及其所有继承接口上的指定特性
            var atts = type.GetCustomAttributes(typeof(TAtt), true).
                Union(type.GetInterfaces().
                SelectMany(interfaceType => interfaceType.GetCustomAttributes(typeof(TAtt), true))).
                Distinct();

            if (atts != null && atts.Any())
            {
                // 找到第一个匹配的特性并通过回调处理
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