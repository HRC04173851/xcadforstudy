//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad
{
    /// <summary>
    /// Enables the display of progress bar and status
    /// 启用进度条和状态显示
    /// </summary>
    public interface IXProgress : IDisposable, IProgress<double>
    {
        /// <summary>
        /// Sets status of the operation
        /// 设置操作的状态文本
        /// </summary>
        /// <param name="status">Status messae 状态消息</param>
        void SetStatus(string status);
    }
}
