// -*- coding: utf-8 -*-
// src/Toolkit/PageBuilder/Base/IAttributeSet.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件定义页面构建元素特性集合接口 IAttributeSet。
// 包含控件标识符、标签、名称、描述、上下文类型和控件描述符。
// 提供特性的检查、获取、添加等操作，管理页面元素的元数据。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Reflection;
using Xarial.XCad.UI.PropertyPage.Base;
using Xarial.XCad.Utils.PageBuilder.Binders;

namespace Xarial.XCad.Utils.PageBuilder.Base
{
    /// <summary>
    /// Represents metadata and attribute collection for page builder element.
    /// <para>表示页面构建元素的元数据与特性集合。</para>
    /// </summary>
    public interface IAttributeSet
    {
        /// <summary>
        /// Unique control identifier.
        /// <para>唯一控件标识符。</para>
        /// </summary>
        int Id { get; }
        /// <summary>
        /// Arbitrary tag value for dependency resolution.
        /// <para>用于依赖解析的任意标签值。</para>
        /// </summary>
        object Tag { get; }
        /// <summary>
        /// Display name.
        /// <para>显示名称。</para>
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Description text.
        /// <para>说明文本。</para>
        /// </summary>
        string Description { get; }
        /// <summary>
        /// Data model context type.
        /// <para>数据模型上下文类型。</para>
        /// </summary>
        Type ContextType { get; }
        /// <summary>
        /// Control descriptor bound to this attribute set.
        /// <para>与该特性集绑定的控件描述符。</para>
        /// </summary>
        IControlDescriptor ControlDescriptor { get; }

        /// <summary>
        /// Checks whether attribute of specified type exists.
        /// <para>检查是否存在指定类型的特性。</para>
        /// </summary>
        bool Has<TAtt>() where TAtt : IAttribute;

        /// <summary>
        /// Gets first attribute of specified type.
        /// <para>获取指定类型的首个特性。</para>
        /// </summary>
        TAtt Get<TAtt>() where TAtt : IAttribute;

        /// <summary>
        /// Gets all attributes of specified type.
        /// <para>获取指定类型的全部特性。</para>
        /// </summary>
        IEnumerable<TAtt> GetAll<TAtt>() where TAtt : IAttribute;

        /// <summary>
        /// Adds attribute to set.
        /// <para>向集合中添加特性。</para>
        /// </summary>
        void Add<TAtt>(TAtt att) where TAtt : IAttribute;
    }
}