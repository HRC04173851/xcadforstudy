// -*- coding: utf-8 -*-
// src/Toolkit/PageBuilder/Internal/ConstructorsContainer.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件实现页面构建器控件元素构造器的解析容器 ConstructorsContainer。
// 根据专用/默认/特殊类型规则查找并调用合适的构造器。
// 管理多种类型对应的构造器注册和查找逻辑。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using Xarial.XCad.UI.PropertyPage.Base;
using Xarial.XCad.Utils.PageBuilder.Attributes;
using Xarial.XCad.Utils.PageBuilder.Base;
using Xarial.XCad.Utils.PageBuilder.Core;
using Xarial.XCad.Utils.PageBuilder.Exceptions;

namespace Xarial.XCad.Utils.PageBuilder.Internal
{
    /// <summary>
    /// Resolves element constructors for page builder controls.
    /// <para>用于解析页面构建器控件元素构造器的容器。</para>
    /// </summary>
    /// <typeparam name="TPage">Page type.<para>页面类型。</para></typeparam>
    /// <typeparam name="TGroup">Group type.<para>分组类型。</para></typeparam>
    internal class ConstructorsContainer<TPage, TGroup>
        where TPage : IPage
        where TGroup : IGroup
    {
        /// <summary>
        /// Constructors for the default data types (i.e. int, double, bool etc.)
        /// <para>默认数据类型（如 int、double、bool 等）对应的构造器。</para>
        /// </summary>
        private readonly Dictionary<Type, IPageElementConstructor> m_DefaultConstructors;

        /// <summary>
        /// Constructors for the special types (i.e. complex, enums, etc.)
        /// <para>特殊类型（如复合类型、枚举等）对应的构造器。</para>
        /// </summary>
        private readonly Dictionary<Type, IPageElementConstructor> m_SpecialTypeConstructors;

        /// <summary>
        /// Specific constructor for specific data types
        /// <para>针对特定数据类型的专用构造器。</para>
        /// </summary>
        private readonly Dictionary<Type, IPageElementConstructor> m_SpecificConstructors;

        /// <summary>
        /// Initializes constructor container with available element constructors.
        /// <para>使用可用元素构造器初始化构造器容器。</para>
        /// </summary>
        internal ConstructorsContainer(params IPageElementConstructor[] constructors)
        {
            m_DefaultConstructors = new Dictionary<Type, IPageElementConstructor>();
            m_SpecificConstructors = new Dictionary<Type, IPageElementConstructor>();
            m_SpecialTypeConstructors = new Dictionary<Type, IPageElementConstructor>();

            foreach (var constr in constructors)
            {
                var dataTypeAtts = constr.GetType().GetCustomAttributes(
                    typeof(DefaultTypeAttribute), true).OfType<DefaultTypeAttribute>();

                var isDefaultConstr = dataTypeAtts.Any();

                if (isDefaultConstr)
                {
                    foreach (var dataTypeAtt in dataTypeAtts)
                    {
                        var type = dataTypeAtt.Type;

                        if (typeof(SpecialTypes.ISpecialType).IsAssignableFrom(type))
                        {
                            if (!m_SpecialTypeConstructors.ContainsKey(type))
                            {
                                m_SpecialTypeConstructors.Add(type, constr);
                            }
                            else
                            {
                                throw new OverdefinedConstructorException(constr.GetType(), type);
                            }
                        }
                        else
                        {
                            if (!m_DefaultConstructors.ContainsKey(dataTypeAtt.Type))
                            {
                                m_DefaultConstructors.Add(dataTypeAtt.Type, constr);
                            }
                            else
                            {
                                throw new OverdefinedConstructorException(constr.GetType(), dataTypeAtt.Type);
                            }
                        }
                    }
                }
                else
                {
                    if (!m_SpecificConstructors.ContainsKey(constr.GetType()))
                    {
                        m_SpecificConstructors.Add(constr.GetType(), constr);
                    }
                    else
                    {
                        throw new OverdefinedConstructorException(constr.GetType(), constr.GetType());
                    }
                }
            }
        }

        /// <summary>
        /// Creates element control by resolving best matching constructor.
        /// <para>通过解析最佳匹配构造器创建元素控件。</para>
        /// </summary>
        internal IControl CreateElement(Type type, IGroup parent, IAttributeSet atts, IMetadata[] metadata, ref int numberOfUsedIds)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (atts == null)
            {
                throw new ArgumentNullException(nameof(atts));
            }

            if (parent == null)
            {
                throw new ArgumentNullException(nameof(parent));
            }

            var constr = FindConstructor(type, atts);

            return constr.Create(parent, atts, metadata, ref numberOfUsedIds);
        }

        /// <summary>
        /// Finds constructor according to specific/default/special type rules.
        /// <para>按专用/默认/特殊类型规则查找构造器。</para>
        /// </summary>
        private IPageElementConstructor FindConstructor(Type type, IAttributeSet atts)
        {
            if (atts == null)
            {
                throw new ArgumentNullException(nameof(atts));
            }

            IPageElementConstructor constr = null;

            if (atts.Has<ISpecificConstructorAttribute>())
            {
                var constrType = atts.Get<ISpecificConstructorAttribute>().ConstructorType;

                var constrs = m_SpecificConstructors.Where(c => constrType.IsAssignableFrom(c.Key));

                if (constrs.Count() == 1)
                {
                    constr = constrs.First().Value;
                }
                else if (!constrs.Any())
                {
                    throw new ConstructorNotFoundException(type, "Specific constructor is not registered");
                }
                else
                {
                    throw new ConstructorNotFoundException(type, "Too many constructors registered");
                }
            }
            else
            {
                if (!m_DefaultConstructors.TryGetValue(type, out constr))
                {
                    constr = m_DefaultConstructors.FirstOrDefault(
                        t => t.Key.IsAssignableFrom(type)).Value;

                    if (constr == null)
                    {
                        foreach (var specType in SpecialTypes.FindMathingSpecialTypes(type))
                        {
                            if (m_SpecialTypeConstructors.TryGetValue(specType, out constr))
                            {
                                break;
                            }
                        }
                    }
                }
            }

            if (constr != null)
            {
                return constr;
            }
            else
            {
                throw new ConstructorNotFoundException(type);
            }
        }
    }
}