//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using Xarial.XCad.Features.CustomFeature.Enums;

namespace Xarial.XCad.Features.CustomFeature.Attributes
{
    /// <summary>
    /// Provides additional options for custom feature
    /// 为自定义特征提供附加选项
    /// </summary>
    public class CustomFeatureOptionsAttribute : Attribute
    {
        /// <summary>
        /// Optipons of the custom feature
        /// 自定义特征选项标志
        /// </summary>
        public CustomFeatureOptions_e Flags { get; }

        /// <summary>
        /// Options for macro feature
        /// 宏特征选项构造函数
        /// </summary>
        /// This is a default name assigned to the feature when created followed by the index</param>
        /// <param name="flags">Additional options for custom feature</param>
        public CustomFeatureOptionsAttribute(
            CustomFeatureOptions_e flags)
        {
            Flags = flags;
        }
    }
}