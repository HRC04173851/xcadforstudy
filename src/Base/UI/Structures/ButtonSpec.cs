// -*- coding: utf-8 -*-
// src/Base/UI/Structures/ButtonSpec.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义按钮规格类，用于描述按钮的用户标识、标题、提示和图标等属性。
// 支持从枚举类型初始化按钮规格，自动提取枚举的显示名称和图标特性。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using Xarial.XCad.Base.Attributes;
using Xarial.XCad.Reflection;

namespace Xarial.XCad.UI.Structures
{
    /// <summary>
    /// Specification of the generic button
    /// 通用按钮规格定义
    /// </summary>
    public class ButtonSpec
    {
        /// <summary>
        /// User id if this button
        /// 按钮用户标识 ID
        /// </summary>
        public int UserId { get; }

        /// <summary>
        /// Title of the button
        /// 按钮标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Tooltip of the button
        /// 按钮提示文本
        /// </summary>
        public string Tooltip { get; set; }

        /// <summary>
        /// Icon associated with the button
        /// 按钮关联图标
        /// </summary>
        public IXImage Icon { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="userId">Button user id</param>
        public ButtonSpec(int userId) 
        {
            UserId = userId;
        }
    }

    internal static class ButtonSpecExtension 
    {
        internal static void InitFromEnum<TEnum>(this ButtonSpec btn, TEnum btnEnum)
            where TEnum : Enum
        {
            if (!btnEnum.TryGetAttribute<DisplayNameAttribute>(
                att => btn.Title = att.DisplayName))
            {
                btn.Title = btnEnum.ToString();
            }

            if (!btnEnum.TryGetAttribute<DescriptionAttribute>(
                att => btn.Tooltip = att.Description))
            {
                btn.Tooltip = btn.Title;
            }

            if (!btnEnum.TryGetAttribute<IconAttribute>(a => btn.Icon = a.Icon))
            {
                btn.Icon = Defaults.Icon;
            }
        }
    }
}
