//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using SolidWorks.Interop.swdocumentmgr;
using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Documents;
using Xarial.XCad.Documents.Structures;
using Xarial.XCad.Geometry.Structures;

namespace Xarial.XCad.SwDocumentManager.Documents
{
    /// <summary>
    /// Drawing document contract.
    /// 工程图文档约定。
    /// </summary>
    public interface ISwDmDrawing : ISwDmDocument, IXDrawing
    {
        new ISwDmSheetCollection Sheets { get; }
    }

    /// <summary>
    /// Drawing document wrapper that exposes sheets through xCAD.
    /// 工程图文档包装器，通过 xCAD 暴露图纸页集合。
    /// </summary>
    internal class SwDmDrawing : SwDmDocument, ISwDmDrawing
    {
        #region Not Supported
        
        IXDrawingOptions IXDrawing.Options => throw new NotSupportedException();
        IXDrawingSaveOperation IXDrawing.PreCreateSaveAsOperation(string filePath) => throw new NotSupportedException();
        public IXLayerRepository Layers => throw new NotSupportedException();

        #endregion

        IXSheetRepository IXDrawing.Sheets => Sheets;

        private readonly Lazy<SwDmSheetCollection> m_SheetsLazy;

        /// <summary>
        /// Initializes the drawing wrapper and delays sheet enumeration until requested.
        /// 初始化工程图包装器，并在真正访问时再延迟枚举图纸页。
        /// </summary>
        public SwDmDrawing(SwDmApplication dmApp, ISwDMDocument doc, bool isCreated,
            Action<ISwDmDocument> createHandler, Action<ISwDmDocument> closeHandler,
            bool? isReadOnly)
            : base(dmApp, doc, isCreated, createHandler, closeHandler, isReadOnly)
        {
            m_SheetsLazy = new Lazy<SwDmSheetCollection>(() => new SwDmSheetCollection(this));
        }

        public ISwDmSheetCollection Sheets => m_SheetsLazy.Value;

        protected override bool IsDocumentTypeCompatible(SwDmDocumentType docType) => docType == SwDmDocumentType.swDmDocumentDrawing;        
    }
}
