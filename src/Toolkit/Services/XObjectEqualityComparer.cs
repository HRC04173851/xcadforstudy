// -*- coding: utf-8 -*-
// src/Toolkit/Services/XObjectEqualityComparer.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件实现 IXObject 的泛型相等性比较器 XObjectEqualityComparer。
// 比较两个 xCAD 对象的语义相等性，支持在字典和集合中使用。
// 用于 xCAD 对象的标识和查找操作。
//*********************************************************************

using System.Collections.Generic;

namespace Xarial.XCad.Services
{
    /// <summary>
    /// Represents the generic equality of the <see cref="IXObject"/>
    /// <para>提供 <see cref="IXObject"/> 的泛型相等性比较实现。</para>
    /// </summary>
    /// <typeparam name="TObj">Specific type of <see cref="IXObject"/><para><see cref="IXObject"/> 的具体类型。</para></typeparam>
    public class XObjectEqualityComparer<TObj> : IEqualityComparer<TObj>
        where TObj : IXObject
    {
        /// <summary>
        /// Compares two xCAD objects for semantic equality.
        /// <para>比较两个 xCAD 对象的语义相等性。</para>
        /// </summary>
        public bool Equals(TObj x, TObj y)
        {
            if (object.ReferenceEquals(x, y))
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            return x.Equals(y);
        }

        /// <summary>
        /// Returns hash code placeholder for comparer usage in maps.
        /// <para>返回用于映射比较器的哈希占位值。</para>
        /// </summary>
        public int GetHashCode(TObj obj) => 0;
    }
}