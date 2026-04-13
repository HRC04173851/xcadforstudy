//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Drawing;
using System.Drawing.Imaging;
using Xarial.XCad.UI;

namespace Xarial.XCad.Toolkit.Base
{
    /// <summary>
    /// Custom handler for the image replace function
    /// <para>图像像素替换函数的自定义处理委托。</para>
    /// </summary>
    /// <param name="r">Red component of pixel<para>像素红色分量。</para></param>
    /// <param name="g">Green component of pixel<para>像素绿色分量。</para></param>
    /// <param name="b">Blue component of pixel<para>像素蓝色分量。</para></param>
    /// <param name="a">Alpha component of pixel<para>像素透明度分量。</para></param>
    public delegate void ColorMaskDelegate(ref byte r, ref byte g, ref byte b, ref byte a);

    /// <summary>
    /// Descriptor for the icon of a specific type
    /// <para>特定图标规格（尺寸、来源、掩码等）的描述接口。</para>
    /// </summary>
    public interface IIconSpec 
    {
        /// <summary>
        /// Base name of the icon
        /// <para>图标基础名称。</para>
        /// </summary>
        string BaseName { get; }

        /// <summary>
        /// Original image of the icon
        /// <para>图标原始图像。</para>
        /// </summary>
        IXImage SourceImage { get; }

        /// <summary>
        /// Required size of the icon
        /// <para>图标目标尺寸。</para>
        /// </summary>
        Size TargetSize { get; }

        /// <summary>
        /// Handler for the mask
        /// <para>图像掩码处理委托。</para>
        /// </summary>
        ColorMaskDelegate Mask { get; }

        /// <summary>
        /// Image margin
        /// <para>图像边距。</para>
        /// </summary>
        int Margin { get; }
    }

    /// <inheritdoc/>
    public class IconSpec : IIconSpec
    {
        /// <summary>
        /// Generates the file name for the icon
        /// <para>为图标生成建议文件名。</para>
        /// </summary>
        /// <param name="baseName">Base name for the icon<para>图标基础名称。</para></param>
        /// <param name="targetSize">Required icon size<para>目标图标尺寸。</para></param>
        /// <param name="format">Format<para>图像格式。</para></param>
        /// <returns>Suggested file name<para>建议使用的文件名。</para></returns>
        public static string CreateFileName(string baseName, Size targetSize, IconImageFormat_e format)
        {
            if (string.IsNullOrEmpty(baseName))
            {
                baseName = Guid.NewGuid().ToString();
            }

            string ext;

            switch (format)
            {
                case IconImageFormat_e.Bmp:
                    ext = "bmp";
                    break;

                case IconImageFormat_e.Png:
                    ext = "png";
                    break;

                case IconImageFormat_e.Jpeg:
                    ext = "jpg";
                    break;

                default:
                    throw new NotSupportedException();
            }

            return $"{baseName}_{targetSize.Width}x{targetSize.Height}.{ext}";
        }

        /// <inheritdoc/>
        public string BaseName { get; }

        /// <inheritdoc/>
        public IXImage SourceImage { get; }

        /// <inheritdoc/>
        public Size TargetSize { get; }

        /// <inheritdoc/>
        public ColorMaskDelegate Mask { get; }

        /// <inheritdoc/>
        public int Margin { get; }

        /// <summary>
        /// Icon size constructor with source image, target size and optional base name
        /// <para>使用源图像、目标尺寸与可选基础名称初始化图标规格。</para>
        /// </summary>
        /// <param name="srcImage">Source image<para>源图像。</para></param>
        /// <param name="targetSize">Target size of the image<para>目标图像尺寸。</para></param>
        /// <param name="margin">Margin of the icon<para>图标边距。</para></param>
        /// <param name="baseName">Base name of the image<para>图像基础名称。</para></param>
        public IconSpec(IXImage srcImage, Size targetSize, int margin = 0, string baseName = "")
        {
            SourceImage = srcImage;
            TargetSize = targetSize;
            Margin = margin;

            BaseName = baseName;
        }

        public IconSpec(IXImage srcImage, Size targetSize, ColorMaskDelegate mask, int margin = 0, string baseName = "")
            : this(srcImage, targetSize, margin, baseName)
        {
            Mask = mask;
        }
    }
}