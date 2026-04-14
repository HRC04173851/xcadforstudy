// -*- coding: utf-8 -*-
// src/Base/Annotations/IXDimensionRepository.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义尺寸标注的仓储接口，继承自通用仓储接口，提供尺寸标注的查询、添加和删除等管理功能。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Annotations;
using Xarial.XCad.Base;

namespace Xarial.XCad.Annotations
{
    /// <summary>
    /// Collection of dimensions
    /// 尺寸标注集合
    /// </summary>
    public interface IXDimensionRepository : IXRepository<IXDimension>
    {
    }
}
