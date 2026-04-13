//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.Data
{
    /// <summary>
    /// Specifies that this entity has properties
    /// 指示该实体包含属性集合
    /// </summary>
    public interface IPropertiesOwner
    {
        /// <summary>
        /// Collection of properties
        /// 属性集合仓储
        /// </summary>
        IXPropertyRepository Properties { get; }
    }
}
