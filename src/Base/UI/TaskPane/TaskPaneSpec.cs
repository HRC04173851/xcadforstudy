//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.UI.Structures;

namespace Xarial.XCad.UI.TaskPane
{
    public class TaskPaneSpec : ButtonGroupSpec
    {
        /// <summary>
        /// Buttons displayed in the task pane
        /// 任务窗格按钮列表
        /// </summary>
        public TaskPaneButtonSpec[] Buttons { get; set; }
    }
}
