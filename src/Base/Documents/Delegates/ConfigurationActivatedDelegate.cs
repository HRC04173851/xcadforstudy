// -*- coding: utf-8 -*-
// src/Base/Documents/Delegates/ConfigurationActivatedDelegate.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 提供配置激活事件的委托定义，用于在文档的配置被激活或切换时触发通知。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.Documents.Delegates
{
    /// <summary>
    /// Delegate for <see cref="IXConfigurationRepository.ConfigurationActivated"/> event
    /// <see cref="IXConfigurationRepository.ConfigurationActivated"/> 事件委托
    /// </summary>
    /// <param name="doc">Document owner of this configuration</param>
    /// <param name="newConf">Configuration which is activated</param>
    public delegate void ConfigurationActivatedDelegate(IXDocument3D doc, IXConfiguration newConf);
}