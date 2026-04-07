using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xarial.XCad.UI;

namespace SwAddInExample
{
    // Adapter that wraps a System.Drawing.Image and exposes it as an IXImage (byte buffer)
    // 中文：适配器类，将 System.Drawing.Image 封装并作为 IXImage（字节缓冲区）暴露给 xCAD 框架
    // xCAD uses IXImage throughout its API for icons, images, and drawing view snapshots
    // 中文：xCAD 在图标、图像和工程图视图快照等 API 中统一使用 IXImage 接口
    internal class XDrawingImage : IXImage
    {
        /// <inheritdoc/>
        /// <para>中文：图像数据的字节缓冲区，以指定的图像格式（默认 PNG）编码</para>
        public byte[] Buffer { get; }

        // Convenience constructor: converts the image using the default PNG format
        // 中文：便捷构造函数：使用默认 PNG 格式将图像转换为字节缓冲区
        internal XDrawingImage(Image img) : this(img, ImageFormat.Png)
        {
        }

        // Constructor: converts the given image to a byte buffer using the specified format
        // 中文：构造函数：使用指定格式将给定图像转换为字节缓冲区
        internal XDrawingImage(Image img, ImageFormat format)
        {
            Buffer = ImageToByteArray(img, format);
        }

        // Helper method: saves the image into a MemoryStream and returns it as a byte array
        // 中文：辅助方法：将图像保存到内存流中并以字节数组形式返回
        private byte[] ImageToByteArray(Image bmp, ImageFormat format)
        {
            // Use a MemoryStream to avoid writing to disk; 'using' ensures it is disposed afterward
            // 中文：使用 MemoryStream 避免写入磁盘；using 语句确保之后自动释放资源
            using (var ms = new MemoryStream())
            {
                bmp.Save(ms, format);
                return ms.ToArray();
            }
        }
    }
}
