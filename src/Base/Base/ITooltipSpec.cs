//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Xarial.XCad.Enums;

namespace Xarial.XCad.Base
{
    /// <summary>
    /// Defines the specification of the tooltip used in the <see cref="IXApplication.ShowTooltip(ITooltipSpec)"/>
    /// 定义 <see cref="IXApplication.ShowTooltip(ITooltipSpec)"/> 中使用的工具提示规范
    /// </summary>
    public interface ITooltipSpec
    {
        /// <summary>
        /// Title of tooltip
        /// 工具提示的标题
        /// </summary>
        string Title { get; }

        /// <summary>
        /// Message to show in tooltip
        /// 工具提示中显示的消息
        /// </summary>
        string Message { get; }

        /// <summary>
        /// Position of the tooltip
        /// 工具提示的位置
        /// </summary>
        Point Position { get; }

        /// <summary>
        /// Position of tooltip arrow
        /// 工具提示筛头的位置
        /// </summary>
        TooltipArrowPosition_e ArrowPosition { get; }
    }
}
