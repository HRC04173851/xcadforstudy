//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Geometry;

namespace Xarial.XCad.Features.CustomFeature.Delegates
{
    /// <summary>
    /// Assigns the custom color to the preview body
    /// 为预览几何体分配自定义颜色
    /// </summary>
    /// <param name="body">Body to assign preview to（预览体）</param>
    /// <param name="color">Color of the preview body（预览颜色）</param>
    public delegate void AssignPreviewBodyColorDelegate(IXBody body, out System.Drawing.Color color);
}
