// -*- coding: utf-8 -*-
// PropertyPage/Services/SwCustomItemsProvider.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 自定义项提供器基类，用于为属性管理器页面提供动态生成的自定义项。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xarial.XCad.UI.PropertyPage.Base;
using Xarial.XCad.UI.PropertyPage.Services;

namespace Xarial.XCad.SolidWorks.UI.PropertyPage.Services
{
    public abstract class SwCustomItemsProvider<TItem> : ICustomItemsProvider
    {
        IEnumerable<object> ICustomItemsProvider.ProvideItems(IXApplication app, IControl[] dependencies) 
            => ProvideItems((ISwApplication)app, dependencies)?.Cast<object>();

        public virtual IEnumerable<TItem> ProvideItems(ISwApplication app, IControl[] dependencies)
            => ProvideItems(app);

        public virtual IEnumerable<TItem> ProvideItems(ISwApplication app)
            => Enumerable.Empty<TItem>();
    }
}
