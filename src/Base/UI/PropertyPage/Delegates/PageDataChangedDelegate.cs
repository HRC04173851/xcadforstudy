// -*- coding: utf-8 -*-
// src/Base/UI/PropertyPage/Delegates/PageDataChangedDelegate.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 属性页数据变更事件委托，当属性页中的数据模型发生变更时触发
//*********************************************************************

namespace Xarial.XCad.UI.PropertyPage.Delegates
{
    /// <summary>
    /// Delegate of <see cref="IXPropertyPage{TDataModel}.DataChanged"/> event
    /// <see cref="IXPropertyPage{TDataModel}.DataChanged"/> 事件委托
    /// </summary>
    public delegate void PageDataChangedDelegate();
}