// -*- coding: utf-8 -*-
// src/Base/UI/PropertyPage/Base/IItemsControl.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 表示带选项项集合的数据源控件基类接口，提供项集合的访问和设置。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.UI.PropertyPage.Structures;

namespace Xarial.XCad.UI.PropertyPage.Base
{
    /// <summary>
    /// Represents the base control for items source
    /// 表示带选项项集合的数据源控件基类接口
    /// </summary>
    public interface IItemsControl : IControl
    {
        /// <summary>
        /// Items of this control
        /// 控件项集合
        /// </summary>
        ItemsControlItem[] Items { get; set; }
    }
}
