// -*- coding: utf-8 -*-
// src/Base/XMacroExtension.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 宏扩展方法类，为 IXMacro 接口提供便捷的运行重载方法，支持使用默认入口点或指定入口点运行宏。
//*********************************************************************

using System.Linq;
using Xarial.XCad.Enums;
using Xarial.XCad.Exceptions;
using Xarial.XCad.Structures;

namespace Xarial.XCad
{
    /// <summary>
    /// Additional methods of <see cref="IXMacro"/>
    /// <see cref="IXMacro"/> 的扩展方法
    /// </summary>
    public static class XMacroExtension
    {
        /// <summary>
        /// Run macro with default entry point and default options
        /// 使用默认入口点和默认选项运行宏
        /// </summary>
        /// <param name="macro">Macro to run 要运行的宏</param>
        public static void Run(this IXMacro macro)
            => Run(macro, MacroRunOptions_e.Default);

        /// <summary>
        /// Run macro with default entry point and specified options
        /// 使用默认入口点和指定选项运行宏
        /// </summary>
        /// <param name="macro">Macro to run 要运行的宏</param>
        /// <param name="opts">macro options 宏选项</param>
        public static void Run(this IXMacro macro, MacroRunOptions_e opts)
        {
            if (macro.EntryPoints?.Any() == true)
            {
                macro.Run(macro.EntryPoints.First(), opts);
            }
            else 
            {
                throw new MacroRunFailedException(macro.Path, -1, "Macro contains no entry points");
            }
        }

        /// <summary>
        /// Run macro with specirfied entry point and default options
        /// 使用指定入口点和默认选项运行宏
        /// </summary>
        /// <param name="macro">Macro to run 要运行的宏</param>
        /// <param name="entryPoint">Entry point 入口点</param>
        public static void Run(this IXMacro macro, MacroEntryPoint entryPoint)
            => macro.Run(entryPoint, MacroRunOptions_e.Default);
    }
}
