// -*- coding: utf-8 -*-
// Delegates/ExtensionConnectDelegate.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义 IXExtension 连接事件委托，在扩展加载时触发
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.Extensions.Delegates
{
    /// <summary>
    /// Delegate for <see cref="IXExtension.Connect"/> event
    /// <see cref="IXExtension.Connect"/> 事件委托
    /// </summary>
    /// <param name="ext">Extension</param>
    public delegate void ExtensionConnectDelegate(IXExtension ext);
}
