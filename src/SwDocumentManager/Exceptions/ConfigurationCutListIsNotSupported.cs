// -*- coding: utf-8 -*-
// src/SwDocumentManager/Exceptions/ConfigurationCutListIsNotSupported.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 当 SOLIDWORKS 文件版本不支持按该方式提取切割清单时抛出的异常。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Exceptions;

namespace Xarial.XCad.SwDocumentManager.Exceptions
{
    /// <summary>
    /// Exception indicates that cut-lists cannot be exctrated for this version of the SOLIDWORKS file
    /// 表示当前 SOLIDWORKS 文件版本不支持按该方式提取切割清单。
    /// </summary>
    public class ConfigurationCutListIsNotSupported : NotSupportedException, IUserException
    {
        public ConfigurationCutListIsNotSupported() 
            : base("Cut-lists can only be extracted from the active configuration for files saved in 2018 or older") 
        {
        }
    }
}
