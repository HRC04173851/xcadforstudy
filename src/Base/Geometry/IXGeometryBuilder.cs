// -*- coding: utf-8 -*-
// src/Base/Geometry/IXGeometryBuilder.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件定义几何体构建器（Geometry Builder）的接口。
// Geometry Builder 提供通过 API 程序化创建几何体的功能。
//
// 构建器类型：
// 1. WireBuilder - 线框几何构建器（曲线、点等）
// 2. SheetBuilder - 曲面几何构建器（曲面、填充等）
// 3. SolidBuilder - 实体几何构建器（拉伸、旋转、布尔等）
//
// 几何体构建方式：
// - 程序化创建：通过 API 调用直接创建几何体
// - 内存几何体：通过 IXMemoryGeometryBuilder 创建临时几何体
//
// 预创建模式（PreCreate）：
// 某些几何体支持预创建模式，先创建模板对象，
// 设置参数后再提交（Commit）实际创建
//*********************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xarial.XCad.Geometry.Structures;
using Xarial.XCad.Geometry.Surfaces;
using Xarial.XCad.Geometry.Wires;

namespace Xarial.XCad.Geometry
{
    /// <summary>
    /// Provides access to geometry buidling functions
    /// <para>中文：提供对几何体构建功能的访问</para>
    /// </summary>
    public interface IXGeometryBuilder
    {
        /// <summary>
        /// Provides an access to wire geometry builder functions
        /// <para>中文：提供对线框几何体构建功能的访问</para>
        /// </summary>
        IXWireGeometryBuilder WireBuilder { get; }

        /// <summary>
        /// Provides an access to sheet geometry builder functions
        /// <para>中文：提供对曲面几何体构建功能的访问</para>
        /// </summary>
        IXSheetGeometryBuilder SheetBuilder { get; }

        /// <summary>
        /// Provides an access to solid geometry builder functions
        /// <para>中文：提供对实体几何体构建功能的访问</para>
        /// </summary>
        IXSolidGeometryBuilder SolidBuilder { get; }

        /// <summary>
        /// Pre-creates planar region
        /// <para>中文：预创建平面区域（轮廓）模板</para>
        /// </summary>
        /// <returns>Region template</returns>
        IXPlanarRegion PreCreatePlanarRegion();
    }

    /// <summary>
    /// Geometry builder for building in-memory geometry objects
    /// <para>中文：用于在内存中构建几何体对象的构建器</para>
    /// </summary>
    public interface IXMemoryGeometryBuilder : IXGeometryBuilder 
    {
        /// <summary>
        /// Deserializes memory body from the stream
        /// <para>中文：从流中反序列化内存实体</para>
        /// </summary>
        /// <param name="stream">Stream to deserialize body from</param>
        /// <returns>Deserialized body</returns>
        IXBody DeserializeBody(Stream stream);

        /// <summary>
        /// Serializes body into the stream
        /// <para>中文：将实体序列化到流中</para>
        /// </summary>
        /// <param name="body">Body to store</param>
        /// <param name="stream">Stream to store to</param>
        void SerializeBody(IXBody body, Stream stream);
    }
}
