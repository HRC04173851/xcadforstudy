//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.UI.PropertyPage.Structures
{
    /// <summary>
    /// Arguments of <see cref="Services.ISelectionCustomFilter"/>
    /// <see cref="Services.ISelectionCustomFilter"/> 的参数
    /// </summary>
    public class SelectionCustomFilterArguments
    {
        /// <summary>
        /// Text of the item to be displayed in the selection box
        /// 在选择框中显示的项目文本
        /// </summary>
        public string ItemText { get; set; }

        /// <summary>
        /// True to allow this item to be selected
        /// 为 true 时允许该项被选择
        /// </summary>
        public bool Filter { get; set; }

        /// <summary>
        /// Reason to display to the user of <see cref="Filter"/> is False
        /// 当 <see cref="Filter"/> 为 false 时向用户显示的原因
        /// </summary>
        public string Reason { get; set; }
    }
}
