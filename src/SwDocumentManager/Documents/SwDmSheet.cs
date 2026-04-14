// -*- coding: utf-8 -*-
// src/SwDocumentManager/Documents/SwDmSheet.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 单张图纸页的包装器实现，包含比例、图幅尺寸和视图集合等工程图特有数据的访问。
//*********************************************************************

using SolidWorks.Interop.swdocumentmgr;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Xarial.XCad.Documents;
using Xarial.XCad.Documents.Enums;
using Xarial.XCad.Documents.Structures;
using Xarial.XCad.Features;
using Xarial.XCad.UI;

namespace Xarial.XCad.SwDocumentManager.Documents
{
    /// <summary>
    /// Drawing sheet contract.
    /// 图纸页约定。
    /// </summary>
    public interface ISwDmSheet : IXSheet, ISwDmObject
    {
        ISwDMSheet Sheet { get; }
    }

    /// <summary>
    /// Wrapper for a single drawing sheet, including scale, paper size, and views.
    /// 单张图纸页的包装器，包含比例、图幅和视图集合等信息。
    /// </summary>
    internal class SwDmSheet : SwDmSelObject, ISwDmSheet
    {
        #region Not Supported
        public IXSheet Clone(IXDrawing targetDrawing) => throw new NotSupportedException();
        public IXSketch2D Sketch => throw new NotSupportedException();
        #endregion

        public string Name
        {
            get => Sheet.Name;
            set => Sheet.Name = value;
        }

        public IXDrawingViewRepository DrawingViews => m_DrawingViewsLazy.Value;

        /// <summary>
        /// Reads the sheet scale from the low-level sheet property array.
        /// 从底层图纸页属性数组中读取图纸比例。
        /// </summary>
        public Scale Scale 
        {
            get 
            {
                if (((ISwDMDocument13)m_Drawing.Document).GetSheetProperties(Name, out object prps) == (int)swSheetPropertiesResult.swSheetProperties_TRUE)
                {
                    var prpsArr = (double[])prps;

                    return new Scale(prpsArr[3], prpsArr[4]);
                }
                else 
                {
                    throw new Exception("Failed to read sheet properties");
                }
            }
            set => throw new NotSupportedException(); 
        }
        
        /// <summary>
        /// Reads the paper size, including custom width and height for user-defined formats.
        /// 读取图纸幅面；若为自定义图幅，则同时返回宽度与高度。
        /// </summary>
        public PaperSize PaperSize 
        {
            get
            {
                if (((ISwDMDocument13)m_Drawing.Document).GetSheetProperties(Name, out object prps) == (int)swSheetPropertiesResult.swSheetProperties_TRUE)
                {
                    var prpsArr = (double[])prps;

                    const int swDwgPapersUserDefined = 12;

                    var paperSize = Convert.ToInt32(prpsArr[0]);

                    var standardPaperSize = paperSize == swDwgPapersUserDefined ? default(StandardPaperSize_e?) : (StandardPaperSize_e)paperSize;

                    return new PaperSize(standardPaperSize, prpsArr[1], prpsArr[2]);
                }
                else
                {
                    throw new Exception("Failed to read sheet properties");
                }
            }
            set => throw new NotSupportedException(); 
        }

        /// <summary>
        /// Extracts the preview image stored for the sheet.
        /// 提取该图纸页保存的预览图像。
        /// </summary>
        public IXImage Preview 
        {
            get 
            {
                SwDmPreviewError previewErr;
                var imgBytes = ((ISwDMSheet2)Sheet).GetPreviewPNGBitmapBytes(out previewErr) as byte[];

                if (previewErr == SwDmPreviewError.swDmPreviewErrorNone)
                {
                    return new BaseImage(imgBytes);
                }
                else
                {
                    throw new Exception($"Failed to extract preview from the sheet: {previewErr}");
                }
            }
        }

        public ISwDMSheet Sheet { get; }
        
        private readonly Lazy<SwDmDrawingViewsCollection> m_DrawingViewsLazy;

        private readonly SwDmDrawing m_Drawing;

        /// <summary>
        /// Creates the sheet wrapper and lazily binds the drawing view repository.
        /// 创建图纸页包装器，并延迟绑定工程图视图仓库。
        /// </summary>
        internal SwDmSheet(ISwDMSheet sheet, SwDmDrawing drw) : base(sheet, drw.OwnerApplication, drw)
        {
            Sheet = sheet;
            m_Drawing = drw;

            m_DrawingViewsLazy = new Lazy<SwDmDrawingViewsCollection>(() => new SwDmDrawingViewsCollection(this, drw));
        }
    }
}
