// -*- coding: utf-8 -*-
// src/Toolkit/PageBuilder/Exceptions/MissingMetadataException.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件实现异常类 MissingMetadataException。
// 当未能找到控件所需的元数据时抛出。
// 用于标识元数据依赖配置缺失问题。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.UI.PropertyPage.Base;

namespace Xarial.XCad.Toolkit.PageBuilder.Exceptions
{
    /// <summary>
    /// Exception indicates that required metadata could not be found for a control.
    /// <para>异常指示未能找到控件所需的元数据。</para>
    /// </summary>
    public class MissingMetadataException : Exception
    {
        /// <summary>
        /// Initializes exception for missing metadata tag.
        /// <para>为缺失元数据标签初始化异常。</para>
        /// </summary>
        public MissingMetadataException(object tag, IControlDescriptor ctrlDesc) 
            : base($"Failed to find the metadata '{tag?.ToString()}' for {ctrlDesc.DisplayName}")
        {
        }
    }
}
