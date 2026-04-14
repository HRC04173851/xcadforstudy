// -*- coding: utf-8 -*-
// src/Base/UI/PropertyPage/Attributes/DependentOnAttribute.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 声明控件依赖其他控件的特性，当依赖控件值变化时触发更新处理。
//*********************************************************************

using System;
using Xarial.XCad.UI.PropertyPage.Base;
using Xarial.XCad.UI.PropertyPage.Services;

namespace Xarial.XCad.UI.PropertyPage.Attributes
{   
    /// <summary>
    /// Attribute declaring control dependency on other controls
    /// 声明控件依赖其他控件的特性
    /// </summary>
    /// <inheritdoc/>
    public class DependentOnAttribute : Attribute, IDependentOnAttribute
    {
        /// <inheritdoc/>
        public object[] Dependencies { get; }

        /// <inheritdoc/>
        public IDependencyHandler DependencyHandler { get; }

        public DependentOnAttribute(Type dependencyHandler, params object[] dependencies)
        {
            if (!typeof(IDependencyHandler).IsAssignableFrom(dependencyHandler)) 
            {
                throw new InvalidCastException($"{dependencyHandler.FullName} must be assignable from {typeof(IDependencyHandler).FullName}");
            }

            DependencyHandler = (IDependencyHandler)Activator.CreateInstance(dependencyHandler);

            Dependencies = dependencies;
        }
    }
}