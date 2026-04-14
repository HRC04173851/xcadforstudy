// -*- coding: utf-8 -*-
// src/Base/Documents/IXDrawingView.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件定义工程图视图（Drawing View）的跨CAD平台接口。
// 工程图视图是图纸上展示模型的投影图形。
//
// 视图类型：
// - 标准视图：前视、后视、左视、右视、俯视、仰视、等轴测
// - 投影视图：从已有视图投影生成
// - 剖视图：用剖切平面切割模型后展示
// - 详图视图：放大显示某个区域
// - 辅助视图：基于参考基准面投影
//
// 视图属性：
// - DisplayMode：显示模式（线框、带边着色、实体等）
// - Bodies：视图中可见的实体范围
// - Sketch：视图关联的草图空间
// - Annotations：视图中的注解集合
// - VisibleEntities：视图中可见的几何实体
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Annotations;
using Xarial.XCad.Base;
using Xarial.XCad.Documents.Enums;
using Xarial.XCad.Documents.Structures;
using Xarial.XCad.Features;
using Xarial.XCad.Geometry;
using Xarial.XCad.Geometry.Structures;

namespace Xarial.XCad.Documents
{
    /// <summary>
    /// Represents the drawing view on <see cref="IXSheet"/>
    /// 表示 <see cref="IXSheet"/> 上的工程图视图
    /// </summary>
    public interface IXDrawingView : IXSelObject, IXObjectContainer, IDimensionable, IXTransaction
    {
        /// <summary>
        /// Display mode of the view
        /// 视图显示模式
        /// </summary>
        /// <remarks>null means that display data is inherited from the base view</remarks>
        ViewDisplayMode_e? DisplayMode { get; set; }

        /// <summary>
        /// Bodies scope of this view
        /// 该视图可见实体体范围
        /// </summary>
        IXBody[] Bodies { get; set; }

        /// <summary>
        /// Sketch space of this sheet
        /// 该视图关联的草图空间
        /// </summary>
        IXSketch2D Sketch { get; }

        /// <summary>
        /// Collection of annotations
        /// 注解集合
        /// </summary>
        IXAnnotationRepository Annotations { get; }

        /// <summary>
        /// Visible entities from this view
        /// 该视图中的可见几何实体
        /// </summary>
        IXEntityRepository VisibleEntities { get; }

        /// <summary>
        /// Returns the visible polylines of the drawing view
        /// 返回该视图可见多段线数据
        /// </summary>
        ViewPolylineData[] Polylines { get; }

        /// <summary>
        /// Contains the document referenced by this drawing view
        /// </summary>
        IXDocument3D ReferencedDocument { get; set; }

        /// <summary>
        /// Contains the configuration this drawing view is created from
        /// </summary>
        IXConfiguration ReferencedConfiguration { get; set; }

        /// <summary>
        /// Name of this drawing view
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Location of this drawing view center
        /// </summary>
        Point Location { get; set; }

        /// <summary>
        /// Represents scale of this drawing view
        /// </summary>
        Scale Scale { get; set; }

        /// <summary>
        /// Outline of the view
        /// </summary>
        Rect2D Boundary { get; }

        /// <summary>
        /// View boundary padding
        /// </summary>
        /// <remarks>Padding represents difference between <see cref="Boundary"/> and geometry</remarks>
        Thickness Padding { get; }

        /// <summary>
        /// Transformation of the drawing view in the drawing space relative to the 3D model orientation
        /// </summary>
        TransformMatrix Transformation { get; }

        /// <summary>
        /// Get the base drawing view
        /// </summary>
        /// <remarks>For the root views, base view will be null</remarks>
        IXDrawingView BaseView { get; set; }

        /// <summary>
        /// Gets all views depending on this view
        /// </summary>
        IEnumerable<IXDrawingView> DependentViews { get; }

        /// <summary>
        /// Updates this drawing view
        /// </summary>
        void Update();
    }

    /// <summary>
    /// View created from the <see cref="IXModelView"/>
    /// </summary>
    public interface IXModelViewBasedDrawingView : IXDrawingView 
    {
        /// <summary>
        /// Model view this drawing view is based on
        /// </summary>
        IXModelView SourceModelView { get; set; }
    }

    /// <summary>
    /// View projected from <see cref="IXDrawingView"/>
    /// </summary>
    public interface IXProjectedDrawingView : IXDrawingView
    {
        /// <summary>
        /// Direction of this projection view
        /// </summary>
        ProjectedViewDirection_e Direction { get; set; }
    }

    /// <summary>
    /// Auxiliary drawing view
    /// </summary>
    public interface IXAuxiliaryDrawingView : IXDrawingView
    {
    }

    /// <summary>
    /// Section drawing view
    /// </summary>
    public interface IXSectionDrawingView : IXDrawingView
    {
        /// <summary>
        /// Section of this drawing view
        /// </summary>
        IXSectionLine SectionLine { get; set; }
    }

    /// <summary>
    /// Detailed drawing view
    /// </summary>
    public interface IXDetailedDrawingView : IXDrawingView
    {
        /// <summary>
        /// Circle of this detailed view
        /// </summary>
        IXDetailCircle DetailCircle { get; set; }
    }

    /// <summary>
    /// Flat pattern view
    /// </summary>
    public interface IXFlatPatternDrawingView : IXDrawingView 
    {
        /// <summary>
        /// Sheet metal body of the flat pattern view
        /// </summary>
        IXSolidBody SheetMetalBody { get; set; }

        /// <summary>
        /// Options of flat pattern view
        /// </summary>
        FlatPatternViewOptions_e Options { get; set; }
    }

    /// <summary>
    /// Orientation definition of the <see cref="IXRelativeDrawingView"/>
    /// </summary>
    public class RelativeDrawingViewOrientation 
    {
        /// <summary>
        /// Entity which corresponds to the first orientation
        /// </summary>
        public IXPlanarRegion FirstEntity { get; }

        /// <summary>
        /// Direction of the first entity
        /// </summary>
        public StandardViewType_e FirstDirection { get; }

        /// <summary>
        /// Entity which corresponds to the second orientation
        /// </summary>
        public IXPlanarRegion SecondEntity { get; }

        /// <summary>
        /// Direction of the second entity
        /// </summary>
        public StandardViewType_e SecondDirection { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        public RelativeDrawingViewOrientation(IXPlanarRegion firstEntity, StandardViewType_e firstDirection, IXPlanarRegion secondEntity, StandardViewType_e secondDirection)
        {
            FirstEntity = firstEntity;
            FirstDirection = firstDirection;
            SecondEntity = secondEntity;
            SecondDirection = secondDirection;
        }
    }

    /// <summary>
    /// Relative drawing view
    /// </summary>
    public interface IXRelativeDrawingView : IXDrawingView 
    {
        /// <summary>
        /// Orientation of the relative view
        /// </summary>
        RelativeDrawingViewOrientation Orientation { get; set; }
    }
}
