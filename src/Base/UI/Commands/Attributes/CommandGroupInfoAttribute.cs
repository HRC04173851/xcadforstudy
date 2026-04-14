// -*- coding: utf-8 -*-
// src/Base/UI/Commands/Attributes/CommandGroupInfoAttribute.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 提供命令组的附加信息，包括用户ID和标签名称，用于在功能区、菜单或工具栏中显示命令组
//*********************************************************************

using System;

namespace Xarial.XCad.UI.Commands.Attributes
{
    /// <summary>
    /// Provides the additional information about the command group
    /// 为命令组提供附加信息
    /// </summary>
    [AttributeUsage(AttributeTargets.Enum)]
    public class CommandGroupInfoAttribute : Attribute
    {
        internal int UserId { get; }
        internal string TabName { get; }

        /// <inheritdoc cref="CommandGroupInfoAttribute"/>
        public CommandGroupInfoAttribute(int userId) : this(userId, "")
        {
        }

        /// <inheritdoc cref="CommandGroupInfoAttribute"/>
        public CommandGroupInfoAttribute(string tabName) : this(-1, tabName)
        {
        }

        /// <summary>
        /// Constructor for specifying the additional information for group
        /// 用于指定命令组附加信息的构造函数
        /// </summary>
        /// <param name="userId">User id for the command group. Must be unique per add-in</param>
        /// <param name="tabName">Name of the tab this group should be added to</param>
        public CommandGroupInfoAttribute(int userId, string tabName)
        {
            UserId = userId;
            TabName = tabName;
        }
    }
}