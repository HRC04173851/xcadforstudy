// -*- coding: utf-8 -*-
// src/Base/Features/CustomFeature/Enums/CustomFeatureDimensionType_e.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义与IXCustomFeature关联的尺寸类型枚举。
// 包括线性尺寸、角度尺寸和径向尺寸三种类型。
//*********************************************************************

namespace Xarial.XCad.Features.CustomFeature.Enums
{
    /// <summary>
    /// Type of the dimansion associated with <see cref="IXCustomFeature"/>
    /// 与 <see cref="IXCustomFeature"/> 关联的尺寸类型
    /// </summary>
    public enum CustomFeatureDimensionType_e
    {
        /// <summary>
        /// Linear
        /// 线性尺寸
        /// </summary>
        Linear = 2,

        /// <summary>
        /// Angular
        /// 角度尺寸
        /// </summary>
        Angular = 3,

        /// <summary>
        /// Radial
        /// 径向尺寸
        /// </summary>
        Radial = 5
    }
}