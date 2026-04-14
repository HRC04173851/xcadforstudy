// -*- coding: utf-8 -*-
// src/SolidWorks/Services/ModelViewControlProvider.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 提供模型视图控件的创建功能，支持通过COM控件和.NET控件两种方式在模型视图中嵌入自定义界面，实现文档交互扩展。
//*********************************************************************

using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Xarial.XCad.SolidWorks.Services
{
    public interface IModelViewControlProvider
    {
        object ProvideComControl(IModelViewManager mdlViewMgr, string progId, string title);
        bool ProvideNetControl(IModelViewManager mdlViewMgr, Control ctrl, string title);
    }

    internal class ModelViewControlProvider : IModelViewControlProvider
    {
        public object ProvideComControl(IModelViewManager mdlViewMgr, string progId, string title)
            => mdlViewMgr.AddControl3(title, progId, "", true);

        public bool ProvideNetControl(IModelViewManager mdlViewMgr, Control ctrl, string title)
            => mdlViewMgr.DisplayWindowFromHandlex64(title, ctrl.Handle.ToInt64(), true);
    }
}
