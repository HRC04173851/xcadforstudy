//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xarial.XCad.UI.PropertyPage.Enums
{
    /// <summary>
    /// Automatic bitmap effects which can be applied to UI controls
    /// 可应用于 UI 控件的自动位图效果
    /// </summary>
    [Flags]
    public enum BitmapEffect_e
    {
        /// <summary>
        /// No effect
        /// </summary>
        None = 0,

        /// <summary>
        /// Grayscale bitmap
        /// </summary>
        Grayscale = 1,

        /// <summary>
        /// Semi-transparent bitmap
        /// </summary>
        Transparent = 2
    }
}
