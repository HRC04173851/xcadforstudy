// -*- coding: utf-8 -*-
// src/Base/Documents/Enums/StandardPaperSize_e.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义工程图图纸的标准纸张尺寸，包括 A 系列和 B、C、D、E 尺寸。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xarial.XCad.Documents.Enums
{
    /// <summary>
    /// Identifies the standard size of the drawing sheet
    /// 标识工程图图纸的标准纸张尺寸
    /// </summary>
    public enum StandardPaperSize_e
    {
        ALandscape,
        APortrait,
        BLandscape,
        CLandscape,
        DLandscape,
        ELandscape,
        A4Landscape,
        A4Portrait,
        A3Landscape,
        A2Landscape,
        A1Landscape,
        A0Landscape
    }
}
