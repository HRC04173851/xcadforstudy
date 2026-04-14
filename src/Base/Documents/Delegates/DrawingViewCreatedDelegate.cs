// -*- coding: utf-8 -*-
// src/Base/Documents/Delegates/DrawingViewCreatedDelegate.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 提供工程图视图创建事件的委托定义，用于在工程图视图创建完成时触发通知。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xarial.XCad.Documents.Delegates
{
    /// <summary>
    /// Delegate of <see cref="IXDrawingViewRepository.ViewCreated"/> event
    /// <see cref="IXDrawingViewRepository.ViewCreated"/> 事件委托
    /// </summary>
    /// <param name="drawing">Drawing where the view is created（创建视图的工程图）</param>
    /// <param name="sheet">Sheet where view is created（创建视图的图纸）</param>
    /// <param name="view">Created drawing view（新建工程图视图）</param>
    public delegate void DrawingViewCreatedDelegate(IXDrawing drawing, IXSheet sheet, IXDrawingView view);
}
