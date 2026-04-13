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
    /// Result of the message box
    /// 消息框的返回结果
    /// </summary>
    public enum MessageBoxResult_e
    {
        /// <summary>
        /// OK button is clicked
        /// 点击了 OK 按钟
        /// </summary>
        Ok,

        /// <summary>
        /// Yes button is clicked
        /// 点击了是按钟
        /// </summary>
        Yes,

        /// <summary>
        /// No button is clicked
        /// 点击了否按钟
        /// </summary>
        No,

        /// <summary>
        /// Cancel button is clicked
        /// 点击了取消按钟
        /// </summary>
        Cancel
    }
}
