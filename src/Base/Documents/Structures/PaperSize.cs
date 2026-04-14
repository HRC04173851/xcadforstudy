// -*- coding: utf-8 -*-
// src/Base/Documents/Structures/PaperSize.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义工程图图纸尺寸，支持A0-A4、标准纸张尺寸以及自定义宽高
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xarial.XCad.Documents.Enums;
using System.Diagnostics;

namespace Xarial.XCad.Documents.Structures
{
    /// <summary>
    /// Defines the size of the drawing sheet paper
    /// 定义工程图图纸纸张尺寸
    /// </summary>
    [DebuggerDisplay("{" + nameof(StandardPaperSize) + "}" + " ({" + nameof(Width) + "} x {" + nameof(Height) + "}")]
    public class PaperSize
    {
        /// <summary>
        /// Standard paper size or null if custom
        /// 标准纸张尺寸；若为自定义则为 null
        /// </summary>
        public StandardPaperSize_e? StandardPaperSize { get; }
        
        /// <summary>
        /// Width of the paper
        /// 纸张宽度
        /// </summary>
        public double Width { get; }

        /// <summary>
        /// Height of the paper
        /// 纸张高度
        /// </summary>
        public double Height { get; }

        /// <summary>
        /// Standard paper size constructor
        /// </summary>
        public PaperSize(StandardPaperSize_e standardPaperSize)
        {
            StandardPaperSize = standardPaperSize;

            switch (standardPaperSize)
            {
                case StandardPaperSize_e.ALandscape:
                    Width = 0.279;
                    Height = 0.2159;
                    break;
                case StandardPaperSize_e.APortrait:
                    Width = 0.2159;
                    Height = 0.279;
                    break;
                case StandardPaperSize_e.BLandscape:
                    Width = 0.4318;
                    Height = 0.2794;
                    break;
                case StandardPaperSize_e.CLandscape:
                    Width = 0.5588;
                    Height = 0.4318;
                    break;
                case StandardPaperSize_e.DLandscape:
                    Width = 0.8636;
                    Height = 0.5588;
                    break;
                case StandardPaperSize_e.ELandscape:
                    Width = 1.1176;
                    Height = 0.8636;
                    break;
                case StandardPaperSize_e.A4Landscape:
                    Width = 0.297;
                    Height = 0.21;
                    break;
                case StandardPaperSize_e.A4Portrait:
                    Width = 0.21;
                    Height = 0.297;
                    break;
                case StandardPaperSize_e.A3Landscape:
                    Width = 0.42;
                    Height = 0.297;
                    break;
                case StandardPaperSize_e.A2Landscape:
                    Width = 0.594;
                    Height = 0.42;
                    break;
                case StandardPaperSize_e.A1Landscape:
                    Width = 0.841;
                    Height = 0.594;
                    break;
                case StandardPaperSize_e.A0Landscape:
                    Width = 1.189;
                    Height = 0.841;
                    break;
            }
        }

        /// <summary>
        /// Custom paper size constructor
        /// </summary>
        ///<inheritdoc/>
        public PaperSize(double width, double height) : this(null, width, height)
        {
        }

        /// <summary>
        /// Constructor with all parameters
        /// </summary>
        /// <param name="standardPaperSize">Standard paper size</param>
        /// <param name="width">Custom width</param>
        /// <param name="height">Custom height</param>
        public PaperSize(StandardPaperSize_e? standardPaperSize, double width, double height)
        {
            StandardPaperSize = standardPaperSize;
            Width = width;
            Height = height;
        }
    }
}
