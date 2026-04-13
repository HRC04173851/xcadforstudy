//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad
{
    /// <summary>
    /// Indicates that object has name
    /// 表示对象具有名称属性
    /// </summary>
    public interface IHasName : IXObject
    {
        /// <summary>
        /// Name of this element
        /// 此元素的名称
        /// </summary>
        string Name { get; set; }
    }
}
