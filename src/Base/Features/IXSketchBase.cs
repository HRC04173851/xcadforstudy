// -*- coding: utf-8 -*-
// src/Base/Features/IXSketchBase.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义草图（二维或三维）的基础接口。
// 提供草图实体列表和空白状态（隐藏/显示）管理功能。
//*********************************************************************

using Xarial.XCad.Sketch;

namespace Xarial.XCad.Features
{
    /// <summary>
    /// Represents the base sketch 2D or 3D
    /// <para>中文：表示二维或三维草图的基础接口</para>
    /// </summary>
    public interface IXSketchBase : IXFeature
    {
        /// <summary>
        /// List of sketch entitites (segments and points)
        /// <para>中文：草图实体列表（线段和点）</para>
        /// </summary>
        IXSketchEntityRepository Entities { get; }

        /// <summary>
        /// Manages the blank state (hidden/visible) of the sketch
        /// <para>中文：管理草图的空白状态（隐藏/显示）</para>
        /// </summary>
        bool IsBlank { get; set; }
    }
}