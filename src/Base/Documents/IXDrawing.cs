//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using Xarial.XCad.Documents.Structures;

namespace Xarial.XCad.Documents
{
    /// <summary>
    /// Represents the drawing (2D draft)
    /// <para>中文：表示工程图文档（二维制图）</para>
    /// </summary>
    public interface IXDrawing : IXDocument
    {
        /// <summary>
        /// Sheets on this drawing
        /// <para>中文：此工程图中的图纸集合</para>
        /// </summary>
        IXSheetRepository Sheets { get; }

        /// <summary>
        /// Drawing layers
        /// <para>中文：工程图图层集合</para>
        /// </summary>
        IXLayerRepository Layers { get; }

        /// <summary>
        /// Drawing specific options
        /// <para>中文：工程图专有选项</para>
        /// </summary>
        new IXDrawingOptions Options { get; }

        /// <summary>
        /// <see cref="IXDrawing"/> specific save as operation
        /// <para>中文：工程图专用的另存为操作</para>
        /// </summary>
        new IXDrawingSaveOperation PreCreateSaveAsOperation(string filePath);
    }
}