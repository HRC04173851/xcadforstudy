// -*- coding: utf-8 -*-
// src/Base/UI/PropertyPage/Attributes/HelpAttribute.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 为页面提供附加帮助链接，支持帮助文档和新功能说明链接。
//*********************************************************************

using System;
using Xarial.XCad.UI.PropertyPage.Base;

namespace Xarial.XCad.UI.PropertyPage.Attributes
{
    /// <summary>
    /// Provides the additional help links for the page
    /// 为页面提供附加帮助链接
    /// </summary>
    /// <remarks>Applied to the model class</remarks>
    public class HelpAttribute : Attribute, IAttribute
    {
        /// <summary>
        /// Link to a help file
        /// </summary>
        /// <remarks>Thsi can be either url or local path (absolute or relative)</remarks>
        public string HelpLink { get; }

        /// <summary>
        /// Link to what's new documentation
        /// </summary>
        /// <remarks>Thsi can be either url or local path (absolute or relative)</remarks>
        public string WhatsNewLink { get; }

        /// <summary>
        /// Constructor for specifying links to help resources
        /// </summary>
        /// <param name="helpLink">Link to help documentation</param>
        /// <param name="whatsNewLink">Link to what's new page</param>
        public HelpAttribute(string helpLink, string whatsNewLink = "")
        {
            HelpLink = helpLink;
            WhatsNewLink = whatsNewLink;
        }
    }
}