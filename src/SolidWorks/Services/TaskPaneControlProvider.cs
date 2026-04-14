// -*- coding: utf-8 -*-
// src/SolidWorks/Services/TaskPaneControlProvider.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 提供任务窗格控件的创建功能，支持通过COM控件和.NET控件两种方式在任务窗格中嵌入自定义界面，实现工具栏扩展功能。
//*********************************************************************

using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Xarial.XCad.SolidWorks.Services
{
    public interface ITaskPaneControlProvider
    {
        object ProvideComControl(ITaskpaneView taskPaneView, string progId);
        bool ProvideNetControl(ITaskpaneView taskPaneView, Control ctrl);
    }

    internal class TaskPaneControlProvider : ITaskPaneControlProvider
    {
        public object ProvideComControl(ITaskpaneView taskPaneView, string progId)
            => taskPaneView.AddControl(progId, "");

        public bool ProvideNetControl(ITaskpaneView taskPaneView, Control ctrl)
            => taskPaneView.DisplayWindowFromHandle(ctrl.Handle.ToInt32());
    }
}
