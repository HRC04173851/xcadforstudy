// -*- coding: utf-8 -*-
// src/Toolkit/PageBuilder/Core/SpecialTypes.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件定义特殊伪类型类 SpecialTypes。
// 用于解析通用页面元素构造器的特殊类型标记。
// 包含 AnyType、ComplexType、EnumType 等类型标记。
//*********************************************************************

using System;
using System.Collections.Generic;

namespace Xarial.XCad.Utils.PageBuilder.Core
{
    /// <summary>
    /// Defines special pseudo-types used to resolve generic page element constructors.
    /// <para>定义用于解析通用页面元素构造器的特殊伪类型。</para>
    /// </summary>
    public static class SpecialTypes
    {
        /// <summary>
        /// Marker interface for special constructor categories.
        /// <para>特殊构造器分类的标记接口。</para>
        /// </summary>
        internal interface ISpecialType
        {
        }

        /// <summary>
        /// Special type representing any unmatched data type.
        /// <para>表示任意未匹配数据类型的特殊类型。</para>
        /// </summary>
        public class AnyType : ISpecialType
        {
        }

        /// <summary>
        /// Special type representing complex non-primitive data.
        /// <para>表示复杂非基础数据类型的特殊类型。</para>
        /// </summary>
        public class ComplexType : ISpecialType
        {
        }

        /// <summary>
        /// Special type representing enumeration data.
        /// <para>表示枚举数据类型的特殊类型。</para>
        /// </summary>
        public class EnumType : ISpecialType
        {
        }

        /// <summary>
        /// Finds matching special constructor categories for a data type.
        /// <para>查找与数据类型匹配的特殊构造器分类。</para>
        /// </summary>
        internal static IEnumerable<Type> FindMathingSpecialTypes(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (type.IsEnum)
            {
                yield return typeof(EnumType);
            }
            else if (IsComplexType(type))
            {
                yield return typeof(ComplexType);
            }
            else
            {
                yield return typeof(AnyType);
            }
        }

        /// <summary>
        /// Determines whether type should be treated as complex type.
        /// <para>判断类型是否应视为复杂类型。</para>
        /// </summary>
        private static bool IsComplexType(Type type)
        {
            return !(type.IsPrimitive
                || type.IsEnum
                || type == typeof(string)
                || type == typeof(decimal));
        }
    }
}