// -*- coding: utf-8 -*-
// src/Base/UI/Commands/IXCommandManager.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义命令管理器接口，负责管理工具栏、菜单和功能区中的命令组，支持添加和移除命令组
//*********************************************************************

using System.Collections.Generic;
using Xarial.XCad.Base.Enums;
using Xarial.XCad.UI.Commands.Structures;

namespace Xarial.XCad.UI.Commands
{
    /// <summary>
    /// Represents the command manager (toolabr and menus)
    /// 表示命令管理器（工具栏与菜单）
    /// </summary>
    public interface IXCommandManager
    {
        /// <summary>
        /// Command groups belonging to this command manager
        /// 命令管理器包含的命令组
        /// </summary>
        IXCommandGroup[] CommandGroups { get; }

        /// <summary>
        /// Adds new command group (menu, toolbar or ribbon)
        /// 添加新命令组（菜单、工具栏或功能区）
        /// </summary>
        /// <param name="cmdBar">Specification of command group</param>
        /// <returns>Command group</returns>
        IXCommandGroup AddCommandGroup(CommandGroupSpec cmdBar);

        /// <summary>
        /// Adds new context menu
        /// 添加新上下文菜单
        /// </summary>
        /// <param name="cmdBar">Specification of the context menu</param>
        /// <returns>Command group</returns>
        /// <remarks>Use <see cref="Attributes.ContextMenuCommandGroupInfoAttribute"/> attribute to assign additional parameters for the context menu</remarks>
        IXCommandGroup AddContextMenu(ContextMenuCommandGroupSpec cmdBar);
    }
}