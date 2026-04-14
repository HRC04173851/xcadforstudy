// -*- coding: utf-8 -*-
// IPropertiesOwner.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义属性所有者接口，表示该实体包含自定义属性集合。
// 实现此接口的对象可以通过 Properties 属性访问其关联的属性仓储。
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
