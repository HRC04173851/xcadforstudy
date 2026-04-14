// -*- coding: utf-8 -*-
// src/Toolkit/Data/LocalTagsManager.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件实现本地标签管理器 LocalTagsManager。
// 在对象实例本地范围内管理标签，使用字典存储键值对。
// 提供标签的Contains、Get、Pop、Put等操作，支持自定义比较器。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xarial.XCad.Data;

namespace Xarial.XCad.Toolkit.Data
{
    /// <summary>
    /// Manages tags locally on the object
    /// <para>在对象实例本地范围内管理标签。</para>
    /// </summary>
    public class LocalTagsManager : ITagsManager
    {
        private readonly Dictionary<string, object> m_Tags;

        /// <summary>
        /// Initializes local tags manager with case-insensitive comparer.
        /// <para>使用不区分大小写比较器初始化本地标签管理器。</para>
        /// </summary>
        public LocalTagsManager()
            : this(StringComparer.CurrentCultureIgnoreCase)
        {
        }

        /// <summary>
        /// Initializes local tags manager with custom comparer.
        /// <para>使用自定义比较器初始化本地标签管理器。</para>
        /// </summary>
        public LocalTagsManager(StringComparer comparer)
        {
            m_Tags = new Dictionary<string, object>(comparer);
        }

        /// <summary>
        /// Indicates whether no tags are stored.
        /// <para>指示当前是否没有存储任何标签。</para>
        /// </summary>
        public bool IsEmpty => !m_Tags.Any();

        public bool Contains(string name) => m_Tags.ContainsKey(name);

        public T Get<T>(string name)
        {
            if (m_Tags.TryGetValue(name, out object val))
            {
                return (T)val;
            }
            else
            {
                throw new KeyNotFoundException("Specified tag is not registered");
            }
        }

        /// <summary>
        /// Gets and removes tag value by name.
        /// <para>按名称读取并移除标签值。</para>
        /// </summary>
        public T Pop<T>(string name)
        {
            var val = Get<T>(name);
            m_Tags.Remove(name);
            return val;
        }

        /// <summary>
        /// Adds or updates tag value.
        /// <para>添加或更新标签值。</para>
        /// </summary>
        public void Put<T>(string name, T value)
        {
            m_Tags[name] = value;
        }
    }
}
