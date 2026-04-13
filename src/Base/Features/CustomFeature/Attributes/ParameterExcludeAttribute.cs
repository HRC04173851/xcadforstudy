//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.Features.CustomFeature.Attributes
{
    /// <summary>
    /// Indicates that this property should not be considered as macro feature parameter
    /// 指示该属性不参与宏特征参数序列化/反序列化
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ParameterExcludeAttribute : Attribute
    {
    }
}
