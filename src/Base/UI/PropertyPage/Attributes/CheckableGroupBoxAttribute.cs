// -*- coding: utf-8 -*-
// src/Base/UI/PropertyPage/Attributes/CheckableGroupBoxAttribute.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 指示该分组应带复选框，允许用户通过复选框切换分组框的展开/折叠状态。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.UI.PropertyPage.Base;

namespace Xarial.XCad.UI.PropertyPage.Attributes
{
    /// <summary>
    /// Indicates that this group should have a check box
    /// 指示该分组应带复选框
    /// </summary>
    public interface ICheckableGroupBoxAttribute : IAttribute
    {
        /// <summary>
        /// Reference to property which defines the toggle state 
        /// </summary>
        object ToggleMetadataTag { get; }
    }

    /// <inheritdoc/>
    public class CheckableGroupBoxAttribute : Attribute, IHasMetadataAttribute, ICheckableGroupBoxAttribute
    {
        /// <inheritdoc/>
        public object LinkedMetadataTag => ToggleMetadataTag;

        /// <inheritdoc/>
        public object ToggleMetadataTag { get; }

        /// <inheritdoc/>
        public bool HasMetadata => true;

        /// <inheritdoc/>
        public object StaticValue => throw new NotSupportedException();

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="toggleMetadataTag">Reference to property which defines the toggle state</param>
        public CheckableGroupBoxAttribute(object toggleMetadataTag)
        {
            ToggleMetadataTag = toggleMetadataTag;
        }
    }
}
