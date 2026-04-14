// -*- coding: utf-8 -*-
// Commands/Exceptions/ComControlHostException.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// COM控件托管异常，当无法从指定的ProgId创建COM控件时抛出此异常。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.SolidWorks.UI.Commands.Exceptions
{
    public class ComControlHostException : Exception
    {
        public ComControlHostException(string progId) 
            : base($"Failed to create COM control from '{progId}'. Make sure that COM component is properly registered") 
        {
        }
    }
}
