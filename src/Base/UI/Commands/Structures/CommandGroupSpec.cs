// -*- coding: utf-8 -*-
// src/Base/UI/Commands/Structures/CommandGroupSpec.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义命令组规格类，包含父命令组引用和组ID，用于在创建命令组时传递配置参数
//*********************************************************************

using System;
using Xarial.XCad.UI.Structures;

namespace Xarial.XCad.UI.Commands.Structures
{
    /// <summary>
    /// Represents the group of commands
    /// 表示命令组规格
    /// </summary>
    public class CommandGroupSpec : ButtonGroupSpec
    {
        /// <summary>
        /// Parent group or null for root group
        /// 父命令组；根组为 null
        /// </summary>
        public CommandGroupSpec Parent { get; set; }

        /// <summary>
        /// Id of this group
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Commands associated with this group
        /// 该组关联的命令集合
        /// </summary>
        public virtual CommandSpec[] Commands { get; set; }

        /// <summary>
        /// Name of the ribbon tab where his groupd should be added
        /// </summary>
        /// <remarks>If this is not set the name of the group is used or the name of the root parent if this group is a sub group</remarks>
        public string RibbonTabName { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="id">Group id</param>
        public CommandGroupSpec(int id) 
        {
            Id = id;
        }
    }

    /// <summary>
    /// Represents command groups of the context menu
    /// </summary>
    public class ContextMenuCommandGroupSpec : CommandGroupSpec 
    {
        /// <summary>
        /// Owner of the context menu
        /// </summary>
        public Type Owner { get; set; }

        /// <inheritdoc/>
        public ContextMenuCommandGroupSpec(int id) : base(id)
        {
        }
    }
}