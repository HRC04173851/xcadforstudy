// -*- coding: utf-8 -*-
// src/Base/Documents/Delegates/ComponentInsertedDelegate.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 提供装配体组件插入通知的委托定义，用于在组件成功插入装配体后触发回调。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xarial.XCad.Documents.Delegates
{
    /// <summary>
    /// Delegate of <see cref="IXAssembly.ComponentInserted"/> notification
    /// <see cref="IXAssembly.ComponentInserted"/> 通知委托
    /// </summary>
    /// <param name="assembly">Assembly where component is inserted</param>
    /// <param name="component">Component inserted into the assembly</param>
    public delegate void ComponentInsertedDelegate(IXAssembly assembly, IXComponent component);
}
