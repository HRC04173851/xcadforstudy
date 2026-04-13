//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.Base.Enums
{
    /// <summary>
    /// Specifies the buttons to display in <see cref="IXApplication.ShowMessageBox(string, MessageBoxIcon_e, MessageBoxButtons_e)"/>
    /// 指定在 <see cref="IXApplication.ShowMessageBox(string, MessageBoxIcon_e, MessageBoxButtons_e)"/> 中显示的按钟
    /// </summary>
    public enum MessageBoxButtons_e
    {
        /// <summary>
        /// OK button only
        /// 仅显示 OK 按钟
        /// </summary>
        Ok,

        /// <summary>
        /// OK and Cancel buttons
        /// 显示 OK 和取消按钟
        /// </summary>
        OkCancel,

        /// <summary>
        /// Yes and No buttons
        /// 显示是和否按钟
        /// </summary>
        YesNo,

        /// <summary>
        /// Yes, No and Cancel buttons
        /// 显示是、否和取消按钟
        /// </summary>
        YesNoCancel
    }
}
