// -*- coding: utf-8 -*-
// src/Base/UI/PropertyPage/Services/IDependencyHandler.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义依赖处理器接口，用于处理动态控件依赖关系的更新逻辑
//*********************************************************************

using Xarial.XCad.UI.PropertyPage.Base;

namespace Xarial.XCad.UI.PropertyPage.Services
{
    /// <summary>
    /// Handling the dynamic control dependencies
    /// 处理动态控件依赖关系
    /// </summary>
    /// <remarks>This is asigned via <see cref="Attributes.DependentOnAttribute"/></remarks>
    public interface IDependencyHandler
    {
        /// <summary>
        /// Invokes when any of the dependencies controls changed
        /// 当任一依赖控件变化时调用
        /// </summary>
        /// <param name="app">Main application</param>
        /// <param name="source">This control to update state on</param>
        /// <param name="dependencies">List of dependencies controls</param>
        void UpdateState(IXApplication app, IControl source, IControl[] dependencies);
    }
}
