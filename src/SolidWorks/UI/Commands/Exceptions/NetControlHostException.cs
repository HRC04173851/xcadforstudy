// -*- coding: utf-8 -*-
// Commands/Exceptions/NetControlHostException.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// .NET控件托管异常，当无法在任务窗格中托管.NET控件时抛出此异常。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.SolidWorks.UI.Commands.Exceptions
{
    public class NetControlHostException : Exception
    {
        public NetControlHostException(IntPtr handle) : base($"Failed to host .NET control (handle {handle}) in task pane") 
        {
        }
    }
}
