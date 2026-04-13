//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.SolidWorks.Utils
{
    /// <summary>
    /// 常用 HRESULT 常量。
    /// </summary>
    internal static class HResult
    {
        /// <summary>操作成功。</summary>
        internal const int S_OK = 0;
        /// <summary>操作返回假/无结果（非致命）。</summary>
        internal const int S_FALSE = 1;
    }
}
