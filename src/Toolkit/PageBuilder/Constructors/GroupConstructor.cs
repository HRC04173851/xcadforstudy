// -*- coding: utf-8 -*-
// src/Toolkit/PageBuilder/Constructors/GroupConstructor.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件实现分组控件构造器的抽象基类 GroupConstructor。
// 继承自 PageElementConstructor，专门处理分组类型控件。
// 支持分组嵌套和分组内控件管理。
//*********************************************************************

using Xarial.XCad.Utils.PageBuilder.Base;

namespace Xarial.XCad.Utils.PageBuilder.Constructors
{
    /// <summary>
    /// Base constructor for group controls.
    /// <para>用于分组控件的构造器基类。</para>
    /// </summary>
    /// <typeparam name="TGroup">Group control type.<para>分组控件类型。</para></typeparam>
    /// <typeparam name="TPage">Page type.<para>页面类型。</para></typeparam>
    public abstract class GroupConstructor<TGroup, TPage> : PageElementConstructor<TGroup, TGroup, TPage>
        where TGroup : IGroup
        where TPage : IPage
    {
    }
}