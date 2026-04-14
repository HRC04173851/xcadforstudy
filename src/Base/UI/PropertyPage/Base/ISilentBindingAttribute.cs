// -*- coding: utf-8 -*-
// src/Base/UI/PropertyPage/Base/ISilentBindingAttribute.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 指示此绑定不触发数据变化通知，用于抑制控件的数据更新事件。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.UI.PropertyPage.Base
{
    /// <summary>
    /// Indicates that this binding should not fire the data changed notifications
    /// 指示此绑定不触发数据变化通知
    /// </summary>
    public interface ISilentBindingAttribute : IAttribute
    {
    }
}
