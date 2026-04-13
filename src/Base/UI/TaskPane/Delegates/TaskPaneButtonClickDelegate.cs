//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
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