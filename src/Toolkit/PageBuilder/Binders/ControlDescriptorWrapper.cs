// -*- coding: utf-8 -*-
// src/Toolkit/PageBuilder/Binders/ControlDescriptorWrapper.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件实现控件描述符包装器 ControlDescriptorWrapper。
// 转换动态控件上下文，使其在运行时对应到属性值对象。
// 委托基础描述符的操作，并处理上下文属性路径解析。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Xarial.XCad.UI;
using Xarial.XCad.UI.PropertyPage.Base;

namespace Xarial.XCad.Toolkit.PageBuilder.Binders
{
    /// <summary>
    /// Wrapper is used to transform context of the dynamic control to be equal to property value resolved in runtime
    /// <para>该包装器用于转换动态控件上下文，使其在运行时对应到属性值对象。</para>
    /// </summary>
    public class ControlDescriptorWrapper : IControlDescriptor
    {
        public string DisplayName => m_BaseCtrlDesc.DisplayName;
        public string Description => m_BaseCtrlDesc.Description;
        public string Name => m_BaseCtrlDesc.Name;
        public IXImage Icon => m_BaseCtrlDesc.Icon;
        public Type DataType => m_BaseCtrlDesc.DataType;
        public IAttribute[] Attributes => m_BaseCtrlDesc.Attributes;

        public object GetValue(object context)
            => m_BaseCtrlDesc.GetValue(GetContext(context));

        public void SetValue(object context, object value)
            => m_BaseCtrlDesc.SetValue(GetContext(context), value);

        private readonly IControlDescriptor m_BaseCtrlDesc;
        private readonly PropertyInfo m_PrpInfo;

        /// <summary>
        /// Initializes wrapper around base descriptor and context property.
        /// <para>使用基础描述符与上下文属性初始化包装器。</para>
        /// </summary>
        public ControlDescriptorWrapper(IControlDescriptor baseCtrlDesc, PropertyInfo prpInfo) 
        {
            m_BaseCtrlDesc = baseCtrlDesc;
            m_PrpInfo = prpInfo;
        }

        private object GetContext(object context)
        {
            if (context != null)
            {
                return m_PrpInfo.GetValue(context, null);
            }
            else
            {
                return null;
            }
        }
    }
}
