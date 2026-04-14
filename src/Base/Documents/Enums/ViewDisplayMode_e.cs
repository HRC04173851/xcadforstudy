// -*- coding: utf-8 -*-
// src/Base/Documents/Enums/ViewDisplayMode_e.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 表示工程图视图和模型视图的显示模式，包括线框、隐藏线可见、隐藏线消除和着色模式。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xarial.XCad.Documents.Enums
{
    /// <summary>
    /// Represents the display mode of the <see cref="IXDrawingView"/> and <see cref="IXModelView"/>
    /// 表示 <see cref="IXDrawingView"/> 与 <see cref="IXModelView"/> 的显示模式
    /// </summary>
    public enum ViewDisplayMode_e
    {
        /// <summary>
        /// Wireframe
        /// 线框显示
        /// </summary>
        Wireframe,

        /// <summary>
        /// Hidden Lines Visible
        /// 显示隐藏线
        /// </summary>
        HiddenLinesVisible,

        /// <summary>
        /// Hidden Lines Removed
        /// 隐藏线消除
        /// </summary>
        HiddenLinesRemoved,

        /// <summary>
        /// Shaded With Edges
        /// 着色并显示边线
        /// </summary>
        ShadedWithEdges,

        /// <summary>
        /// Shaded
        /// 仅着色
        /// </summary>
        Shaded
    }
}
