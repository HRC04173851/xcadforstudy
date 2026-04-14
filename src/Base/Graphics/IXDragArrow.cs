// -*- coding: utf-8 -*-
// src/Base/Graphics/IXDragArrow.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义拖动箭头操作器接口，提供带方向的箭头操纵器，支持选择、翻转和长度方向设置。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Base;
using Xarial.XCad.Geometry.Structures;

namespace Xarial.XCad.Graphics
{
    /// <summary>
    /// Delegate of <see cref="IXDragArrow.Flipped"/> event
    /// <see cref="IXDragArrow.Flipped"/> 事件的委托
    /// </summary>
    /// <param name="sender">Drag arrow manipulator</param>
    /// <param name="direction">New direction</param>
    public delegate void DragArrowFlippedDelegate(IXDragArrow sender, Vector direction);

    /// <summary>
    /// Delegate of <see cref="IXDragArrow.Selected"/> event
    /// <see cref="IXDragArrow.Selected"/> 事件的委托
    /// </summary>
    /// <param name="sender">Drag arrow manipulator</param>
    public delegate void DragArrowSelectedDelegate(IXDragArrow sender);

    /// <summary>
    /// Drag arrow manipulator
    /// 拖动箭头操作器
    /// </summary>
    public interface IXDragArrow : IXObject, IXTransaction, IDisposable
    {
        /// <summary>
        /// Event is raised when the direction is flipped
        /// 方向翻转时触发
        /// </summary>
        event DragArrowFlippedDelegate Flipped;

        /// <summary>
        /// Event is raised when drag arrow manipulator is selected
        /// 操作器被选中时触发
        /// </summary>
        event DragArrowSelectedDelegate Selected;

        /// <summary>
        /// True if the direction can be flipped
        /// 是否可以翻转方向
        /// </summary>
        bool CanFlip { get; set; }

        /// <summary>
        /// Length of the manipulator
        /// 操作器的长度
        /// </summary>
        double Length { get; set; }

        /// <summary>
        /// Direction of the arrow
        /// 箭头的方向
        /// </summary>
        Vector Direction { get; set; }

        /// <summary>
        /// Origin of the arrow
        /// 箭头的起点
        /// </summary>
        Point Origin { get; set; }

        /// <summary>
        /// Controls the visibility of this triad
        /// 控制操作器的可见性
        /// </summary>
        bool Visible { get; set; }
    }
}
