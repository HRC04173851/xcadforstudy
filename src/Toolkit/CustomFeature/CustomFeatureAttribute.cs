//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using Xarial.XCad.Features.CustomFeature;

namespace Xarial.XCad.Utils.CustomFeature
{
    /// <summary>
    /// Represents the custom attribute of the <see cref="IXCustomFeature"/>
    /// <para>表示 <see cref="IXCustomFeature"/> 的自定义属性项。</para>
    /// </summary>
    public class CustomFeatureAttribute
    {
        /// <summary>
        /// Name of the attribute
        /// <para>属性名称。</para>
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Data type of the attribute
        /// <para>属性的数据类型。</para>
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Value of the attribute
        /// <para>属性值。</para>
        /// </summary>
        public object Value { get; }

        /// <summary>
        /// Constructor
        /// <para>构造函数。</para>
        /// </summary>
        /// <param name="name">Attribute name<para>属性名称。</para></param>
        /// <param name="type">Attribute data type<para>属性数据类型。</para></param>
        /// <param name="value">Attribute value<para>属性值。</para></param>
        public CustomFeatureAttribute(string name, Type type, object value)
        {
            Name = name;
            Type = type;
            Value = value;
        }
    }
}