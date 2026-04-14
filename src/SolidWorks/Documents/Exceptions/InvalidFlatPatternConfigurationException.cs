// -*- coding: utf-8 -*-
// src/SolidWorks/Documents/Exceptions/InvalidFlatPatternConfigurationException.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本异常在展开图视图引用的配置无效时抛出。
// 通常是因为零件文件中的 SM-FLAT-PATTERN 配置状态不正确，
// 导致展开图视图无法正确显示。该视图关联的图纸页也会被标记为无效。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xarial.XCad.Exceptions;

namespace Xarial.XCad.SolidWorks.Documents.Exceptions
{
    /// <summary>
    /// Indicates that flat pattern view does not refer the correct configuration
    /// </summary>
    public class InvalidFlatPatternConfigurationException : Exception, IUserException
    {
        /// <summary>
        /// Failed drawing view
        /// </summary>
        public ISwDrawingView View { get; }

        internal InvalidFlatPatternConfigurationException(Exception innerException, ISwDrawingView view)
            : base("The flat pattern drawing view is invalid as it does not contain the flat pattern feature in the flattened state. This is usually caused by an invalid SM-FLAT-PATTERN configuration in the part file. Try removing this configuration", innerException)
        {
            View = view;
        }
    }
}
