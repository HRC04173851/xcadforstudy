//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.Enums
{
    /// <summary>
    /// Options for running the macro via <see cref="IXMacro.Run(Structures.MacroEntryPoint, MacroRunOptions_e)"/>
    /// 运行宏时的选项
    /// </summary>
    public enum MacroRunOptions_e
    {
        /// <summary>
        /// Default options
        /// 默认选项
        /// </summary>
        Default,

        /// <summary>
        /// Unload macro from memory after run
        /// 运行后从内存卸载宏
        /// </summary>
        UnloadAfterRun
    }
}
