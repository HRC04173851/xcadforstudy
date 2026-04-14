// -*- coding: utf-8 -*-
// src/Base/UI/Commands/Delegates/CommandClickDelegate.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义命令按钮点击事件和枚举强类型点击事件的委托，用于处理命令的用户交互
//*********************************************************************

using System;
using Xarial.XCad.UI.Commands.Structures;

namespace Xarial.XCad.UI.Commands.Delegates
{
    /// <summary>
    /// Delegate of <see cref="IXCommandGroup.CommandClick"/>
    /// <see cref="IXCommandGroup.CommandClick"/> 事件委托
    /// </summary>
    /// <param name="spec">Command specification</param>
    public delegate void CommandClickDelegate(CommandSpec spec);

    /// <summary>
    /// Delegate of specific <see cref="IXCommandGroup.CommandClick"/>
    /// 枚举强类型的 <see cref="IXCommandGroup.CommandClick"/> 委托
    /// </summary>
    /// <typeparam name="TCmdEnum">Enumaration type</typeparam>
    /// <param name="spec">Enumeration value</param>
    public delegate void CommandEnumClickDelegate<TCmdEnum>(TCmdEnum spec)
        where TCmdEnum : Enum;
}