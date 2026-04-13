//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using Xarial.XCad.Annotations.Delegates;
using Xarial.XCad.Base;

namespace Xarial.XCad.Annotations
{
    /// <summary>
    /// Annotation which drives the dimension parameter
    /// 驱动尺寸参数的标注接口
    /// </summary>
    public interface IXDimension : IXAnnotation, IXTransaction
    {
        /// <summary>
        /// Fired when the value of this dimension is changed
        /// 尺寸值改变时触发
        /// </summary>
        event DimensionValueChangedDelegate ValueChanged;

        /// <summary>
        /// Name of the dimension
        /// 尺寸的名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Dimension value in the system units
        /// 以系统单位表示的尺寸值
        /// </summary>
        double Value { get; set; }
    }
}