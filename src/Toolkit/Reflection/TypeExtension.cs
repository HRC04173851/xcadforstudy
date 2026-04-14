// -*- coding: utf-8 -*-
// src/Toolkit/Reflection/TypeExtension.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件为 Type 提供扩展方法。
// 从类型、其所有父类型以及接口中获取特性，支持泛型类型解析。
// 实现 COM ProgId 获取和可见性检查等工具方法。
//*********************************************************************

using System;
using System.Runtime.InteropServices;
using Xarial.XCad.Reflection;

namespace Xarial.XCad.Utils.Reflection
{
    /// <summary>
    /// Provides the extension methods for <see cref="Type"/>
    /// <para>为 <see cref="Type"/> 提供扩展方法。</para>
    /// </summary>
    public static class TypeExtension
    {
        /// <summary>
        /// Get the specified attribute from the type, all parent types and interfaces
        /// <para>从类型、其所有父类型以及接口中获取指定特性。</para>
        /// </summary>
        /// <typeparam name="TAtt">Attribute type<para>特性类型。</para></typeparam>
        /// <param name="type">Type<para>目标类型。</para></param>
        /// <returns>Attribute<para>返回找到的特性实例。</para></returns>
        /// <exception cref="NullReferenceException"/>
        /// <remarks>This method throws an exception if attribute is missing<para>如果缺少该特性，此方法会抛出异常。</para></remarks>
        public static TAtt GetAttribute<TAtt>(this Type type)
                    where TAtt : Attribute
        {
            TAtt att = default(TAtt);

            if (!type.TryGetAttribute<TAtt>(a => att = a))
            {
                throw new NullReferenceException($"Attribute of type {typeof(TAtt)} is not fond on {type.FullName}");
            }

            return att;
        }

        /// <summary>
        /// Attempts to the attribute from type, all parent types and interfaces
        /// <para>尝试从类型、其所有父类型以及接口中获取特性。</para>
        /// </summary>
        /// <typeparam name="TAtt">Type of the attribute<para>特性类型。</para></typeparam>
        /// <param name="type">Type to get attribute from<para>要读取特性的类型。</para></param>
        /// <returns>Attribute or null if not found<para>如果找到则返回特性，否则返回 `null`。</para></returns>
        public static TAtt TryGetAttribute<TAtt>(this Type type)
            where TAtt : Attribute
        {
            TAtt thisAtt = null;
            type.TryGetAttribute<TAtt>(a => thisAtt = a);
            return thisAtt;
        }

        /// <summary>
        /// Attempts to get the attribute from the type, all parent types and interfaces
        /// <para>尝试从类型、其所有父类型以及接口中获取特性，并通过输出参数返回。</para>
        /// </summary>
        /// <typeparam name="TAtt">Type of the attribute<para>特性类型。</para></typeparam>
        /// <param name="type">Type to get attribute from<para>要读取特性的类型。</para></param>
        /// <param name="att">Attribute of the type<para>输出找到的特性实例。</para></param>
        /// <returns>True if attribute exists<para>如果存在该特性则返回 `true`。</para></returns>
        public static bool TryGetAttribute<TAtt>(this Type type, out TAtt att)
            where TAtt : Attribute
        {
            TAtt thisAtt = null;
            var res = type.TryGetAttribute<TAtt>(a => thisAtt = a);
            att = thisAtt;
            return res;
        }

        /// <summary>
        /// Checks if this type can be assigned to the generic type
        /// <para>检查当前类型是否可赋值给指定泛型类型。</para>
        /// </summary>
        /// <param name="thisType">Type<para>当前类型。</para></param>
        /// <param name="genericType">Base generic type (i.e. MyGenericType&lt;&gt;)<para>基础泛型类型（如 `MyGenericType&lt;&gt;`）。</para></param>
        /// <returns>True if type is assignable to generic<para>如果类型可赋值给该泛型则返回 `true`。</para></returns>
        public static bool IsAssignableToGenericType(this Type thisType, Type genericType)
        {
            return thisType.TryFindGenericType(genericType) != null;
        }

        /// <summary>
        /// Gets the specific arguments of this type in relation to specified generic type
        /// <para>获取当前类型相对于指定泛型类型的具体泛型参数。</para>
        /// </summary>
        /// <param name="thisType">This type which must be assignable to the specified genericType<para>必须可赋值给指定泛型类型的当前类型。</para></param>
        /// <param name="genericType">Generic type<para>目标泛型类型。</para></param>
        /// <returns>Arguments<para>返回具体泛型参数数组。</para></returns>
        /// <remarks>For example this method called on List&lt;string&gt; where the genericType is IEnumerable&lt;&gt; would return string<para>例如，对 `List&lt;string&gt;` 调用本方法并指定 `IEnumerable&lt;&gt;` 时，将返回 `string`。</para></remarks>
        public static Type[] GetArgumentsOfGenericType(this Type thisType, Type genericType)
        {
            var type = thisType.TryFindGenericType(genericType);

            if (type != null)
            {
                return type.GetGenericArguments();
            }
            else
            {
                return Type.EmptyTypes;
            }
        }

        /// <summary>
        /// Finds the specific generic type to a specified base generic type
        /// <para>查找与指定基础泛型类型对应的具体泛型类型。</para>
        /// </summary>
        /// <param name="thisType">This type<para>当前类型。</para></param>
        /// <param name="genericType">Base generic type<para>基础泛型类型。</para></param>
        /// <returns>Specific generic type or null if not found<para>如果找到则返回具体泛型类型，否则返回 `null`。</para></returns>
        public static Type TryFindGenericType(this Type thisType, Type genericType)
        {
            var interfaceTypes = thisType.GetInterfaces();

            Predicate<Type> canCastFunc = (t) => t.IsGenericType && t.GetGenericTypeDefinition() == genericType;

            foreach (var it in interfaceTypes)
            {
                if (canCastFunc(it))
                {
                    return it;
                }
            }

            if (canCastFunc(thisType))
            {
                return thisType;
            }

            var baseType = thisType.BaseType;

            if (baseType != null)
            {
                return baseType.TryFindGenericType(genericType);
            }

            return null;
        }

        /// <summary>
        /// Returns the COM ProgId of a type
        /// <para>返回类型的 COM ProgId。</para>
        /// </summary>
        /// <param name="type">Input type<para>输入类型。</para></param>
        /// <returns>COM Prog id<para>COM ProgId 字符串。</para></returns>
        public static string GetProgId(this Type type)
        {
            string progId = "";

            if (!type.TryGetAttribute<ProgIdAttribute>(a => progId = a.Value))
            {
                progId = type.FullName;
            }

            return progId;
        }

        /// <summary>
        /// Identifies if type is COM visible
        /// <para>判断类型是否对 COM 可见。</para>
        /// </summary>
        /// <param name="type">Type to check<para>要检查的类型。</para></param>
        /// <returns>True if type is COM visible<para>如果类型对 COM 可见则返回 `true`。</para></returns>
        public static bool IsComVisible(this Type type)
        {
            bool isComVisible = false;

            var comVisAtt = Attribute.GetCustomAttribute(type, typeof(ComVisibleAttribute), false) as ComVisibleAttribute;

            if (comVisAtt != null)
            {
                isComVisible = comVisAtt.Value;
            }
            else
            {
                type.Assembly.TryGetAttribute<ComVisibleAttribute>(a => isComVisible = a.Value);
            }

            return isComVisible;
        }
    }
}