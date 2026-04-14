// -*- coding: utf-8 -*-
// src/Base/UI/TaskPane/TaskPaneButtonSpec.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 任务窗格按钮规格类，继承自按钮规格，用于定义单个按钮的属性，包括标准图标类型。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.UI.Structures;
using Xarial.XCad.UI.TaskPane.Enums;

namespace Xarial.XCad.UI.TaskPane
{
    public class TaskPaneButtonSpec : ButtonSpec
    {
        /// <summary>
        /// Standard icon type of this task pane button
        /// 任务窗格按钮的标准图标类型
        /// </summary>
        public TaskPaneStandardIcons_e? StandardIcon { get; set; }

        public TaskPaneButtonSpec(int userId) : base(userId) 
        {
        }
    }

    internal class TaskPaneEnumButtonSpec<TEnum> : TaskPaneButtonSpec
        where TEnum : Enum
    {
        public TEnum Value { get; set; }

        public TaskPaneEnumButtonSpec(int userId) : base(userId)
        {
        }
    }
}
