// -*- coding: utf-8 -*-
// src/Base/Geometry/IXMemoryBody.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件定义内存临时几何体（Memory Body）的接口。
// Memory Body 是不属于文档的临时几何体，用于算法计算和预览。
//
// Memory Body 与普通 Body 的区别：
// - 普通 Body：属于文档，持久化存储
// - Memory Body：存在于内存，不属于文档，用于临时计算
//
// 核心功能：
// 1. 布尔运算：Add（并）、Substract（差）、Common（交）
// 2. 预览显示：Preview 方法在文档中显示临时几何体
// 3. 资源管理：实现 IDisposable 释放内存
//
// 使用场景：
// - 特征编辑时的实时预览
// - 布尔运算的中间结果
// - 临时几何体构建后的后续处理
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Drawing;
using Xarial.XCad.Base;
using Xarial.XCad.Documents;
using Xarial.XCad.Geometry.Primitives;
using Xarial.XCad.Geometry.Structures;

namespace Xarial.XCad.Geometry
{
    /// <summary>
    /// Represents the memory (temp) body object
    /// 表示内存中的临时几何体对象
    /// </summary>
    public interface IXMemoryBody : IXBody, IDisposable
    {
        /// <summary>
        /// DIsplays the preview of the memory body
        /// 显示临时几何体的预览图形
        /// </summary>
        /// <param name="context">Context where preview should be displayed (e.g. document or component)</param>
        /// <param name="color">Color of the body</param>
        void Preview(IXObject context, Color color);

        /// <summary>
        /// Boolean add operation on body
        /// 对几何体执行布尔并（Union）运算
        /// </summary>
        /// <param name="other">Other body</param>
        /// <returns>Resulting body</returns>
        /// <exception cref="Exceptions.BodyBooleanOperationNoIntersectException"/>
        IXMemoryBody Add(IXMemoryBody other);

        /// <summary>
        /// Boolean substract operation
        /// 对几何体执行布尔差（Subtract）运算
        /// </summary>
        /// <param name="other">Body to substract</param>
        /// <returns>Resulting bodies</returns>
        /// <exception cref="Exceptions.BodyBooleanOperationNoIntersectException"/>
        IXMemoryBody[] Substract(IXMemoryBody other);

        /// <summary>
        /// Boolean common operation
        /// 对几何体执行布尔交（Common/Intersect）运算
        /// </summary>
        /// <param name="other">Body to get common with</param>
        /// <returns>Resulting body</returns>
        /// <exception cref="Exceptions.BodyBooleanOperationNoIntersectException"/>
        IXMemoryBody[] Common(IXMemoryBody other);
    }

    /// <summary>
    /// Represents the memory (temp) sheet (surface) body
    /// 表示内存中的临时片体（曲面体）
    /// </summary>
    public interface IXMemorySheetBody : IXMemoryBody, IXSheetBody
    {
    }

    /// <summary>
    /// Subtype of <see cref="IXMemorySheetBody"/> which is planar
    /// </summary>
    public interface IXMemoryPlanarSheetBody : IXMemorySheetBody, IXPlanarSheetBody
    {
    }

    /// <summary>
    /// Represents the memory (temp) solid body geometry
    /// 表示内存中的临时实体体
    /// </summary>
    public interface IXMemorySolidBody : IXMemoryBody, IXSolidBody
    {
    }

    /// <summary>
    /// Represents the the memory (temp) wire body
    /// 表示内存中的临时线框体
    /// </summary>
    public interface IXMemoryWireBody : IXMemoryBody, IXWireBody
    {
    }
}