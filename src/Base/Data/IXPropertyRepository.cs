// -*- coding: utf-8 -*-
// IXPropertyRepository.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义属性集合仓储接口，继承自 IXRepository，用于管理自定义属性集合。
// 作为 IXProperty 对象的容器，提供标准的仓储操作接口。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Base;

namespace Xarial.XCad.Data
{
    /// <summary>
    /// Represents the collection of properties
    /// 表示属性集合仓储
    /// </summary>
    public interface IXPropertyRepository : IXRepository<IXProperty>
    {
    }
}
