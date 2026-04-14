// -*- coding: utf-8 -*-
// src/Base/UI/PropertyPage/Base/IControl.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 表示属性页中的控件，提供值获取、设置、状态管理和事件通知等功能。
//*********************************************************************

using System;

namespace Xarial.XCad.UI.PropertyPage.Base
{
    /// <summary>
    /// Delegate of <see cref="IControl.ValueChanged"/> event
    /// <see cref="IControl.ValueChanged"/> 事件委托
    /// </summary>
    /// <param name="sender">Control</param>
    /// <param name="newValue">New value of the control</param>
    public delegate void ControlObjectValueChangedDelegate(IControl sender, object newValue);

    /// <summary>
    /// Represents the control in the page
    /// 表示属性页中的控件
    /// </summary>
    public interface IControl : IDisposable
    {
        /// <summary>
        /// Fired when the value of the control has been changed
        /// 控件值变化时触发
        /// </summary>
        event ControlObjectValueChangedDelegate ValueChanged;

        /// <summary>
        /// Metadata attached to this control
        /// 附加到控件上的元数据
        /// </summary>
        IMetadata[] Metadata { get; }

        /// <summary>
        /// Manages the enable state of the control
        /// </summary>
        bool Enabled { get; set; }

        /// <summary>
        /// Manages the visibility of the control
        /// </summary>
        bool Visible { get; set; }

        /// <summary>
        /// Identifier of this control
        /// </summary>
        int Id { get; }

        /// <summary>
        /// Custom tag associated with this control via <see cref="IControlTagAttribute"/>
        /// </summary>
        object Tag { get; }

        /// <summary>
        /// Gets the value from this control
        /// </summary>
        /// <returns></returns>
        object GetValue();

        /// <summary>
        /// Sets the value to this control
        /// </summary>
        /// <param name="value"></param>
        void SetValue(object value);

        /// <summary>
        /// Shows tooltip for this control
        /// </summary>
        /// <param name="title">Title of the tooltip</param>
        /// <param name="msg">Message to show in the tooltip</param>
        void ShowTooltip(string title, string msg);

        /// <summary>
        /// Updated this control
        /// </summary>
        void Update();

        /// <summary>
        /// Sets the focus to the current control
        /// </summary>
        void Focus();

        /// <summary>
        /// Specific type of the value for this control
        /// </summary>
        /// <remarks>This is a type of the property this control value is bound to</remarks>
        Type ValueType { get; }
    }
}