// -*- coding: utf-8 -*-
// src/Base/UI/PropertyPage/Delegates/PageClosedDelegate.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 属性页关闭事件委托，在属性页关闭后触发并传递关闭原因
//*********************************************************************

using Xarial.XCad.UI.PropertyPage.Enums;

namespace Xarial.XCad.UI.PropertyPage.Delegates
{
    /// <summary>
    /// Delegate of <see cref="IXPropertyPage{TDataModel}.Closed"/> event
    /// <see cref="IXPropertyPage{TDataModel}.Closed"/> 事件委托
    /// </summary>
    /// <param name="reason">Reason of page closing</param>
    public delegate void PageClosedDelegate(PageCloseReasons_e reason);
}