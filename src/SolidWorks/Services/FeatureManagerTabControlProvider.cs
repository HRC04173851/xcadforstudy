// -*- coding: utf-8 -*-
// src/SolidWorks/Services/FeatureManagerTabControlProvider.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 提供FeatureManager标签页控件的创建功能，支持通过COM控件和.NET控件两种方式在标签页中嵌入自定义界面。
//*********************************************************************

using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Xarial.XCad.SolidWorks.Services
{
    public interface IFeatureManagerTabControlProvider
    {
        IFeatMgrView ProvideComControl(IModelViewManager mdlViewMgr, string imgPath, string progId, string title);
        IFeatMgrView ProvideNetControl(IModelViewManager mdlViewMgr, Control ctrl, string imgPath, string title);
    }

    internal class FeatureManagerTabControlProvider : IFeatureManagerTabControlProvider
    {
        public IFeatMgrView ProvideComControl(IModelViewManager mdlViewMgr, string imgPath, string progId, string title)
            => mdlViewMgr.CreateFeatureMgrControl3(imgPath, progId, "", title,
                (int)swFeatMgrPane_e.swFeatMgrPaneBottom) as IFeatMgrView;

        public IFeatMgrView ProvideNetControl(IModelViewManager mdlViewMgr, Control ctrl, string imgPath, string title)
            => mdlViewMgr.CreateFeatureMgrWindowFromHandlex64(
                imgPath, ctrl.Handle.ToInt64(),
                title, (int)swFeatMgrPane_e.swFeatMgrPaneBottom) as IFeatMgrView;
    }
}
