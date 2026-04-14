// -*- coding: utf-8 -*-
// src/SolidWorks/Data/Exceptions/CustomPropertyMissingException.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义当访问不存在的自定义属性时抛出的异常，提示用户使用PreCreate方法创建新属性。
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

namespace Xarial.XCad.SolidWorks.Data.Exceptions
{
    public class CustomPropertyMissingException : Exception
    {
        public CustomPropertyMissingException(string name) 
            : base($"'{name}' property doesn't exist. Use '{nameof(SwCustomPropertiesCollection.PreCreate)}' method instead to create new property") 
        {
        }
    }
}
