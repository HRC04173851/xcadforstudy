// -*- coding: utf-8 -*-
// src/Base/UI/Commands/IXCommandGroup.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义命令组接口，提供命令点击事件和状态解析事件的回调，用于管理菜单、工具栏或功能区中的命令组
//*********************************************************************

using Xarial.XCad.UI.Commands.Delegates;
using Xarial.XCad.UI.Commands.Structures;

namespace Xarial.XCad.UI.Commands
{
    /// <summary>
    /// Represents the group of commands
    /// 表示命令组
    /// </summary>
    public interface IXCommandGroup
    {
        /// <summary>
        /// Event raised when the specific command button is clicked
        /// 指定命令按钮点击时触发
        /// </summary>
        event CommandClickDelegate CommandClick;


        /// <summary>
        /// Event raised when it is required to resolve the state of the button as condition has changed
        /// 当条件变化需重新计算按钮状态时触发
        /// </summary>
        event CommandStateDelegate CommandStateResolve;

        /// <summary>
        /// Specification of the group
        /// 命令组规格定义
        /// </summary>
        CommandGroupSpec Spec { get; }
    }
}