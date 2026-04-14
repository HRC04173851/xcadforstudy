// -*- coding: utf-8 -*-
// Commands/Attributes/SwContextMenuCommandGroupInfoAttribute.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// SOLIDWORKS特定的上下文命令菜单规范，用于定义上下文命令组信息。
//*********************************************************************

using SolidWorks.Interop.swconst;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xarial.XCad.UI.Commands.Attributes;

namespace Xarial.XCad.SolidWorks.UI.Commands.Attributes
{
    /// <summary>
    /// SOLIDWORKS specific context command menu spec
    /// </summary>
    public class SwContextMenuCommandGroupInfoAttribute : ContextMenuCommandGroupInfoAttribute
    {
        /// <summary>
        /// Selection type of the owner for this context menu
        /// </summary>
        public new swSelectType_e Owner { get; }

        /// <summary>
        /// Default construcotr
        /// </summary>
        /// <param name="owner">selection type of the owner for this context menu</param>
        public SwContextMenuCommandGroupInfoAttribute(int userId, swSelectType_e owner) : base(userId, null)
        {
            Owner = owner;
        }
    }
}
