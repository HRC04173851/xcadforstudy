//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using Xarial.XCad.UI.PropertyPage.Base;

namespace Xarial.XCad.UI.PropertyPage.Attributes
{
    /// <summary>
    /// Attribute for assigning tag to control binding
    /// 用于给控件绑定分配标签的特性
    /// </summary>
    public class ControlTagAttribute : Attribute, IControlTagAttribute
    {   
        /// <inheritdoc/>
        public object Tag { get; }

        public ControlTagAttribute(object tag)
        {
            Tag = tag;
        }
    }
}