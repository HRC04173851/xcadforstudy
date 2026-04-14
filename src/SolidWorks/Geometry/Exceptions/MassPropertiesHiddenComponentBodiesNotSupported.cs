// -*- coding: utf-8 -*-
// src/SolidWorks/Geometry/Exceptions/MassPropertiesHiddenComponentBodiesNotSupported.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件定义隐藏组件 Body 的质量属性不支持时抛出的异常。
// 在 SOLIDWORKS 2019 中，IMassProperty API 不支持指定包含或排除隐藏实体，
// 当用户在装配体中设置 VisibleOnly 选项但存在隐藏 Body 时触发此异常。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Exceptions;

namespace Xarial.XCad.SolidWorks.Geometry.Exceptions
{
    /// <summary>
    /// IMassProperty API does not support option to specify to include or exclude hidden bodies or components (this option is always default to include hidden)
    /// This excepion indicates that mass cannot be calculated for the assembly which has hidden bodies while <see cref="XCad.Base.IEvaluation.VisibleOnly"/> is set to True
    /// </summary>
    public class MassPropertiesHiddenComponentBodiesNotSupported : NotSupportedException
    {
        internal MassPropertiesHiddenComponentBodiesNotSupported()
            : base("Input component contains hidden bodies. But 'Visible Bodies And Components Only' option is specified. This condition is not supported in SOLIDWORKS 2019")
        {
        }
    }
}
