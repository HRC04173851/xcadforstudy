// -*- coding: utf-8 -*-
// src/Base/Geometry/IXSolidGeometryBuilder.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件定义实体几何体构建器（Solid Geometry Builder）的接口。
// Solid Builder 专门用于创建实体几何体相关的操作。
//
// 实体构建功能：
// - 拉伸（Extrusion）：将2D轮廓沿垂直方向拉伸成实体
// - 旋转（Revolution）：将2D轮廓绕轴旋转成实体
// - 扫掠（Sweep）：沿路径扫掠轮廓成实体
// - 放样（Loft）：多轮廓渐变过渡成实体
// - 布尔运算（Boolean）：合并、差集、交集
//
// 与 Sheet Builder 的区别：
// - Solid Builder：创建有封闭体积的几何体
// - Sheet Builder：创建无封闭体积的片体曲面
//*********************************************************************

namespace Xarial.XCad.Geometry
{
    /// <summary>
    /// Builds solid geometry
    /// 用于构建实体几何
    /// </summary>
    public interface IXSolidGeometryBuilder : IX3DGeometryBuilder
    {
    }
}
