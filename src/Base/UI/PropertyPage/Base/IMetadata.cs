// -*- coding: utf-8 -*-
// src/Base/UI/PropertyPage/Base/IMetadata.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 表示绑定项附加的自定义元数据，包含标签、值和变更通知。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.UI.PropertyPage.Base
{
    /// <summary>
    /// Represents the custom metadata which is attached to binding
    /// 表示绑定项附加的自定义元数据
    /// </summary>
    public interface IMetadata
    {
        /// <summary>
        /// Tag of this metadata
        /// 元数据标签
        /// </summary>
        object Tag { get; }

        /// <summary>
        /// Value associated with the metadata
        /// 元数据关联值
        /// </summary>
        object Value { get; set; }

        /// <summary>
        /// Notifies when metadata is changed
        /// </summary>
        event Action<IMetadata, object> Changed;
    }
}
