// -*- coding: utf-8 -*-
// src/SwDocumentManager/Documents/SwDmDrawingView.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 工程图视图包装器，负责解析其引用的模型文档与配置，支持视图级图纸定位与比例访问。
//*********************************************************************

using SolidWorks.Interop.swdocumentmgr;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Xarial.XCad.Annotations;
using Xarial.XCad.Base;
using Xarial.XCad.Base.Enums;
using Xarial.XCad.Documents;
using Xarial.XCad.Documents.Enums;
using Xarial.XCad.Documents.Structures;
using Xarial.XCad.Features;
using Xarial.XCad.Geometry;
using Xarial.XCad.Geometry.Structures;

namespace Xarial.XCad.SwDocumentManager.Documents
{
    /// <summary>
    /// Drawing view contract exposed by Document Manager.
    /// 由 Document Manager 暴露的工程图视图约定。
    /// </summary>
    public interface ISwDmDrawingView : IXDrawingView, ISwDmObject
    {
        ISwDMView DrawingView { get; }
    }

    /// <summary>
    /// Wrapper for a drawing view and its referenced model/configuration.
    /// 工程图视图包装器，同时负责解析其引用模型与配置。
    /// </summary>
    internal class SwDmDrawingView : SwDmSelObject, ISwDmDrawingView
    {
        #region Not Supported
        public Point Location { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
        TSelObject IXObjectContainer.ConvertObject<TSelObject>(TSelObject obj)
            => throw new NotSupportedException();
        public void Update() => throw new NotSupportedException();
        public Scale Scale { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
        public Rect2D Boundary => throw new NotSupportedException();
        public IXBody[] Bodies { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
        public Thickness Padding => throw new NotSupportedException();
        public IXDimensionRepository Dimensions => throw new NotSupportedException();
        public IXAnnotationRepository Annotations => throw new NotSupportedException();
        public IXDrawingView BaseView { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
        public IEnumerable<IXDrawingView> DependentViews => throw new NotSupportedException();
        public IXSketch2D Sketch => throw new NotSupportedException();
        public TransformMatrix Transformation => throw new NotSupportedException();
        public IXEntityRepository VisibleEntities => throw new NotSupportedException();
        public ViewPolylineData[] Polylines => throw new NotSupportedException();
        public ViewDisplayMode_e? DisplayMode { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
        #endregion

        public ISwDMView DrawingView { get; }

        public string Name 
        {
            get => DrawingView.Name;
            set => throw new NotSupportedException(); 
        }

        /// <summary>
        /// Resolves the model document referenced by the drawing view.
        /// 解析该工程图视图所引用的模型文档。
        /// </summary>
        public IXDocument3D ReferencedDocument 
        {
            get 
            {
                if (m_CachedDocument == null || !m_CachedDocument.IsAlive)
                {
                    var fileName = DrawingView.ReferencedDocument;

                    if (!string.IsNullOrEmpty(fileName) && fileName != "-")
                    {
                        var refDoc = m_Drw.Dependencies.First(d => string.Equals(Path.GetFileName(d.Path), fileName, StringComparison.CurrentCultureIgnoreCase));

                        if (!refDoc.IsCommitted) 
                        {
                            var isReadOnly = m_Drw.State.HasFlag(DocumentState_e.ReadOnly);

                            if (isReadOnly) 
                            {
                                refDoc.State = DocumentState_e.ReadOnly;
                            }
                        }

                        m_CachedDocument = refDoc;
                    }
                }

                return m_CachedDocument;
            }
            set => throw new NotSupportedException();
        }

        /// <summary>
        /// Resolves the referenced configuration within the referenced model document.
        /// 在引用模型文档中解析该视图对应的配置。
        /// </summary>
        public IXConfiguration ReferencedConfiguration 
        {
            get 
            {
                var confName = DrawingView.ReferencedConfiguration;

                if (!string.IsNullOrEmpty(confName) && confName != "-")
                {
                    if (!ReferencedDocument.IsCommitted)
                    {
                        ReferencedDocument.Commit();
                    }

                    return ReferencedDocument.Configurations.FirstOrDefault(
                        c => string.Equals(c.Name, confName, StringComparison.CurrentCultureIgnoreCase));
                }
                else 
                {
                    return null;
                }
            }
            set => throw new NotSupportedException();
        }

        public override bool IsCommitted => true;

        private readonly SwDmDrawing m_Drw;
        private IXDocument3D m_CachedDocument;

        /// <summary>
        /// Creates the drawing view wrapper and binds it to the owning drawing.
        /// 创建工程图视图包装器，并绑定到所属工程图。
        /// </summary>
        internal SwDmDrawingView(ISwDMView view, SwDmDrawing drw) : base(view, drw.OwnerApplication, drw)
        {
            DrawingView = view;
            m_Drw = drw;
        }
    }
}
