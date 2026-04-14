// -*- coding: utf-8 -*-
// src/Base/Features/CustomFeature/Enums/CustomFeatureOptions_e.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义自定义特征的选项标志枚举，控制特征的特定行为。
// 包括是否始终位于特征树底部、是否支持阵列、是否可拖拽、是否缓存几何体等选项。
//*********************************************************************

using System;

namespace Xarial.XCad.Features.CustomFeature.Enums
{
    /// <summary>
    /// Options of the <see cref="IXCustomFeature"/>
    /// <see cref="IXCustomFeature"/> 的选项标志
    /// </summary>
    [Flags]
    public enum CustomFeatureOptions_e
    {
        /// <summary>
        /// Default options
        /// 默认选项
        /// </summary>
        Default = 0,

        /// <summary>
        /// Custom feature should be always at the bottom of the feature tree
        /// 自定义特征始终位于特征树底部
        /// </summary>
        AlwaysAtEnd = 1,

        /// <summary>
        /// Custom feature can be patterned
        /// 自定义特征可参与阵列
        /// </summary>
        Patternable = 2,

        /// <summary>
        /// Castom feature can be dragged
        /// 自定义特征可拖拽
        /// </summary>
        Dragable = 4,

        /// <summary>
        /// Custom feature does not cache the body
        /// 自定义特征不缓存实体
        /// </summary>
        NoCachedBody = 8
    }
}