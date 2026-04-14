// -*- coding: utf-8 -*-
// src/Base/UI/IXImage.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义图像资源接口 IXImage 和基础实现类 BaseImage。
// 提供图像字节数据的访问接口，通常用于图标等图像资源的表示。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.UI
{
    /// <summary>
    /// Represents the image
    /// 表示图像资源
    /// </summary>
    /// <remarks>This is usually used for icons. 通常用于图标资源。</remarks>
    public interface IXImage
    {
        /// <summary>
        /// Byte data of this image
        /// 图像的字节数据
        /// </summary>
        byte[] Buffer { get; }
    }

    /// <summary>
    /// Represents base image
    /// 表示基础图像实现
    /// </summary>
    public class BaseImage : IXImage
    {
        /// <inheritdoc/>
        public byte[] Buffer { get; }

        /// <summary>
        /// Base image constructor
        /// </summary>
        /// <param name="buffer">Image buffer</param>
        public BaseImage(byte[] buffer) 
        {
            Buffer = buffer;
        }
    }
}
