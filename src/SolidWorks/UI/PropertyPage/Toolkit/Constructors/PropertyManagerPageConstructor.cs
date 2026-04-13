//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
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
    /// <summary>
    /// PMP 根页面构造器。
    /// 负责创建 PropertyManagerPagePage 并初始化处理器与 SolidWorks 会话绑定。
    /// </summary>
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
            // 将处理器绑定到当前 SolidWorks 会话，接收控件回调事件
            handler.Init(m_App.Sw);
        }

        protected override PropertyManagerPagePage Create(IAttributeSet atts)
            => new PropertyManagerPagePage(m_App, atts, m_IconsConv, m_Handler);
    }
}