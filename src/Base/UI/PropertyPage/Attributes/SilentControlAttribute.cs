// -*- coding: utf-8 -*-
// src/Base/UI/PropertyPage/Attributes/SilentControlAttribute.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 指示该控件不触发数据变化通知，用于需要抑制数据绑定通知的场景。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.UI.PropertyPage.Base;

namespace Xarial.XCad.UI.PropertyPage.Attributes
{
    /// <summary>
    /// Indicates that this control should not raise the <see cref="IXPropertyPage{TDataModel}.DataChanged"/> notification
    /// 指示该控件不触发 <see cref="IXPropertyPage{TDataModel}.DataChanged"/> 通知
    /// </summary>
    public class SilentControlAttribute : Attribute, ISilentBindingAttribute
    {
    }
}
