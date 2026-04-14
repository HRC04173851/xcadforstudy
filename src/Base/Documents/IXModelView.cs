// -*- coding: utf-8 -*-
// src/Base/Documents/IXModelView.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义模型视图接口，提供视图显示模式、变换矩阵、缩放功能，
// 支持自定义图形渲染和视图冻结等操作。
//*********************************************************************

using System;
using System.Drawing;
using Xarial.XCad.Base;
using Xarial.XCad.Documents.Delegates;
using Xarial.XCad.Documents.Enums;
using Xarial.XCad.Geometry.Structures;

namespace Xarial.XCad.Documents
{
    /// <summary>
    /// Context to draw the custom graphics
    /// 自定义图形绘制上下文
    /// </summary>
    public interface IXCustomGraphicsContext : IDisposable
    {
    }

    /// <summary>
    /// Represents the model view
    /// 表示模型视图
    /// </summary>
    public interface IXModelView : IXTransaction
    {
        /// <summary>
        /// Display mode of the view
        /// 视图显示模式
        /// </summary>
        ViewDisplayMode_e DisplayMode { get; set; }

        /// <summary>
        /// Fired when custom graphics can be drawn in the model
        /// 当模型允许绘制自定义图形时触发
        /// </summary>
        event RenderCustomGraphicsDelegate RenderCustomGraphics;

        /// <summary>
        /// Freezes all view updates
        /// 冻结所有视图刷新
        /// </summary>
        /// <param name="freeze">True to suppress all updates</param>
        /// <returns>Freeze object, when disposed - view is restored</returns>
        IDisposable Freeze(bool freeze);

        /// <summary>
        /// Transformation of this view related to the model origin
        /// 该视图相对于模型原点的变换
        /// </summary>
        TransformMatrix Transform { get; set; }

        /// <summary>
        /// Transformation of this view related to the screen coordinates
        /// 该视图相对于屏幕坐标的变换
        /// </summary>
        TransformMatrix ScreenTransform { get; }

        /// <summary>
        /// View boundaries
        /// 视图屏幕边界
        /// </summary>
        Rectangle ScreenRect { get; }

        /// <summary>
        /// Zooms view to the specified box in XYZ model space
        /// </summary>
        /// <param name="box">Box to zoom to</param>
        void ZoomToBox(Box3D box);

        /// <summary>
        /// Zooms view to fit the model
        /// </summary>
        void ZoomToFit();

        /// <summary>
        /// Zooms to the specified objects
        /// </summary>
        /// <param name="objects">Objects to zoom to</param>
        void ZoomToObjects(IXSelObject[] objects);

        /// <summary>
        /// Refreshes the view
        /// </summary>
        void Update();
    }

    /// <summary>
    /// Represents the view which contains name
    /// </summary>
    public interface IXNamedView : IXModelView 
    {
        /// <summary>
        /// Name of the view
        /// </summary>
        string Name { get; }
    }

    /// <summary>
    /// Represents the one of the standard views
    /// </summary>
    public interface IXStandardView : IXModelView 
    {
        /// <summary>
        /// Type of this standard view
        /// </summary>
        StandardViewType_e Type { get; }
    }

    /// <summary>
    /// Standard 3D views of the model
    /// </summary>
    public enum StandardViewType_e 
    {
        /// <summary>
        /// Front view
        /// </summary>
        Front,

        /// <summary>
        /// Back view
        /// </summary>
        Back,

        /// <summary>
        /// Left view
        /// </summary>
        Left,

        /// <summary>
        /// Right view
        /// </summary>
        Right,

        /// <summary>
        /// Top view
        /// </summary>
        Top,

        /// <summary>
        /// Bottom view
        /// </summary>
        Bottom,

        /// <summary>
        /// Isometric view
        /// </summary>
        Isometric,

        /// <summary>
        /// Trimetric view
        /// </summary>
        Trimetric,

        /// <summary>
        /// Dimetric view
        /// </summary>
        Dimetric
    }
}