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

namespace Xarial.XCad.Enums
{
    /// <summary>
    /// Justification of the text
    /// 文本对齐方式
    /// </summary>
    public enum TextJustification_e
    {
        /// <summary>
        /// Default justification
        /// 默认对齐
        /// </summary>
        None,

        /// <summary>
        /// Align to the left
        /// 左对齐
        /// </summary>
        Left,

        /// <summary>
        /// Align to the center
        /// 居中对齐
        /// </summary>
        Center,

        /// <summary>
        /// Align to the right
        /// 右对齐
        /// </summary>
        Right
    }
}
