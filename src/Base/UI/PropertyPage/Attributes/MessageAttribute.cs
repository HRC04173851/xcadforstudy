// -*- coding: utf-8 -*-
// src/Base/UI/PropertyPage/Attributes/MessageAttribute.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 允许在属性管理器页面显示消息文本，支持设置可见性和展开状态。
//*********************************************************************

using System;
using Xarial.XCad.UI.PropertyPage.Base;
using Xarial.XCad.UI.PropertyPage.Enums;

namespace Xarial.XCad.UI.PropertyPage.Attributes
{
    /// <summary>
    /// Attributes allows to specify the message to be displayed in the property manager page
    /// 允许在属性管理器页面显示消息文本的特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class MessageAttribute : Attribute, IAttribute
    {
        public string Text { get; private set; }
        public PageMessageVisibility Visibility { get; private set; }
        public PageMessageExpanded Expanded { get; private set; }
        public string Caption { get; private set; }

        /// <summary>
        /// Constructor to specify message and its parameters
        /// </summary>
        /// <param name="text">Text to be displayed in the message</param>
        /// <param name="caption">Message box caption</param>
        /// <param name="visibility">Visibility option</param>
        /// <param name="expanded">Expansion state</param>
        public MessageAttribute(string text, string caption,
            PageMessageVisibility visibility = PageMessageVisibility.Visible,
            PageMessageExpanded expanded = PageMessageExpanded.Expand)
        {
            Text = text;
            Caption = caption;
            Visibility = visibility;
            Expanded = expanded;
        }
    }
}