// -*- coding: utf-8 -*-
// src/SolidWorks/Features/CustomFeature/Toolkit/SwDimensionPlaceholder.cs

//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using Xarial.XCad.SolidWorks.Annotations;

namespace Xarial.XCad.SolidWorks.Features.CustomFeature.Toolkit
{
    /// <summary>
    /// 尺寸占位符类。
    /// <para>
    /// 这是 SolidWorks 显示尺寸的模拟实现。
    /// 用于在 <see cref="XCad.Features.CustomFeature.Services.IParameterConverter.ConvertDisplayDimensions"/> 中
    /// 支持宏特性参数的向后兼容性。
    /// </para>
    /// </summary>
    /// <remarks>
    /// 当参数解析过程中需要返回尺寸但实际尺寸对象不可用时，
    /// 使用此占位符代替。它的值始终为 NaN，表示无效或未初始化状态。
    /// </remarks>
    internal class SwDimensionPlaceholder : SwDimension
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        internal SwDimensionPlaceholder() : base(null, null, null)
        {
        }

        /// <summary>
        /// 获取或设置尺寸值
        /// </summary>
        /// <remarks>占位符的值始终为 NaN，因为实际的尺寸值来自真实尺寸对象</remarks>
        public override double Value
        {
            get => double.NaN;
            set => base.Value = value;
        }
    }
}