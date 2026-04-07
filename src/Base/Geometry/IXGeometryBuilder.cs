//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
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
