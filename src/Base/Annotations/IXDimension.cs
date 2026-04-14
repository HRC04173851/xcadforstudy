// -*- coding: utf-8 -*-
// src/Base/Annotations/IXDimension.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件定义尺寸标注（Dimension）的跨CAD平台接口。
// Dimension 是工程图中最常见的注解类型，用于标注模型的尺寸。
//
// 尺寸类型：
// - 线性尺寸：直线距离
// - 角度尺寸：两条直线或平面间的角度
// - 直径尺寸：圆或圆弧的直径
// - 半径尺寸：圆或圆弧的半径
// - 倒角尺寸：倒角特征的尺寸
//
// 尺寸特性：
// - ValueChanged 事件：尺寸值改变时通知
// - Value 属性：获取或设置尺寸值
// - Name 属性：尺寸的唯一名称
//
// 尺寸与参数：
// - 驱动尺寸：修改尺寸值会改变模型参数
// - Driven 尺寸：只读显示，不影响模型
//*********************************************************************

using Xarial.XCad.Annotations.Delegates;
using Xarial.XCad.Base;

namespace Xarial.XCad.Annotations
{
    /// <summary>
    /// Annotation which drives the dimension parameter
    /// 驱动尺寸参数的标注接口
    /// </summary>
    public interface IXDimension : IXAnnotation, IXTransaction
    {
        /// <summary>
        /// Fired when the value of this dimension is changed
        /// 尺寸值改变时触发
        /// </summary>
        event DimensionValueChangedDelegate ValueChanged;

        /// <summary>
        /// Name of the dimension
        /// 尺寸的名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Dimension value in the system units
        /// 以系统单位表示的尺寸值
        /// </summary>
        double Value { get; set; }
    }
}