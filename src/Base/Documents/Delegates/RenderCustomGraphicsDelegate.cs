// -*- coding: utf-8 -*-
// src/Base/Documents/Delegates/RenderCustomGraphicsDelegate.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 提供自定义图形渲染事件的委托定义，用于在模型视图渲染时触发自定义图形绘制回调。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.Documents.Delegates
{
    /// <summary>
    /// Delegate of <see cref="IXModelView.RenderCustomGraphics"/> event
    /// <see cref="IXModelView.RenderCustomGraphics"/> 事件委托
    /// </summary>
    /// <param name="sender">Model view which sends this event（触发事件的模型视图）</param>
    /// <param name="context">Custom graphics context（自定义图形上下文）</param>
    public delegate bool RenderCustomGraphicsDelegate(IXModelView sender, IXCustomGraphicsContext context);
}
