// -*- coding: utf-8 -*-
// src/SolidWorks/Services/ICalloutHandlerProvider.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义标注处理器提供程序接口，用于创建和管理文档图形中的标注处理器，支持自定义标注的显示和行为。
//*********************************************************************

using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.SolidWorks.Graphics;
using Xarial.XCad.SolidWorks.UI;

namespace Xarial.XCad.SolidWorks.Services
{
    /// <summary>
    /// Handler for the callout when used with <see cref="XCad.Documents.IXDocumentGraphics.PreCreateCallout"/>
    /// </summary>
    public interface ICalloutHandlerProvider
    {
        ///<summary> This function is called when new handler instance needs to be created</summary>
        /// <param name="app">Pointer to SOLIDWORKS application</param>
        /// <returns>Callout handler</returns>
        /// <remarks>The class must be com-visible. Provide new instance of the handler with each call</remarks>
        SwCalloutBaseHandler CreateHandler(ISwApplication app);
    }

    internal class NotSetCalloutHandlerProvider : ICalloutHandlerProvider
    {
        public SwCalloutBaseHandler CreateHandler(ISwApplication app)
            => throw new Exception($"{nameof(ICalloutHandlerProvider)} service is not registered. Configure this service within the {nameof(SwAddInEx)}::OnConfigureServices which returns the COM-visible instance of {nameof(SwCalloutBaseHandler)}");
    }
}
