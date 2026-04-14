// -*- coding: utf-8 -*-
// src/SolidWorks/Documents/Exceptions/ConfigurationSpecificCutListNotSupportedException.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本异常在尝试访问配置特定的切割清单时抛出。
// 切割清单（Cut List）通常需要从活动配置中访问，
// 配置特定的切割清单不受支持，本异常防止无效的访问操作。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Exceptions;

namespace Xarial.XCad.SolidWorks.Documents.Exceptions
{
    public class ConfigurationSpecificCutListNotSupportedException : NotSupportedException, IUserException
    {
        public ConfigurationSpecificCutListNotSupportedException() 
            : base("Configuration specific cut-lists are not supported. Instead access cut-lists from an active configuration") 
        {
        }
    }
}
