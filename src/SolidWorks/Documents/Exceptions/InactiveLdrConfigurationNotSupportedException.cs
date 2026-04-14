// -*- coding: utf-8 -*-
// src/SolidWorks/Documents/Exceptions/InactiveLdrConfigurationNotSupportedException.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本异常在尝试访问大设计审查（LDR）模式下打开的装配体的非活动配置时抛出。
// LDR 模式下仅支持访问活动配置，非活动配置的组件数据无法加载。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Exceptions;

namespace Xarial.XCad.SolidWorks.Documents.Exceptions
{
    public class InactiveLdrConfigurationNotSupportedException : NotSupportedException, IUserException
    {
        public InactiveLdrConfigurationNotSupportedException() 
            : base("Inactive configuration of assembly opened in Large Design Review model is not supported and cannot be loaded")
        {
        }
    }
}
