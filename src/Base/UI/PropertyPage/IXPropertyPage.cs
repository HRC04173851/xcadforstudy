// -*- coding: utf-8 -*-
// src/Base/UI/PropertyPage/IXPropertyPage.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义泛型属性页接口，提供数据模型绑定、页面生命周期事件和关闭控制
//*********************************************************************

using Xarial.XCad.UI.PropertyPage.Delegates;

namespace Xarial.XCad.UI.PropertyPage
{
    /// <summary>
    /// Represents native proeprty page to manage entity parameters
    /// 表示用于管理实体参数的原生属性页
    /// </summary>
    /// <typeparam name="TDataModel"></typeparam>
    public interface IXPropertyPage<TDataModel>
    {
        /// <summary>
        /// Fired when the data is changed (i.e. text box changed, combobox selection changed etc.)
        /// 当数据变化（文本框、下拉框等）时触发
        /// </summary>
        event PageDataChangedDelegate DataChanged;

        /// <summary>
        /// Fired when property page is about to be closed. Use the argument to provide additional instructions
        /// 属性页即将关闭时触发，可通过参数控制关闭行为
        /// </summary>
        event PageClosingDelegate Closing;

        /// <summary>
        /// Fired when property manager page is closed
        /// 属性管理器页面关闭后触发
        /// </summary>
        event PageClosedDelegate Closed;

        /// <summary>
        /// Keystroke handler if page created with option <see cref="Enums.PageOptions_e.HandleKeystrokes"/>
        /// </summary>
        event KeystrokeHookDelegate KeystrokeHook;

        /// <summary>
        /// Checks if page is pinned
        /// </summary>
        bool IsPinned { get; set; }

        /// <summary>
        /// Opens the property page with the specified data model
        /// </summary>
        /// <param name="model">Pointer to an instance of the bound data model</param>
        void Show(TDataModel model);

        /// <summary>
        /// Closes the current page
        /// </summary>
        /// <param name="cancel">Cancel the current page or OK</param>
        void Close(bool cancel);
    }
}