// -*- coding: utf-8 -*-
// src/Toolkit/PageBuilder/Exceptions/DuplicateMetadataTagException.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件实现异常类 DuplicateMetadataTagException。
// 当同一个元数据标签被重复注册时抛出。
// 用于防止元数据标签冲突。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.Toolkit.PageBuilder.Exceptions
{
    /// <summary>
    /// Exception indicates that metadata tag is registered more than once.
    /// <para>异常指示同一个元数据标签被重复注册。</para>
    /// </summary>
    public class DuplicateMetadataTagException : Exception
    {
        /// <summary>
        /// Initializes exception for duplicate metadata tag.
        /// <para>为重复元数据标签初始化异常。</para>
        /// </summary>
        public DuplicateMetadataTagException(object tag) : base($"'{tag?.ToString()}' tag already assigned to metadata") 
        {
        }
    }
}
