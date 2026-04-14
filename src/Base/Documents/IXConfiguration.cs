// -*- coding: utf-8 -*-
// src/Base/Documents/IXConfiguration.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件定义配置（Configuration）的跨CAD平台接口。
// Configuration 是同一零件或装配体的不同变体。
//
// 配置核心概念：
// 1. 多配置：单一 CAD 文档可以包含多个配置
// 2. 参数差异：不同配置使用不同的参数值
// 3. BOM 关联：配置与物料清单（BOM）条目关联
// 4. 显示控制：配置控制零件的显示状态
//
// 配置属性：
// - Name：配置名称
// - PartNumber：零件编号
// - Quantity：BOM 数量
// - BomChildrenSolving：子项在 BOM 中的显示方式
// - Parent：父子配置关系
//
// 使用场景：
// - 零件的不同版本（标准版、高配版）
// - 装配体的不同配置（有无某个零部件）
// - 制造差异（不同加工工艺的参数）
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