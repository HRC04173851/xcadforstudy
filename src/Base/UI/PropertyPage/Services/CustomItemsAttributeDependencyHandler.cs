// -*- coding: utf-8 -*-
// src/Base/UI/PropertyPage/Services/CustomItemsAttributeDependencyHandler.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 实现从自定义提供器刷新项控件内容的依赖处理器，支持动态更新列表项
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xarial.XCad.UI.PropertyPage.Base;
using Xarial.XCad.UI.PropertyPage.Structures;

namespace Xarial.XCad.UI.PropertyPage.Services
{
    /// <summary>
    /// Dependency handler that refreshes items controls from custom provider
    /// 用于从自定义提供器刷新项控件内容的依赖处理器
    /// </summary>
    internal class CustomItemsAttributeDependencyHandler : IDependencyHandler
    {
        private readonly ICustomItemsProvider m_ItemsProvider;
        private readonly string m_DisplayMemberMemberPath;

        internal CustomItemsAttributeDependencyHandler(ICustomItemsProvider itemsProvider, string displayMemberMemberPath)
        {
            m_ItemsProvider = itemsProvider;
            m_DisplayMemberMemberPath = displayMemberMemberPath;
        }

        public void UpdateState(IXApplication app, IControl source, IControl[] dependencies)
        {
            var itemsCtrl = (IItemsControl)source;

            itemsCtrl.Items = m_ItemsProvider.ProvideItems(app, dependencies)
                ?.Select(i => new ItemsControlItem(i, m_DisplayMemberMemberPath)).ToArray();
        }
    }
}
