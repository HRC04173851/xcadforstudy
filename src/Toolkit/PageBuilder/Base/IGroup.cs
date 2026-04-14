// -*- coding: utf-8 -*-
// src/Toolkit/PageBuilder/Base/IGroup.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件定义分组容器接口 IGroup。
// 表示 PropertyManager 页面中的容器分组控件。
// 作为页面元素的容器，可包含其他控件或子分组。
//*********************************************************************

using Xarial.XCad.UI.PropertyPage.Base;

namespace Xarial.XCad.Utils.PageBuilder.Base
{
    /// <summary>
    /// Represents a container control group in PropertyManager page.
    /// <para>表示 PropertyManager 页面中的容器分组控件。</para>
    /// </summary>
    public interface IGroup : IControl
    {
    }
}