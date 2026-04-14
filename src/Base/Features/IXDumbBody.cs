// -*- coding: utf-8 -*-
// src/Base/Features/IXDumbBody.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义承载哑实体（无历史特征体）的特征接口。
// 哑实体是不基于特征树的几何体，直接包含实体数据。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Geometry;

namespace Xarial.XCad.Features
{
    /// <summary>
    /// Represents the feature which holds the dumb body
    /// 表示承载哑实体（无历史特征体）的特征
    /// </summary>
    public interface IXDumbBody : IXFeature
    {
        /// <summary>
        /// Body geometry of the feature
        /// 该特征对应的几何体
        /// </summary>
        IXBody BaseBody { get; set; }
    }
}
