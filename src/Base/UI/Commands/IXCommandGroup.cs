//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
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