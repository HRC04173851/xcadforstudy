// -*- coding: utf-8 -*-
// src/Base/IHasName.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 名称接口，表示具有名称属性的对象，继承自 IXObject，支持获取和设置对象名称。
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
