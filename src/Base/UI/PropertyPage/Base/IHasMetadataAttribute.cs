// -*- coding: utf-8 -*-
// src/Base/UI/PropertyPage/Base/IHasMetadataAttribute.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 指示该绑定使用元数据，提供元数据标签和静态值访问。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.UI.PropertyPage.Base
{
    /// <summary>
    /// Indicates that this binding uses metadata
    /// 指示该绑定使用元数据
    /// </summary>
    public interface IHasMetadataAttribute : IAttribute
    {
        /// <summary>
        /// True if the referenced property has metadata
        /// </summary>
        bool HasMetadata { get; }

        /// <summary>
        /// Tag of metadata linked to <see cref="IMetadataAttribute.Tag"/>
        /// </summary>
        object LinkedMetadataTag { get; }

        /// <summary>
        /// Static value of the metadata
        /// </summary>
        object StaticValue { get; }
    }
}
