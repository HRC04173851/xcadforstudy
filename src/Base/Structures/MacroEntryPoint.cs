//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.Structures
{
    /// <summary>
    /// Represents the entty point of the macro
    /// 表示宏的入口点
    /// </summary>
    public class MacroEntryPoint
    {
        /// <summary>
        /// Module name for the entry point
        /// 入口点所在的模块名称
        /// </summary>
        public string ModuleName { get; }

        /// <summary>
        /// Name of the procedure defined as an entry point
        /// 定义为入口点的过程名称
        /// </summary>
        public string ProcedureName { get; }

        /// <summary>
        /// Default constructor for entry point
        /// 入口点的默认构造函数
        /// </summary>
        /// <param name="moduleName">Module name 模块名称</param>
        /// <param name="procName">Procedure name 过程名称</param>
        public MacroEntryPoint(string moduleName, string procName)
        {
            ModuleName = moduleName;
            ProcedureName = procName;
        }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(ModuleName))
            {
                return $"{ModuleName}.{ProcedureName}";
            }
            else
            {
                return ProcedureName;
            }
        }
    }
}
