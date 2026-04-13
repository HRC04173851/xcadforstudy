//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

namespace Xarial.XCad.UI.Commands.Structures
{
    /// <summary>
    /// State of the command within the <see cref="IXCommandGroup"/>
    /// <see cref="IXCommandGroup"/> 中命令状态
    /// </summary>
    public class CommandState
    {
        /// <summary>
        /// Is command enabled
        /// 命令是否启用
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Is command checked
        /// 命令是否选中
        /// </summary>
        public bool Checked { get; set; }
    }
}