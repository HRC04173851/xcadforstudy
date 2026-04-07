//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using Xarial.XCad.Base;
using Xarial.XCad.Documents.Delegates;
using Xarial.XCad.Documents.Services;
using Xarial.XCad.Documents.Structures;

namespace Xarial.XCad.Documents
{
    /// <summary>
    /// Represents the collection of documents for this application
    /// <para>中文：表示应用程序的文档集合存储库</para>
    /// </summary>
    public interface IXDocumentRepository : IXRepository<IXDocument>
    {
        /// <summary>
        /// Fired when document is activated
        /// <para>中文：文档被激活（切换为当前文档）时触发</para>
        /// </summary>
        event DocumentEventDelegate DocumentActivated;
        
        /// <summary>
        /// Fired when new document is loaded (opened or new document is created)
        /// <para>中文：新文档被加载（打开或新建）时触发</para>
        /// </summary>
        /// <remarks>This event is fired for all referenced documents (e.g. assembly components or drawing view referenced models)
        /// Document might not be fully loaded at this point
        /// This event is fired before <see cref="DocumentOpened"/> and <see cref="NewDocumentCreated"/>
        /// </remarks>
        event DocumentEventDelegate DocumentLoaded;

        /// <summary>
        /// Fired when top-level document is opened
        /// <para>中文：顶层文档（零件、装配体或工程图）被打开时触发</para>
        /// </summary>
        /// <remarks>Unlike <see cref="DocumentLoaded"/> event, this even will only be fired for the top document (part, assembly or drawing) but not for the references. This event is fired after the <see cref="DocumentLoaded"/></remarks>
        event DocumentEventDelegate DocumentOpened;

        /// <summary>
        /// Fired when new document is created
        /// <para>中文：新文档被创建时触发</para>
        /// </summary>
        /// <remarks>This event is fired after the <see cref="DocumentLoaded"/></remarks>
        event DocumentEventDelegate NewDocumentCreated;

        /// <summary>
        /// Returns the pointer to active document
        /// <para>中文：返回当前激活文档的指针</para>
        /// </summary>
        IXDocument Active { get; set; }

        /// <summary>
        /// Registers document handler
        /// <para>中文：注册文档处理程序</para>
        /// </summary>
        /// <param name="handlerFact">Handler factory</param>
        /// <typeparam name="THandler"></typeparam>
        void RegisterHandler<THandler>(Func<THandler> handlerFact)
            where THandler : IDocumentHandler;

        /// <summary>
        /// Unregisters document handler
        /// <para>中文：取消注册文档处理程序</para>
        /// </summary>
        /// <typeparam name="THandler"></typeparam>
        void UnregisterHandler<THandler>() where THandler : IDocumentHandler;

        /// <summary>
        /// Returns the handler for this document
        /// <para>中文：返回指定文档的处理程序实例</para>
        /// </summary>
        /// <typeparam name="THandler">Handler type</typeparam>
        /// <param name="doc">Document to get handler from</param>
        /// <returns>Instance of the handler</returns>
        THandler GetHandler<THandler>(IXDocument doc) where THandler : IDocumentHandler;
    }
}