// -*- coding: utf-8 -*-
// src/SwDocumentManager/Exceptions/ConfigurationSpecificCutListPropertiesWriteNotSupportedException.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 当代码尝试写入非活动配置的切割清单属性时抛出的异常，Document Manager 不支持此操作。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Exceptions;

namespace Xarial.XCad.SwDocumentManager.Exceptions
{
    /// <summary>
    /// Raised when code attempts to write cut-list properties for a non-active configuration.
    /// 当代码尝试写入非活动配置的切割清单属性时抛出该异常。
    /// </summary>
    public class ConfigurationSpecificCutListPropertiesWriteNotSupportedException : NotSupportedException, IUserException
    {
        public ConfigurationSpecificCutListPropertiesWriteNotSupportedException()
            : base("Modifying configuration specific cut-list properties is not supported. Instead modify the properties in active configuration only")
        {
        }
    }
}
