// -*- coding: utf-8 -*-
// src/Base/UI/PropertyPage/Structures/ItemsControlItem.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 表示列表控件中的项，包含值、显示名称和描述信息
//*********************************************************************

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using Xarial.XCad.Reflection;

namespace Xarial.XCad.UI.PropertyPage.Structures
{
    /// <summary>
    /// Represents the item in the <see cref="Base.IItemsControl"/>
    /// 表示 <see cref="Base.IItemsControl"/> 中的项
    /// </summary>
    [DebuggerDisplay("{" + nameof(DisplayName) + "} [{" + nameof(Value) + "}]")]
    public class ItemsControlItem
    {
        private static string GetDisplayName(object value, string dispMembPath)
        {
            if (!string.IsNullOrEmpty(dispMembPath))
            {
                var prps = dispMembPath.Split('.');

                var curVal = value;

                for (int i = 0; i < prps.Length; i++)
                {
                    curVal = GetPropertyValue(curVal, prps[i]);
                }

                return curVal?.ToString() ?? "";
            }
            else
            {
                if (value != null)
                {
                    string dispName = "";

                    value.GetType().TryGetAttribute<DisplayNameAttribute>(a => dispName = a.DisplayName);

                    if (string.IsNullOrEmpty(dispName))
                    {
                        dispName = value.ToString();
                    }

                    return dispName;
                }
                else 
                {
                    return "";
                }
            }
        }

        private static string GetDescription(object value)
        {
            string desc = "";

            if (value != null)
            {
                value.GetType().TryGetAttribute<DescriptionAttribute>(a => desc = a.Description);
            }

            return desc;
        }

        private static object GetPropertyValue(object value, string prpName)
        {
            if (value != null)
            {
                var prp = value.GetType().GetProperty(prpName);

                if (prp != null)
                {
                    return prp.GetValue(value, null);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Display name of the item
        /// 项显示名称
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// Value of the item
        /// 项值
        /// </summary>
        public object Value { get; }

        /// <summary>
        /// Description of the item
        /// 项描述
        /// </summary>
        public string Description { get; }

        public ItemsControlItem(object value, string dispMembPath) 
            : this(value, GetDisplayName(value, dispMembPath), GetDescription(value))
        {
        }

        public ItemsControlItem(object value, string dispName, string desc)
        {
            Value = value;
            DisplayName = dispName;
            Description = desc;
        }
    }
}
