// -*- coding: utf-8 -*-
// src/Inventor/Utils/WinAPI.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// Windows API工具类。
// 提供Windows原生API的P/Invoke声明，如窗口进程ID获取等底层操作。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Xarial.XCad.Inventor.Utils
{
    internal static class WinAPI
    {
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern uint GetWindowThreadProcessId(IntPtr hWnd, out int processId);
    }
}
