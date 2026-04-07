//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using Xarial.XCad.Base;
using Xarial.XCad.Data;
using Xarial.XCad.Features;
using Xarial.XCad.UI;
using Xarial.XCad.Documents.Enums;
using System.Collections.Generic;
using Xarial.XCad.Annotations;

namespace Xarial.XCad.Documents
{
    /// <summary>
    /// Represents the configiration (variant) of the document
    /// <para>中文：表示文档的配置（变体）</para>
    /// </summary>
    public interface IXConfiguration : IXSelObject, IXTransaction, IPropertiesOwner, IDimensionable
    {
        /// <summary>
        /// BOM quantity value
        /// <para>中文：物料清单（BOM）中的数量值</para>
        /// </summary>
        double Quantity { get; }

        /// <summary>
        /// Name of the configuration
        /// <para>中文：配置名称</para>
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Returns part number of this configuration
        /// <para>中文：返回此配置的零件编号</para>
        /// </summary>
        string PartNumber { get; }

        /// <summary>
        /// Parent configuration or null if this is a top level configuration
        /// <para>中文：父配置，若为顶层配置则返回 null</para>
        /// </summary>
        IXConfiguration Parent { get; }

        /// <summary>
        /// Options for displaying this configuration in BOM
        /// <para>中文：此配置在物料清单（BOM）中的子项显示选项</para>
        /// </summary>
        BomChildrenSolving_e BomChildrenSolving { get; }

        /// <summary>
        /// Preview image of this configuration
        /// <para>中文：此配置的预览图像</para>
        /// </summary>
        IXImage Preview { get; }
    }
}