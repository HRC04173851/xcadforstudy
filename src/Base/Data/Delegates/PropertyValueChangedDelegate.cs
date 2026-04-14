// -*- coding: utf-8 -*-
// Delegates/PropertyValueChangedDelegate.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义属性值变更事件委托，用于在属性值发生变化时通知监听器。
// 该委托被 IXProperty 接口的 ValueChanged 事件使用。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.Data.Delegates
{
    /// <summary>
    /// Delegate for <see cref="IXProperty.ValueChanged"/> event
    /// <see cref="IXProperty.ValueChanged"/> 事件委托
    /// </summary>
    /// <param name="prp">Event sender</param>
    /// <param name="newValue">New value assigned to the property</param>
    public delegate void PropertyValueChangedDelegate(IXProperty prp, object newValue);
}
