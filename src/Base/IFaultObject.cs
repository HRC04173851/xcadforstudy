// -*- coding: utf-8 -*-
// src/Base/IFaultObject.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 故障对象接口，用于标识无法解析或缺失的 xCAD 对象，提供错误对象的标记能力。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad
{
    /// <summary>
    /// Represents the fault or missing <see cref="IXObject"/>
    /// 表示故障或缺失的 <see cref="IXObject"/>
    /// </summary>
    public interface IFaultObject
    {
    }
}
