// -*- coding: utf-8 -*-
// src/Base/Features/CustomFeature/Enums/CustomFeatureOutdateState_e.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 指示因参数升级而过期的宏特征元素状态枚举。
// 用于标记图标或尺寸定义发生变化且无法自动升级的情况。
//*********************************************************************

using System;

namespace Xarial.XCad.Features.CustomFeature.Enums
{
    /// <summary>
    /// Indicates which elements of the macro feature are outdated due to the parameters upgrade
    /// 指示因参数升级而过期的宏特征元素
    /// </summary>
    [Flags]
    public enum CustomFeatureOutdateState_e
    {
        /// <summary>
        /// All parameters are up-to-date
        /// 所有参数均为最新
        /// </summary>
        UpToDate = 0,

        /// <summary>
        /// Macro feature icon has changed and cannot be updated
        /// 宏特征图标已变化且无法自动升级
        /// </summary>
        Icons = 1 << 0,

        /// <summary>
        /// Macro feature dimensions have changed and cannot be upgraded
        /// 宏特征尺寸定义已变化且无法自动升级
        /// </summary>
        Dimensions = 1 << 1
    }
}