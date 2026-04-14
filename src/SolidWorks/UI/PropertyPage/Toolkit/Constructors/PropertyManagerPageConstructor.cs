// -*- coding: utf-8 -*-
// PropertyPage/Toolkit/Constructors/PropertyManagerPageConstructor.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 属性管理器页面构造函数，负责创建属性管理器页面实例。
//*********************************************************************

using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System.Linq;
using Xarial.XCad.Base.Attributes;
using Xarial.XCad.SolidWorks.Services;
using Xarial.XCad.SolidWorks.UI.PropertyPage.Toolkit.Controls;
using Xarial.XCad.SolidWorks.UI.PropertyPage.Toolkit.Icons;
using Xarial.XCad.SolidWorks.Utils;
using Xarial.XCad.Toolkit.Services;
using Xarial.XCad.UI.PropertyPage.Attributes;
using Xarial.XCad.UI.PropertyPage.Enums;
using Xarial.XCad.Utils.PageBuilder.Base;
using Xarial.XCad.Utils.PageBuilder.Constructors;
using Xarial.XCad.Utils.Reflection;

namespace Xarial.XCad.SolidWorks.UI.PropertyPage.Toolkit.Constructors
{
    internal class PropertyManagerPageConstructor : PageConstructor<PropertyManagerPagePage>
    {
        private readonly SwApplication m_App;
        private readonly IIconsCreator m_IconsConv;
        private readonly SwPropertyManagerPageHandler m_Handler;

        internal PropertyManagerPageConstructor(SwApplication app, IIconsCreator iconsConv, SwPropertyManagerPageHandler handler)
        {
            m_App = app;
            m_IconsConv = iconsConv;

            m_Handler = handler;
            handler.Init(m_App.Sw);
        }

        protected override PropertyManagerPagePage Create(IAttributeSet atts)
            => new PropertyManagerPagePage(m_App, atts, m_IconsConv, m_Handler);
    }
}