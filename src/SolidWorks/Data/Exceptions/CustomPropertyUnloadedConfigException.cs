// -*- coding: utf-8 -*-
// src/SolidWorks/Data/Exceptions/CustomPropertyUnloadedConfigException.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义当尝试向未加载的配置添加自定义属性时抛出的异常，提示用户先激活配置。
//*********************************************************************

//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Exceptions;

namespace Xarial.XCad.SolidWorks.Data.Exceptions
{
    public class CustomPropertyUnloadedConfigException : Exception, IUserException
    {
        public CustomPropertyUnloadedConfigException() 
            : base("Custom property is not added to unloaded configuration. Try activate configuration before adding the property") 
        {
        }
    }
}
