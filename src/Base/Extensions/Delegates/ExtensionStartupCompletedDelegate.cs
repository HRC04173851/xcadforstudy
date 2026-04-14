// -*- coding: utf-8 -*-
// Delegates/ExtensionStartupCompletedDelegate.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义 IXExtension 启动完成事件委托，在扩展启动完成且应用与组件完全加载后触发
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.Extensions.Delegates
{
    /// <summary>
    /// Delegate for <see cref="IXExtension.StartupCompleted"/> event
    /// <see cref="IXExtension.StartupCompleted"/> 事件委托
    /// </summary>
    /// <param name="ext">Extension</param>
    public delegate void ExtensionStartupCompletedDelegate(IXExtension ext);
}
