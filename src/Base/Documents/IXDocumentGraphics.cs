// -*- coding: utf-8 -*-
// src/Base/Documents/IXDocumentGraphics.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义文档图形接口，为三维文档提供图形交互与可视化能力，
// 支持创建标注气泡、三轴操控器和拖拽箭头等图形元素。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Graphics;
using Xarial.XCad.UI;

namespace Xarial.XCad.Documents
{
    /// <summary>
    /// Provides graphics features to the see <see cref="IXDocument3D"/>
    /// 为 <see cref="IXDocument3D"/> 提供图形交互与可视化能力
    /// </summary>
    public interface IXDocumentGraphics
    {
        /// <summary>
        /// Pre-creates callout instance
        /// 预创建标注气泡（Callout）
        /// </summary>
        /// <returns>Instance of the callout</returns>
        IXCallout PreCreateCallout();

        /// <summary>
        /// Pre-creates triad manipulator
        /// 预创建三轴操控器（Triad）
        /// </summary>
        /// <returns>Instance of triad manipulator</returns>
        IXTriad PreCreateTriad();

        IXDragArrow PreCreateDragArrow();
    }
}
