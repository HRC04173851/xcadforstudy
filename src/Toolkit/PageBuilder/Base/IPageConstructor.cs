// -*- coding: utf-8 -*-
// src/Toolkit/PageBuilder/Base/IPageConstructor.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件定义页面构造器接口 IPageConstructor<TPage>。
// 负责根据特性集合创建页面实例。
// 是页面构建器的核心组件之一。
//*********************************************************************

namespace Xarial.XCad.Utils.PageBuilder.Base
{
    /// <summary>
    /// Creates page instances from attribute set.
    /// <para>根据特性集合创建页面实例。</para>
    /// </summary>
    /// <typeparam name="TPage">Page type.<para>页面类型。</para></typeparam>
    public interface IPageConstructor<TPage>
        where TPage : IPage
    {
        /// <summary>
        /// Creates page object.
        /// <para>创建页面对象。</para>
        /// </summary>
        TPage Create(IAttributeSet atts);
    }
}