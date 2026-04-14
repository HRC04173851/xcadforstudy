// -*- coding: utf-8 -*-
// src/Base/UI/PropertyPage/Delegates/PageClosingDelegate.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 属性页正在关闭事件委托，在属性页关闭时触发，可通过参数修改关闭行为
//*********************************************************************

using Xarial.XCad.UI.PropertyPage.Enums;
using Xarial.XCad.UI.PropertyPage.Structures;

namespace Xarial.XCad.UI.PropertyPage.Delegates
{
    /// <summary>
    /// Delegateof <see cref="IXPropertyPage{TDataModel}.Closing"/> event
    /// <see cref="IXPropertyPage{TDataModel}.Closing"/> 事件委托
    /// </summary>
    /// <param name="reason">Reason of closing</param>
    /// <param name="arg">Additional arguments to change the closing behavior</param>
    public delegate void PageClosingDelegate(PageCloseReasons_e reason, PageClosingArg arg);
}