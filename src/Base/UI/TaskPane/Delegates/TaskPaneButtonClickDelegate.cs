// -*- coding: utf-8 -*-
// src/Base/UI/TaskPane/Delegates/TaskPaneButtonClickDelegate.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义任务窗格按钮点击事件的委托，包括通用按钮点击和基于枚举类型的按钮点击委托。
//*********************************************************************

using System;
using Xarial.XCad.UI.Structures;
using Xarial.XCad.UI.TaskPane;

namespace Xarial.XCad.UI.TaskPane.Delegates
{
    /// <summary>
    /// Task pane button click delegate
    /// 任务窗格按钮点击委托
    /// </summary>
    public delegate void TaskPaneButtonClickDelegate(TaskPaneButtonSpec spec);

    /// <summary>
    /// Enum-based task pane button click delegate
    /// 基于枚举的任务窗格按钮点击委托
    /// </summary>
    public delegate void TaskPaneButtonEnumClickDelegate<TCmdEnum>(TCmdEnum spec)
        where TCmdEnum : Enum;
}