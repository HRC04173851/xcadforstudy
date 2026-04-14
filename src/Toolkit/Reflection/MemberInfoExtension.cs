// -*- coding: utf-8 -*-
// src/Toolkit/Reflection/MemberInfoExtension.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件为 MemberInfo 提供扩展方法。
// 尝试从类成员（字段或属性）获取指定特性。
// 用于反射基础的元数据查询。
//*********************************************************************

using System;
using System.Linq;
using System.Reflection;

namespace Xarial.XCad.Utils.Reflection
{
    /// <summary>
    /// Provides extension methods for the <see cref="MemberInfo"/>
    /// <para>为 <see cref="MemberInfo"/> 提供扩展方法。</para>
    /// </summary>
    public static class MemberInfoExtension
    {
        /// <summary>
        /// Attempts to get the attribute from the class member
        /// <para>尝试从类成员获取指定特性。</para>
        /// </summary>
        /// <typeparam name="TAtt">Attribute type<para>特性类型。</para></typeparam>
        /// <param name="membInfo">Pointer to member (field or property)<para>成员信息对象（字段或属性）。</para></param>
        /// <returns>Pointer to attribute or null if not found<para>如果找到则返回特性，否则返回 `null`。</para></returns>
        public static TAtt TryGetAttribute<TAtt>(this MemberInfo membInfo)
            where TAtt : Attribute
        {
            var atts = membInfo.GetCustomAttributes(typeof(TAtt), true);

            if (atts != null && atts.Any())
            {
                return atts.First() as TAtt;
            }
            else
            {
                return null;
            }
        }
    }
}