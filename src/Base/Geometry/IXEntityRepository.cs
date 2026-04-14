// -*- coding: utf-8 -*-
// src/Base/Geometry/IXEntityRepository.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件定义几何实体集合仓储（Entity Repository）的接口。
// Entity Repository 提供对拓扑实体（Face、Edge、Vertex）的统一访问。
//
// 仓储功能：
// - Find：按名称或条件查找实体
// - FindAll：查找所有实体
// - Filter：按类型过滤实体
//
// 使用场景：
// - 从 Body 获取所有面、边、顶点
// - 从 Feature 获取关联的拓扑实体
// - 批量处理拓扑元素
//*********************************************************************

using Xarial.XCad.Base;

namespace Xarial.XCad.Geometry
{
    /// <summary>
    /// Represents the collection of <see cref="IXEntity"/>
    /// 表示 <see cref="IXEntity"/> 的集合仓储
    /// </summary>
    public interface IXEntityRepository : IXRepository<IXEntity> 
    {
    }
}