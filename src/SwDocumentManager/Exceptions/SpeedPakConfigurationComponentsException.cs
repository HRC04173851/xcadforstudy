// -*- coding: utf-8 -*-
// src/SwDocumentManager/Exceptions/SpeedPakConfigurationComponentsException.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 当调用方尝试从 SpeedPak 配置中枚举组件时抛出的异常，Document Manager 无法为其提供完整组件树。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xarial.XCad.Exceptions;

namespace Xarial.XCad.SwDocumentManager.Exceptions
{
    /// <summary>
    /// Raised when the caller tries to enumerate components from a SpeedPak configuration.
    /// 当调用方尝试从 SpeedPak 配置中枚举组件时抛出该异常。
    /// </summary>
    public class SpeedPakConfigurationComponentsException : Exception, IUserException
    {
        public SpeedPakConfigurationComponentsException() : base("Components cannot be extracted from the SpeedPak configuration")
        {
        }
    }
}
