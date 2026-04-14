// -*- coding: utf-8 -*-
// src/Base/UI/PropertyPage/Base/IMetadataAttribute.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 表示控件使用的元数据特性接口，提供元数据标签访问。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.UI.PropertyPage.Base
{
    /// <summary>
    /// Represents the custom metadata which is used by controls
    /// 表示控件使用的元数据特性接口
    /// </summary>
    public interface IMetadataAttribute : IAttribute
    {
        /// <summary>
        /// Tag of the metadata
        /// 元数据标签
        /// </summary>
        object Tag { get; }
    }
}
