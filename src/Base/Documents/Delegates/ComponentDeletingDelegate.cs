// -*- coding: utf-8 -*-
// src/Base/Documents/Delegates/ComponentDeletingDelegate.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 提供装配体组件删除前通知的委托定义，用于在组件即将被删除时触发回调，允许取消删除操作。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xarial.XCad.Documents.Structures;

namespace Xarial.XCad.Documents.Delegates
{
    /// <summary>
    /// Delegate of <see cref="IXAssembly.ComponentDeleting"/> notification
    /// <see cref="IXAssembly.ComponentDeleting"/> 通知委托
    /// </summary>
    /// <param name="assembly">Assembly where component is being deleted</param>
    /// <param name="component">Component being deleted from the assembly</param>
    /// <param name="args">Deleting arguments</param>
    public delegate void ComponentDeletingDelegate(IXAssembly assembly, IXComponent component, ItemDeleteArgs args);
}
