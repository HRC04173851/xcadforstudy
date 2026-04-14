// -*- coding: utf-8 -*-
// src/Base/Annotations/Delegates/DimensionValueChangedDelegate.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义 DimensionValueChangedDelegate 委托，用于处理维度值变更事件。
// 该委托接收维度对象和新的数值，作为 IXDimension.ValueChanged 事件的回调签名。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.Annotations.Delegates
{
    /// <summary>
    /// Delegate of <see cref="IXDimension.ValueChanged"/> event
    /// <see cref="IXDimension.ValueChanged"/> 事件的委托
    /// </summary>
    /// <param name="dim">Sender</param>
    /// <param name="newVal">New value of the dimension</param>
    public delegate void DimensionValueChangedDelegate(IXDimension dim, double newVal);
}
