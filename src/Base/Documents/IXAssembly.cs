// -*- coding: utf-8 -*-
// src/Base/Documents/IXAssembly.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件定义装配体（Assembly）的跨CAD平台接口。
// 装配体是由多个零件和/或子装配体组成的集合，通过约束关系定位。
//
// 装配体核心概念：
// 1. 组件（Component）：装配体中的零件或子装配体引用
// 2. 约束（Mates）：定义组件间的几何关系（重合、平行、距离、角度等）
// 3. 爆炸状态：组件可以展开显示或爆炸视图显示
// 4. 配置管理：支持装配体的多配置
//
// 装配体类型层次：
// - IXAssembly：基础装配体接口
//   - IXPartAssembly：零件装配体
//   - IX weldmentAssembly：焊接装配体
//
// 装配体事件：
// - ComponentInserted：组件插入时
// - ComponentDeleting：组件删除前
// - ComponentDeleted：组件删除后
//*********************************************************************

using Xarial.XCad.Documents.Delegates;
using Xarial.XCad.Geometry;

namespace Xarial.XCad.Documents
{
    /// <summary>
    /// Represents an assembly document (composition of <see cref="IXPart"/> and other <see cref="IXAssembly"/>)
    /// <para>中文：表示装配体文档（由零件和子装配体组成）</para>
    /// </summary>
    public interface IXAssembly : IXDocument3D
    {
        /// <summary>
        /// Raised when new component is inserted into the assembly
        /// <para>中文：新组件插入装配体时触发</para>
        /// </summary>
        event ComponentInsertedDelegate ComponentInserted;

        /// <summary>
        /// Raised when component is about to be deleted from the assembly
        /// <para>中文：组件即将从装配体中删除时触发</para>
        /// </summary>
        event ComponentDeletingDelegate ComponentDeleting;

        /// <summary>
        /// Raised when component is deleted from the assembly
        /// <para>中文：组件已从装配体中删除时触发</para>
        /// </summary>
        event ComponentDeletedDelegate ComponentDeleted;

        /// <inheritdoc/>
        new IXAssemblyConfigurationRepository Configurations { get; }

        /// <inheritdoc/>
        new IXAssemblyEvaluation Evaluation { get; }

        /// <summary>
        /// Returns the component which is currently being editied in-context or null
        /// <para>中文：返回当前正在进行关联编辑的组件，若无则返回 null</para>
        /// </summary>
        IXComponent EditingComponent { get; }
    }
}