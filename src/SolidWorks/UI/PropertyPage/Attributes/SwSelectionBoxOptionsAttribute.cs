// -*- coding: utf-8 -*-
// PropertyPage/Attributes/SwSelectionBoxOptionsAttribute.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// SOLIDWORKS特定的选择框选项属性，用于配置选择框的实体过滤器。
//*********************************************************************

using SolidWorks.Interop.swconst;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xarial.XCad.UI.PropertyPage.Attributes;

namespace Xarial.XCad.SolidWorks.UI.PropertyPage.Attributes
{
    /// <summary>
    /// SOLIDWORKS specific selection box control
    /// </summary>
    public class SwSelectionBoxOptionsAttribute : SelectionBoxOptionsAttribute
    {
        /// <summary>
         /// Allowed entities filter for the selection
         /// </summary>
        public new swSelectType_e[] Filters { get; set; }

        /// <summary>
        /// Default constructors
        /// </summary>
        /// <param name="filters">Array of selection filters</param>
        public SwSelectionBoxOptionsAttribute(params swSelectType_e[] filters)
        {
            Filters = filters;
        }
    }
}
