//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;

namespace Xarial.XCad.UI.Commands.Attributes
{
    /// <summary>
    /// Allwos to customize the context menu command created with <see cref="IXCommandManager.AddContextMenu(Structures.ContextMenuCommandGroupSpec)"/>
    /// 允许自定义通过 <see cref="IXCommandManager.AddContextMenu(Structures.ContextMenuCommandGroupSpec)"/> 创建的上下文菜单命令组
    /// </summary>
    public class ContextMenuCommandGroupInfoAttribute : CommandGroupInfoAttribute
    {
        /// <summary>
        /// Type where context menu is attached to
        /// 上下文菜单附着的对象类型
        /// </summary>
        public Type Owner { get; }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="owner">Type to wheer attach the context menu</param>
        /// <inheritdoc/>
        public ContextMenuCommandGroupInfoAttribute(int userId, Type owner) : base(userId)
        {
            Owner = owner;
        }
    }
}
