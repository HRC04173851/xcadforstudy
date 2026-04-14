// -*- coding: utf-8 -*-
// src/Base/IXMacro.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 宏接口，表示可执行的宏对象，包含宏文件路径、入口点列表以及运行宏的方法。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Enums;
using Xarial.XCad.Structures;

namespace Xarial.XCad
{
    /// <summary>
    /// Represents the macro
    /// 表示宏（Macro）
    /// </summary>
    public interface IXMacro
    {
        /// <summary>
        /// Path to the macro
        /// 宏的文件路径
        /// </summary>
        string Path { get; }

        /// <summary>
        /// Available entry points of this macro
        /// 此宏可用的入口点列表
        /// </summary>
        MacroEntryPoint[] EntryPoints { get; }

        /// <summary>
        /// Run the macro
        /// 运行宏
        /// </summary>
        /// <param name="entryPoint">Entry point 入口点</param>
        /// <param name="opts">Options 运行选项</param>
        void Run(MacroEntryPoint entryPoint, MacroRunOptions_e opts);
    }
}
