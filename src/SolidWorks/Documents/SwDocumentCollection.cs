//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Xarial.XCad.Base;
using Xarial.XCad.Base.Enums;
using Xarial.XCad.Documents;
using Xarial.XCad.Documents.Delegates;
using Xarial.XCad.Documents.Exceptions;
using Xarial.XCad.Documents.Services;
using Xarial.XCad.Documents.Structures;
using Xarial.XCad.Exceptions;
using Xarial.XCad.SolidWorks.Documents.Services;
using Xarial.XCad.SolidWorks.Utils;
using Xarial.XCad.Toolkit.Services;
using Xarial.XCad.Toolkit.Utils;
using Xarial.XCad.Utils.Diagnostics;

namespace Xarial.XCad.SolidWorks.Documents
{
    /// <summary>
    /// SolidWorks document collection interface that manages all open documents.
    /// <para>中文：SolidWorks 文档集合接口，管理所有已打开的文档，提供激活文档访问、按名称/模型索引等功能。</para>
    /// </summary>
    public interface ISwDocumentCollection : IXDocumentRepository, IDisposable
    {
        /// <summary>
        /// Gets or sets the currently active document.
        /// <para>中文：获取或设置当前激活文档。</para>
        /// </summary>
        new ISwDocument Active { get; set; }

        /// <summary>
        /// Gets a document by its name.
        /// <para>中文：按文档名称获取文档对象。</para>
        /// </summary>
        new IXDocument this[string name] { get; }

        /// <summary>
        /// Gets a document by its native SolidWorks model object.
        /// <para>中文：通过 SolidWorks 原生模型对象（IModelDoc2）获取对应的文档对象。</para>
        /// </summary>
        ISwDocument this[IModelDoc2 model] { get; }
    }

    /// <summary>
    /// Manages all open SolidWorks documents and provides document lifecycle events.
    /// <para>中文：管理所有已打开的 SolidWorks 文档，并提供文档生命周期事件（文档加载、文档激活、新建文档、文档打开）。</para>
    /// </summary>
    [DebuggerDisplay("Documents: {" + nameof(Count) + "}")]
    internal class SwDocumentCollection : ISwDocumentCollection
    {
        /// <summary>
        /// Raised when a document is loaded into SolidWorks.
        /// <para>中文：当文档加载到 SolidWorks 时触发（文档加载事件）。</para>
        /// </summary>
        public event DocumentEventDelegate DocumentLoaded
        {
            add
            {
                if (m_DocumentLoaded == null)
                {
                    m_SwApp.DocumentLoadNotify2 += OnDocumentLoadNotify2;
                }

                m_DocumentLoaded += value;
            }
            remove
            {
                m_DocumentLoaded -= value;

                if (m_DocumentLoaded == null)
                {
                    m_SwApp.DocumentLoadNotify2 -= OnDocumentLoadNotify2;
                }
            }
        }

        /// <summary>
        /// Raised when the active document changes in SolidWorks.
        /// <para>中文：当 SolidWorks 中激活文档切换时触发（文档激活事件）。</para>
        /// </summary>
        public event DocumentEventDelegate DocumentActivated 
        {
            add 
            {
                if (m_DocumentActivated == null) 
                {
                    m_SwApp.ActiveModelDocChangeNotify += OnActiveModelDocChangeNotify;
                }

                m_DocumentActivated += value;
            }
            remove 
            {
                m_DocumentActivated -= value;

                if (m_DocumentActivated == null)
                {
                    m_SwApp.ActiveModelDocChangeNotify -= OnActiveModelDocChangeNotify;
                }
            }
        }

        /// <summary>
        /// Raised when a new document is created in SolidWorks.
        /// <para>中文：当在 SolidWorks 中新建文档时触发（新建文档事件）。</para>
        /// </summary>
        public event DocumentEventDelegate NewDocumentCreated
        {
            add
            {
                if (m_NewDocumentCreated == null)
                {
                    m_SwApp.FileNewNotify2 += OnFileNewNotify;
                }

                m_NewDocumentCreated += value;
            }
            remove
            {
                m_NewDocumentCreated -= value;

                if (m_NewDocumentCreated == null)
                {
                    m_SwApp.FileNewNotify2 -= OnFileNewNotify;
                }
            }
        }

