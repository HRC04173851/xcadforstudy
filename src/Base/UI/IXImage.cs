//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
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
