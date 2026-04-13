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
    /// Specifies that the current property is a dimension of the macro feature.
    /// The value if the property is the current value of the dimension.
    /// This property is bi-directional: it will update the value of the dimension
    /// when changed within the <see cref="IXCustomFeatureDefinition.OnRebuild(IXApplication, Documents.IXDocument, IXCustomFeature)"/>
    /// as well as will contain the current value of the dimension when it got modified by the user in the
    /// graphics area
    /// 指定当前属性为宏特征尺寸参数，支持参数与图形区尺寸值双向同步
    /// </summary>
    /// <remarks>This should only be used for numeric properties</remarks>
    [AttributeUsage(AttributeTargets.Property)]
    public class ParameterDimensionAttribute : Attribute
    {
        public CustomFeatureDimensionType_e DimensionType { get; private set; }

        /// <summary>
        /// Marks this property as dimension and assigns the dimension type
        /// 将属性标记为尺寸参数并指定尺寸类型
        /// </summary>
        /// <param name="dimType">Type of the dimension</param>
        public ParameterDimensionAttribute(CustomFeatureDimensionType_e dimType)
        {
            DimensionType = dimType;
        }
    }
}