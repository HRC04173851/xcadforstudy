//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using Xarial.XCad.Base;
using Xarial.XCad.Geometry.Primitives;

namespace Xarial.XCad.Geometry
{
    /// <summary>
    /// Represents geometry builder interface
    /// 表示三维几何构建器接口
    /// </summary>
    public interface IX3DGeometryBuilder : IXRepository<IXPrimitive>
    {
        
    }

    /// <summary>
    /// Additional methods for <see cref="IX3DGeometryBuilder"/>
    /// <see cref="IX3DGeometryBuilder"/> 的扩展方法
    /// </summary>
    public static class X3DGeometryBuilderExtension 
    {
        /// <summary>
        /// Creates an extrusion template
        /// 创建拉伸（Extrusion）模板
        /// </summary>
        /// <returns>Extrusion template</returns>
        public static IXExtrusion PreCreateExtrusion(this IX3DGeometryBuilder geomBuilder) => geomBuilder.PreCreate<IXExtrusion>();

        /// <summary>
        /// Creates sweep template
        /// 创建扫描（Sweep）模板
        /// </summary>
        /// <returns>Sweep template</returns>
        public static IXSweep PreCreateSweep(this IX3DGeometryBuilder geomBuilder) => geomBuilder.PreCreate<IXSweep>();

        /// <summary>
        /// Creates loft template
        /// 创建放样（Loft）模板
        /// </summary>
        /// <returns>Loft template</returns>
        public static IXLoft PreCreateLoft(this IX3DGeometryBuilder geomBuilder) => geomBuilder.PreCreate<IXLoft>();

        /// <summary>
        /// Creates revolve template
        /// 创建旋转（Revolve）模板
        /// </summary>
        /// <returns>Revolve template</returns>
        public static IXRevolve PreCreateRevolve(this IX3DGeometryBuilder geomBuilder) => geomBuilder.PreCreate<IXRevolve>();

        /// <summary>
        /// Creates knit template
        /// 创建缝合（Knit）模板
        /// </summary>
        /// <returns>Knit template</returns>
        public static IXKnit PreCreateKnit(this IX3DGeometryBuilder geomBuilder) => geomBuilder.PreCreate<IXKnit>();
    }
}
