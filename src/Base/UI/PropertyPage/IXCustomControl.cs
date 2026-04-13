//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.UI.PropertyPage
{
    /// <summary>
    /// Delegate for <see cref="IXCustomControl.ValueChanged"/> event
    /// <see cref="IXCustomControl.ValueChanged"/> 事件委托
    /// </summary>
    /// <param name="sender">Sender control</param>
    /// <param name="newValue">New value</param>
    public delegate void CustomControlValueChangedDelegate(IXCustomControl sender, object newValue);

    /// <summary>
    /// Represents the custom control hosted in the page
    /// 表示属性页中托管的自定义控件
    /// </summary>
    public interface IXCustomControl
    {
        /// <summary>
        /// Raised when data context of this control is changed
        /// </summary>
        event CustomControlValueChangedDelegate ValueChanged;

        /// <summary>
        /// Returns the data context of this control
        /// </summary>
        object Value { get; set; }
    }
}
