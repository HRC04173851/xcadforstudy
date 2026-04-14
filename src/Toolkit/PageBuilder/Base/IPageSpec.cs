// -*- coding: utf-8 -*-
// src/Toolkit/PageBuilder/Base/IPageSpec.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件定义页面规格接口 IPageSpec。
// 描述页面标题和 PropertyManager 页面选项。
// 用于配置页面的显示属性和行为选项。
//*********************************************************************

using Xarial.XCad.UI.PropertyPage.Enums;

namespace Xarial.XCad.Utils.PageBuilder.Base
{
    /// <summary>
    /// Describes page title and option flags.
    /// <para>描述页面标题和选项标志。</para>
    /// </summary>
    public interface IPageSpec
    {
        /// <summary>
        /// Page title text.
        /// <para>页面标题文本。</para>
        /// </summary>
        string Title { get; }
        /// <summary>
        /// PropertyManager page options.
        /// <para>PropertyManager 页面选项。</para>
        /// </summary>
        PageOptions_e Options { get; }
    }
}