// -*- coding: utf-8 -*-
// src/Toolkit/PageBuilder/Core/AttributeSet.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件实现页面元素特性集合类 AttributeSet。
// 存储从模型元数据解析得到的页面元素特性。
// 提供特性的添加、获取和类型检查功能。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xarial.XCad.UI.PropertyPage.Base;
using Xarial.XCad.Utils.PageBuilder.Base;
using Xarial.XCad.Utils.PageBuilder.Binders;

namespace Xarial.XCad.Utils.PageBuilder.Core
{
    /// <summary>
    /// Stores page-element attributes resolved from model metadata.
    /// <para>存储从模型元数据解析得到的页面元素特性集合。</para>
    /// </summary>
    public class AttributeSet : IAttributeSet
    {
        private readonly Dictionary<Type, List<IAttribute>> m_Attributes;

        public IControlDescriptor ControlDescriptor { get; }
        public Type ContextType { get; }
        public string Description { get; }
        public int Id { get; }
        public string Name { get; }
        public object Tag { get; }

        /// <summary>
        /// Initializes a new attribute set instance.
        /// <para>初始化新的特性集合实例。</para>
        /// </summary>
        internal AttributeSet(int ctrlId, string ctrlName, string desc, Type contextType, object tag, IControlDescriptor ctrlDesc = null)
        {
            Id = ctrlId;
            Name = ctrlName;
            Description = desc;
            ContextType = contextType;
            ControlDescriptor = ctrlDesc;
            Tag = tag;

            m_Attributes = new Dictionary<Type, List<IAttribute>>();
        }

        /// <summary>
        /// Adds attribute instance to the collection.
        /// <para>向集合添加特性实例。</para>
        /// </summary>
        public void Add<TAtt>(TAtt att) where TAtt : IAttribute
        {
            if (att == null)
            {
                throw new ArgumentNullException(nameof(att));
            }

            List<IAttribute> atts;

            if (!m_Attributes.TryGetValue(att.GetType(), out atts))
            {
                atts = new List<IAttribute>();
                m_Attributes.Add(att.GetType(), atts);
            }

            atts.Add(att);
        }

        /// <summary>
        /// Returns first attribute matching requested type.
        /// <para>返回与请求类型匹配的首个特性。</para>
        /// </summary>
        public TAtt Get<TAtt>()
            where TAtt : IAttribute
        {
            return GetAll<TAtt>().First();
        }

        /// <summary>
        /// Returns all attributes assignable to requested type.
        /// <para>返回可赋值为请求类型的全部特性。</para>
        /// </summary>
        public IEnumerable<TAtt> GetAll<TAtt>()
            where TAtt : IAttribute
        {
            var atts = new List<IAttribute>();

            foreach (var attGrp in m_Attributes.Where(
                a => typeof(TAtt).IsAssignableFrom(a.Key)))
            {
                atts.AddRange(attGrp.Value);
            }

            if (atts.Any())
            {
                return atts.Cast<TAtt>();
            }
            else
            {
                //throw exception
                throw new Exception();
            }
        }

        /// <summary>
        /// Checks whether set contains attribute type.
        /// <para>检查集合中是否包含指定特性类型。</para>
        /// </summary>
        public bool Has<TAtt>()
            where TAtt : IAttribute
        {
            return m_Attributes.Keys.Any(
                t => typeof(TAtt).IsAssignableFrom(t));
        }
    }
}