        /// <summary>
        /// Raised when a document is opened (post-open notification) in SolidWorks.
        /// <para>中文：当文档在 SolidWorks 中打开完成后触发（文档打开事件）。</para>
        /// </summary>
        public event DocumentEventDelegate DocumentOpened
        {
            add
            {
                if (m_DocumentOpened == null)
                {
                    m_SwApp.FileOpenPostNotify += OnFileOpenPostNotify;
                }

                m_DocumentOpened += value;
            }
            remove
            {
                m_DocumentOpened -= value;

                if (m_DocumentOpened == null)
                {
                    m_SwApp.FileOpenPostNotify -= OnFileOpenPostNotify;
                }
            }
        }

        IXDocument IXDocumentRepository.Active 
        {
            get => Active;
            set => Active = (SwDocument)value;
        }
        
        private readonly SwApplication m_App;
        private readonly SldWorks m_SwApp;
        private readonly IXLogger m_Logger;
        private readonly DocumentsHandler m_DocsHandler;

        private DocumentEventDelegate m_DocumentLoaded;
        private DocumentEventDelegate m_DocumentActivated;
        private DocumentEventDelegate m_DocumentOpened;
        private DocumentEventDelegate m_NewDocumentCreated;

        //NOTE: Creation of SwDocument has some additional API calls (e.g. subscribing the save event, caching the path)
        //this may have a performance effect when called very often (e.g. within the IXCommandGroup.CommandStateResolve)
        //cahcing of the document allows to reuse the instance and improves the performance
        // 中文：注意：创建 SwDocument 包含额外的 API 调用（如订阅保存事件、缓存文件路径），
        // 中文：在频繁调用时（如命令状态解析）会影响性能；通过缓存文档实例复用对象以提升性能。
        private IModelDoc2 m_CachedNativeDoc;
        private SwDocument m_CachedDoc;

        /// <summary>
        /// Gets or sets the currently active (激活) SolidWorks document.
        /// <para>中文：获取或设置当前激活文档。设置时调用 SolidWorks API 激活指定文档。</para>
        /// </summary>
        public ISwDocument Active
        {
            get
            {
                var activeDoc = m_SwApp.IActiveDoc2;

                if (activeDoc != null)
                {
                    return this[activeDoc];
                }
                else
                {
                    return null;
                }
            }
            set 
            {
                int errors = -1;
                var doc = m_SwApp.ActivateDoc3(value.Title, true, (int)swRebuildOnActivation_e.swDontRebuildActiveDoc,
                    ref errors);

                if (doc == null) 
                {
                    throw new Exception($"Failed to activate the document. Error code: {errors}");
                }
            }
        }

        /// <summary>
        /// Gets the total number of open documents in the current SolidWorks session.
        /// <para>中文：获取当前 SolidWorks 会话中已打开的文档总数。</para>
        /// </summary>
        public int Count => m_SwApp.GetDocumentCount();

        public IXDocument this[string name] => RepositoryHelper.Get(this, name);

        internal SwDocumentCollection(SwApplication app, IXLogger logger)
        {
            m_App = app;
            m_SwApp = (SldWorks)m_App.Sw;
            m_Logger = logger;

            m_DocsHandler = new DocumentsHandler(app, m_Logger);
        }
                
        private int OnActiveModelDocChangeNotify()
        {
            var activeDoc = m_SwApp.IActiveDoc2;

            try
            {
                m_DocumentActivated?.Invoke(CreateDocument(activeDoc));
            }
            catch (Exception ex)
            {
                m_Logger.Log(ex);
            }

            return HResult.S_OK;
        }

        public ISwDocument this[IModelDoc2 model] => CreateDocument(model);

