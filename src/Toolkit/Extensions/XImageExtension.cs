// -*- coding: utf-8 -*-
// src/Toolkit/Extensions/XImageExtension.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件为 IXImage 接口提供扩展方法。
// 实现将 IXImage 转换为标准 System.Drawing.Image 的便捷方法。
// 通过内存流读取缓冲区数据，支持空值安全处理。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xarial.XCad.UI;

namespace Xarial.XCad.Toolkit.Extensions
{
    /// <summary>
    /// Additional methods for <see cref="IXImage"/>
    /// <para><see cref="IXImage"/> 的扩展辅助方法。</para>
    /// </summary>
    public static class XImageExtension
    {
        /// <summary>
        /// Tries converts <see cref="IXImage"/> to <see cref="Image"/>
        /// <para>尝试将 <see cref="IXImage"/> 转换为 <see cref="Image"/>。</para>
        /// </summary>
        /// <param name="img">Source image wrapper<para>源图像包装对象。</para></param>
        /// <returns>Imgage or null<para>转换成功返回图像对象，否则返回 `null`。</para></returns>
        public static Image ToImage(this IXImage img) 
        {
            try
            {
                if (img != null && img.Buffer != null)
                {
                    using (var memStr = new MemoryStream(img.Buffer))
                    {
                        memStr.Seek(0, SeekOrigin.Begin);
                        return Image.FromStream(memStr);
                    }
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
