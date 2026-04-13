//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using Xarial.XCad.UI;

namespace Xarial.XCad.SolidWorks.Utils
{
    /// <summary>
    /// IPictureDisp 转换辅助类。
    /// 将 SolidWorks 返回的 COM 图片对象转换为 xCAD `IXImage`。
    /// </summary>
    internal static class PictureDispUtils
    {
        internal static IXImage PictureDispToXImage(object pictDisp) 
        {
            if (pictDisp == null)
            {
                throw new NullReferenceException("Failed to extract IPictureDisp from the document");
            }

            var getPictureFromIPictureFunc = typeof(System.Windows.Forms.AxHost)
                .GetMethod("GetPictureFromIPicture", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
           
            var bmp = getPictureFromIPictureFunc.Invoke(null, new object[] { pictDisp }) as Bitmap;

            return new XDrawingImage(bmp);
        }
    }
}
