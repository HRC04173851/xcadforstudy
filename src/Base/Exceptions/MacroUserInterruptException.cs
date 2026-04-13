//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.Exceptions
{
    /// <summary>
    /// Exception indicates that macro can be forcibly terminated by the user
    /// 此异常表示宏可以被用户强制终止
    /// </summary>
    public class MacroUserInterruptException : MacroRunFailedException, IUserException
    {
        /// <summary>
        /// Default constructor
        /// 默认构造函数
        /// </summary>
        /// <param name="path">Path to the macro 宏的路径</param>
        /// <param name="errorCode">CAD specific error code CAD特定的错误代码</param>
        public MacroUserInterruptException(string path, int errorCode)
            : base(path, errorCode, "User interrupt")
        {
        }
    }
}
