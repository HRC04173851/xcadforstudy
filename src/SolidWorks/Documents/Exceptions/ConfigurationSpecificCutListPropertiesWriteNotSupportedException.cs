// -*- coding: utf-8 -*-
// src/SolidWorks/Documents/Exceptions/ConfigurationSpecificCutListPropertiesWriteNotSupportedException.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本异常在尝试修改配置特定的切割清单属性时抛出。
// 切割清单属性修改必须在活动配置中进行，
// 配置特定的切割清单属性不支持写入操作。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Exceptions;

namespace Xarial.XCad.SolidWorks.Documents.Exceptions
{
    public class ConfigurationSpecificCutListPropertiesWriteNotSupportedException : NotSupportedException, IUserException
    {
        public ConfigurationSpecificCutListPropertiesWriteNotSupportedException() 
            : base("Modifying configuration specific cut-list properties is not supported") 
        {
        }
    }
}
