// -*- coding: utf-8 -*-
// src/Toolkit/PageBuilder/Base/IBindingManager.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件定义绑定管理器接口 IBindingManager。
// 管理页面控件的数据绑定集合和依赖关系图。
// 提供加载绑定、依赖定义和元数据的方法。
//*********************************************************************

using System.Collections.Generic;
using Xarial.XCad.UI.PropertyPage.Base;

namespace Xarial.XCad.Utils.PageBuilder.Base
{
    /// <summary>
    /// Manages bindings and dependency graph for page controls.
    /// <para>管理页面控件的数据绑定与依赖关系图。</para>
    /// </summary>
    public interface IBindingManager
    {
        /// <summary>
        /// Collection of control bindings.
        /// <para>控件绑定集合。</para>
        /// </summary>
        IEnumerable<IBinding> Bindings { get; }
        /// <summary>
        /// Dependency manager instance.
        /// <para>依赖管理器实例。</para>
        /// </summary>
        IDependencyManager Dependency { get; }
        /// <summary>
        /// Metadata associated with current page model.
        /// <para>与当前页面模型关联的元数据。</para>
        /// </summary>
        IMetadata[] Metadata { get; }

        /// <summary>
        /// Initializes manager with bindings, dependency definitions and metadata.
        /// <para>使用绑定、依赖定义和元数据初始化管理器。</para>
        /// </summary>
        void Load(IXApplication app, IEnumerable<IBinding> bindings, IRawDependencyGroup dependencies, IMetadata[] metadata);
    }
}