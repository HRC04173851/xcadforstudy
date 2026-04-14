// -*- coding: utf-8 -*-
// src/Base/UI/PropertyPage/Attributes/MetadataAttribute.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 将该属性标记为元数据，用于在属性页控件间共享和传递数据信息。
//*********************************************************************

using System;
using Xarial.XCad.UI.PropertyPage.Base;

namespace Xarial.XCad.UI.PropertyPage.Attributes
{
    /// <inheritdoc/>
    [AttributeUsage(AttributeTargets.Property)]
    public class MetadataAttribute : Attribute, IMetadataAttribute
    {
        /// <inheritdoc/>
        public object Tag { get; }

        /// <summary>
        /// Marks this property as metadata
    /// 将该属性标记为元数据
        /// </summary>
        /// <param name="tag">Tag of the metadata</param>
        public MetadataAttribute(object tag)
        {
            Tag = tag;
        }
    }
}