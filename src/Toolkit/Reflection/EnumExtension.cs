//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Xarial.XCad.Reflection;

namespace Xarial.XCad.Utils.Reflection
{
    /// <summary>
    /// Provides extension classes for the <see cref="Enum"/> enumerator
    /// <para>为 <see cref="Enum"/> 枚举值提供扩展方法。</para>
    /// </summary>
    public static class EnumExtension
    {
        /// <summary>
        /// Get the specified attribute from the enumerator field
        /// <para>从枚举字段获取指定特性。</para>
        /// </summary>
        /// <typeparam name="TAtt">Attribute type<para>特性类型。</para></typeparam>
        /// <param name="enumer">Enumerator field<para>枚举字段值。</para></param>
        /// <returns>Attribute<para>返回找到的特性实例。</para></returns>
        /// <exception cref="NullReferenceException"/>
        /// <remarks>This method throws an exception if attribute is missing<para>如果缺少该特性，此方法会抛出异常。</para></remarks>
        public static TAtt GetAttribute<TAtt>(this Enum enumer)
            where TAtt : Attribute
        {
            TAtt att = default;

            if (!enumer.TryGetAttribute<TAtt>(a => att = a))
            {
                throw new NullReferenceException($"Attribute of type {typeof(TAtt)} is not fond on {enumer}");
            }

            return att;
        }

        /// <summary>
        /// Attempts to the attribute from enumeration
        /// <para>尝试从枚举值获取特性。</para>
        /// </summary>
        /// <typeparam name="TAtt">Type of the attribute<para>特性类型。</para></typeparam>
        /// <param name="enumer">Enumeration value<para>要读取特性的枚举值。</para></param>
        /// <returns>Attribute or null if not found<para>如果找到则返回特性，否则返回 `null`。</para></returns>
        public static TAtt TryGetAttribute<TAtt>(this Enum enumer)
            where TAtt : Attribute
        {
            TAtt thisAtt = null;
            enumer.TryGetAttribute<TAtt>(a => thisAtt = a);
            return thisAtt;
        }
    }
}