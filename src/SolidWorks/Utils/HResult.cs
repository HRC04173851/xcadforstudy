// -*- coding: utf-8 -*-
// src/SolidWorks/Utils/HResult.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义 COM HRESULT 常用常量值，如 S_OK 和 S_FALSE，用于 COM 接口返回值处理。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.SolidWorks.Utils
{
    internal static class HResult
    {
        internal const int S_OK = 0;
        internal const int S_FALSE = 1;
    }
}
