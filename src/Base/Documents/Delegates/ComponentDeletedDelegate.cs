// -*- coding: utf-8 -*-
// src/Base/Documents/Delegates/ComponentDeletedDelegate.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 提供装配体组件删除完成通知的委托定义，用于在组件从装配体中删除后触发回调。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xarial.XCad.Documents.Delegates
{
    /// <summary>
    /// Delegate of <see cref="IXAssembly.ComponentDeleted"/> notification
    /// <see cref="IXAssembly.ComponentDeleted"/> 通知委托
    /// </summary>
    /// <param name="assembly">Assembly where component is deleted（发生删除的装配体）</param>
    /// <param name="component">Component deleted from the assembly（被删除组件，指针可能已失效）</param>
    public delegate void ComponentDeletedDelegate(IXAssembly assembly, IXComponent component);
}
