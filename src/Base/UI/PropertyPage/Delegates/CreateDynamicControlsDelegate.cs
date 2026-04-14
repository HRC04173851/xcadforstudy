// -*- coding: utf-8 -*-
// src/Base/UI/PropertyPage/Delegates/CreateDynamicControlsDelegate.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 属性页动态控件创建委托，用于在属性页中动态创建控件描述符
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.UI.PropertyPage.Base;

namespace Xarial.XCad.UI.PropertyPage.Delegates
{
    /// <summary>
    /// Handler of dynamic controls in the proeprty page
    /// 属性页动态控件创建委托
    /// </summary>
    /// <param name="tag">Control tag</param>
    /// <returns>Dynamic control descriptors</returns>
    public delegate IControlDescriptor[] CreateDynamicControlsDelegate(object tag);
}
