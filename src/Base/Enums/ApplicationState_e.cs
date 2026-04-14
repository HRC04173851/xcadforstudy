// -*- coding: utf-8 -*-
// src/Base/Enums/ApplicationState_e.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义应用程序运行状态枚举，包括隐藏、后台、静默和安全模式等选项，用于控制xCAD应用程序的启动和运行行为。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.Enums
{
    /// <summary>
    /// Represents the state of the application
    /// 表示应用程序状态
    /// </summary>
    [Flags]
    public enum ApplicationState_e
    {
        /// <summary>
        /// Default state
        /// </summary>
        Default = 0,

        /// <summary>
        /// Application window is not visible
        /// </summary>
        Hidden = 1,

        /// <summary>
        /// Application runs in the background
        /// </summary>
        Background = 2,

        /// <summary>
        /// Application runs silently
        /// </summary>
        Silent = 4,

        /// <summary>
        /// Application runs in the safe mode
        /// </summary>
        Safe = 8
    }
}
