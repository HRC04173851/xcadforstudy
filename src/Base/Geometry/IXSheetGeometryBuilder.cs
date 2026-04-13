//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using Xarial.XCad.Geometry.Primitives;

namespace Xarial.XCad.Geometry
{
    /// <summary>
    /// Provides methods to buld sheet geometry
    /// 提供曲面/片体几何构建方法
    /// </summary>
    public interface IXSheetGeometryBuilder : IX3DGeometryBuilder
    {   
    }

    /// <summary>
    /// Additional methods for <see cref="IXSheetGeometryBuilder"/>
    /// <see cref="IXSheetGeometryBuilder"/> 的扩展方法
    /// </summary>
    public static class XSheetGeometryBuilderExtension 
    {
        /// <summary>
        /// Creates new instance of planar sheet
        /// 创建平面片体（Planar Sheet）模板
        /// </summary>
        /// <returns>Planar sheet template</returns>
        public static IXPlanarSheet PreCreatePlanarSheet(this IXSheetGeometryBuilder geomBuilder) => geomBuilder.PreCreate<IXPlanarSheet>();
    }
}