        /// <summary>
        /// Returns an enumerator over all currently open documents.
        /// <para>中文：返回当前所有已打开文档的枚举器，遍历 SolidWorks 中的所有文档集合。</para>
        /// </summary>
        public IEnumerator<IXDocument> GetEnumerator()
        {
            var openDocs = m_SwApp.GetDocuments() as object[];

            if (openDocs != null)
            {
                foreach (IModelDoc2 model in openDocs)
                {
                    yield return CreateDocument(model);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Filters documents in the collection using the provided query filters.
        /// <para>中文：使用提供的查询条件过滤文档集合，支持正序或倒序返回。</para>
        /// </summary>
        public IEnumerable Filter(bool reverseOrder, params RepositoryFilterQuery[] filters) 
            => RepositoryHelper.FilterDefault(this, filters, reverseOrder);

        private int OnFileOpenPostNotify(string fileName)
        {
            try
            {
                m_DocumentOpened?.Invoke(CreateDocument(FindModel(fileName, fileName)));
            }
            catch (Exception ex)
            {
                m_Logger.Log(ex);
            }

            return HResult.S_OK;
        }

        private int OnFileNewNotify(object newDoc, int docType, string templateName)
        {
            try
            {
                m_NewDocumentCreated?.Invoke(CreateDocument((IModelDoc2)newDoc));
            }
            catch (Exception ex)
            {
                m_Logger.Log(ex);
            }

            return HResult.S_OK;
        }

        private int OnDocumentLoadNotify2(string docTitle, string docPath)
        {
            try
            {
                m_DocumentLoaded?.Invoke(CreateDocument(FindModel(docTitle, docPath)));
            }
            catch (Exception ex)
            {
                m_Logger.Log(ex);
            }

            return HResult.S_OK;
        }

        private ModelDoc2 FindModel(string docTitle, string docPath)
        {
            var docName = docPath;

            if (string.IsNullOrEmpty(docName))
            {
                docName = docTitle;
            }

            var foundModel = m_App.Sw.GetOpenDocument(docName);

            if (foundModel != null)
            {
                return foundModel;
            }
            else
            {
                foreach (ModelDoc2 model in m_App.Sw.GetDocuments() as object[] ?? new object[0])
                {
                    if (!string.IsNullOrEmpty(docPath))
                    {
                        if (string.Equals(model.GetPathName(), docPath, StringComparison.CurrentCultureIgnoreCase))
                        {
                            return model;
                        }
                    }
                    else if (!string.IsNullOrEmpty(docTitle))
                    {
                        if (string.Equals(model.GetTitle(), docTitle, StringComparison.CurrentCultureIgnoreCase))
                        {
                            return model;
                        }
                    }
                }
            }

            throw new Exception($"Failed to find the document by title and path: {docTitle} [{docPath}]");
        }

        /// <summary>
        /// Registers a document handler factory for lifecycle event handling.
        /// <para>中文：注册文档事件处理器工厂，用于文档生命周期事件处理（事件处理注册）。</para>
        /// </summary>
        public void RegisterHandler<THandler>(Func<THandler> handlerFact) 
            where THandler : IDocumentHandler
            => m_DocsHandler.RegisterHandler(handlerFact);

        public void UnregisterHandler<THandler>()
            where THandler : IDocumentHandler
            => m_DocsHandler.UnregisterHandler<THandler>();

        public THandler GetHandler<THandler>(IXDocument doc) 
            where THandler : IDocumentHandler
            => m_DocsHandler.GetHandler<THandler>(doc);

        public T PreCreate<T>() where T : IXDocument
        {
            var doc = RepositoryHelper.PreCreate<IXDocument, T>(this,
                () => new SwUnknownDocument(null, m_App, m_Logger, false),
                () => new SwUnknownDocument3D(null, m_App, m_Logger, false),
                () => new SwPart(null, m_App, m_Logger, false),
                () => new SwAssembly(null, m_App, m_Logger, false),
                () => new SwDrawing(null, m_App, m_Logger, false));

            if (!(doc is SwDocument))
            {
                throw new InvalidCastException("Document type must be of type SwDocument");
            }

            return doc;
        }

        /// <summary>
        /// Tries to get an open document by name or path.
        /// <para>中文：尝试按文档名称或路径获取已打开的文档；若未找到则返回 false。</para>
        /// </summary>
        public bool TryGet(string name, out IXDocument ent)
        {
            IModelDoc2 model = m_SwApp.GetOpenDocument(name);

            if (model == null)
            {
                model = m_SwApp.GetOpenDocumentByName(name) as IModelDoc2;
            }

            if (model != null)
            {
                ent = CreateDocument(model);
                return true;
            }
            else 
            {
                ent = null;
                return false;
            }
        }

        /// <summary>
        /// Adds documents to the collection by committing each one.
        /// <para>中文：将文档列表批量添加到集合（逐个提交新建文档）。</para>
        /// </summary>
        public void AddRange(IEnumerable<IXDocument> ents, CancellationToken cancellationToken) => RepositoryHelper.AddRange(ents, cancellationToken);

        /// <summary>
        /// Closes and removes the specified documents from SolidWorks.
        /// <para>中文：关闭并从 SolidWorks 中移除指定的文档列表。</para>
        /// </summary>
        public void RemoveRange(IEnumerable<IXDocument> ents, CancellationToken cancellationToken)
        {
            foreach (var doc in ents.ToArray()) 
            {
                doc.Close();
            }
        }

        /// <summary>
        /// Tries to find an existing open document by its file path.
        /// <para>中文：尝试通过文件路径在已打开的文档集合中查找已存在的文档对象（不区分大小写匹配文件路径）。</para>
        /// </summary>
        internal bool TryFindExistingDocumentByPath(string path, out SwDocument doc)
        {
            if (!string.IsNullOrEmpty(path))
            {
                doc = (SwDocument)this.FirstOrDefault(
                    d => string.Equals(d.Path, path, StringComparison.CurrentCultureIgnoreCase));
            }
            else 
            {
                doc = null;
            }

            return doc != null;
        }

        /// <summary>
        /// Creates or retrieves the typed SwDocument wrapper for the given native model.
        /// <para>中文：根据原生模型类型（零件/装配体/工程图）创建对应的 SwDocument 包装对象，并缓存以提升性能。</para>
        /// </summary>
        private SwDocument CreateDocument(IModelDoc2 nativeDoc)
        {
            //NOTE: see the description of the m_CachedNativeDoc field
            if (nativeDoc != null && m_CachedNativeDoc == nativeDoc)
            {
                return m_CachedDoc;
            }
            else
            {
                SwDocument doc;

                switch (nativeDoc)
                {
                    case IPartDoc part:
                        doc = new SwPart(part, m_App, m_Logger, true);
                        break;

                    case IAssemblyDoc assm:
                        doc = new SwAssembly(assm, m_App, m_Logger, true);
                        break;

                    case IDrawingDoc drw:
                        doc = new SwDrawing(drw, m_App, m_Logger, true);
                        break;

                    default:
                        throw new NotSupportedException($"Invalid cast of '{nativeDoc.GetPathName()}' [{nativeDoc.GetTitle()}] of type '{((object)nativeDoc).GetType().FullName}'. Specific document type: {(swDocumentTypes_e)nativeDoc.GetType()}");
                }

                m_CachedNativeDoc = nativeDoc;
                m_CachedDoc = doc;

                return doc;
            }
        }

        /// <summary>
        /// Disposes the document collection and unsubscribes all SolidWorks event handlers.
        /// <para>中文：释放文档集合资源，取消所有 SolidWorks 事件订阅（文档加载、激活、新建、打开），并释放文档处理器。</para>
        /// </summary>
        public void Dispose()
            m_SwApp.ActiveModelDocChangeNotify -= OnActiveModelDocChangeNotify;
            m_SwApp.FileNewNotify2 -= OnFileNewNotify;
            m_SwApp.FileOpenPostNotify -= OnFileOpenPostNotify;

            m_DocsHandler.Dispose();
        }
    }

    /// <summary>
    /// Additional extension methods for document collections.
    /// <para>中文：文档集合的扩展方法类，提供按文件路径预创建文档的便捷方法。</para>
    /// </summary>
    public static class SwDocumentCollectionExtension 
    {
        /// <summary>
        /// Pre creates new document from path
        /// <para>中文：根据文件路径（文件扩展名）预创建对应类型的 SolidWorks 文档对象（零件文档、装配体文档或工程图文档），并设置文件路径属性。</para>
        /// </summary>
        /// <param name="docsColl">Documents collection</param>
        /// <param name="path"></param>
        /// <returns>Pre-created document</returns>
        public static ISwDocument PreCreateFromPath(this ISwDocumentCollection docsColl, string path)
        {
            var ext = Path.GetExtension(path);

            ISwDocument doc;

            switch (ext.ToLower())
            {
                case ".sldprt":
                case ".sldblk":
                case ".prtdot":
                case ".sldlfp":
                    doc = docsColl.PreCreate<ISwPart>();
                    break;

                case ".sldasm":
                case ".asmdot":
                    doc = docsColl.PreCreate<ISwAssembly>();
                    break;

                case ".slddrw":
                case ".drwdot":
                    doc = docsColl.PreCreate<ISwDrawing>();
                    break;

                default:
                    throw new NotSupportedException("Only native documents are supported");
            }

            doc.Path = path;

            return doc;
        }
    }
}