// -*- coding: utf-8 -*-
// Toolkit/IAutoDisposable.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 自动释放接口，表示该对象在插件卸载时将被自动释放资源。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.SolidWorks.UI.Toolkit
{
    /// <summary>
    /// Indicates that this item will be disposed when main add-in is unloaded
    /// </summary>
    internal interface IAutoDisposable : IDisposable
    {
        event Action<IAutoDisposable> Disposed;
    }
}
