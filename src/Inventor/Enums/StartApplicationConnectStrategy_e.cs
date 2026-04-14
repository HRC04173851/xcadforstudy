// -*- coding: utf-8 -*-
// src/Inventor/Enums/StartApplicationConnectStrategy_e.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 应用程序启动连接策略枚举。
// 定义与Inventor应用程序建立连接的不同策略选项。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xarial.XCad.Inventor.Enums
{
    [Flags]
    public enum StartApplicationConnectStrategy_e
    {
        Default = 0,
        AllowCreatingTempTokenDocuments = 1,
        WaitUntilFullyLoaded
    }
}
