// -*- coding: utf-8 -*-
// Exceptions/AppStartCancelledByUserException.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 应用程序启动被用户取消时抛出的异常
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.SolidWorks.Exceptions
{
    public class AppStartCancelledByUserException : Exception
    {
        public AppStartCancelledByUserException() : base("Application start is cancelled by user") 
        {
        }
    }
}
