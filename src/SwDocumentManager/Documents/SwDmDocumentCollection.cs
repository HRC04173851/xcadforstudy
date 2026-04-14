// -*- coding: utf-8 -*-
// src/SwDocumentManager/Documents/SwDmDocumentCollection.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 跟踪 Document Manager 应用实例所拥有的全部文档包装器，支持按路径、文件名或标题解析文档。
//*********************************************************************

using SolidWorks.Interop.swdocumentmgr;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Xarial.XCad.Base;
using Xarial.XCad.Documents;
using Xarial.XCad.Documents.Delegates;
using Xarial.XCad.Documents.Services;
using Xarial.XCad.Exceptions;
using Xarial.XCad.Toolkit.Utils;

namespace Xarial.XCad.SwDocumentManager.Documents
{
    /// <summary>
    /// Repository contract for opened or pre-created Document Manager documents.
    /// 已打开或预创建的 Document Manager 文档仓库约定。
    /// </summary>
    public interface ISwDmDocumentCollection : IXDocumentRepository, IDisposable 
    {
        bool TryGet(string name, out ISwDmDocument ent);
        new ISwDmDocument this[string name] { get; }
        new ISwDmDocument Active { get; set; }
    }

    /// <summary>
    /// Tracks all document wrappers owned by a Document Manager application instance.
    /// 跟踪某个 Document Manager 应用实例所拥有的全部文档包装器。
    /// </summary>
    internal class SwDmDocumentCollection : ISwDmDocumentCollection
    {
        #region NotSupported
        
        public event DocumentEventDelegate DocumentActivated 
        {
            add => throw new NotSupportedException();
            remove => throw new NotSupportedException();
        }

        public event DocumentEventDelegate DocumentLoaded
        {
            add => throw new NotSupportedException();
            remove => throw new NotSupportedException();
        }

        public event DocumentEventDelegate DocumentOpened
        {
            add => throw new NotSupportedException();
            remove => throw new NotSupportedException();
        }

        public event DocumentEventDelegate NewDocumentCreated
        {
            add => throw new NotSupportedException();
            remove => throw new NotSupportedException();
        }

        #endregion

        IXDocument IXRepository<IXDocument>.this[string name] => this[name];

        IXDocument IXDocumentRepository.Active
        {
            get => Active;
            set => Active = (ISwDmDocument)value;
        }

        bool IXRepository<IXDocument>.TryGet(string name, out IXDocument ent)
        {
            var res = this.TryGet(name, out ISwDmDocument doc);
            ent = doc;
            return res;
        }

        public ISwDmDocument this[string name] => (ISwDmDocument)RepositoryHelper.Get(this, name);

        private ISwDmDocument m_Active;

        /// <summary>
        /// Active document in the repository; this is a logical xCAD concept rather than a UI selection.
        /// 仓库中的当前活动文档；这里表示的是逻辑活动对象，而不是界面中的前台窗口。
        /// </summary>
        public ISwDmDocument Active
        {
            get => m_Active;
            set
            {
                if (value == null || m_Documents.Contains(value))
                {
                    m_Active = value;
                }
                else
                {
                    throw new Exception("Document does not belong to documents repository");
                }
            }
        }

        public int Count => m_Documents.Count;

        private List<ISwDmDocument> m_Documents;

        private readonly SwDmApplication m_DmApp;

        /// <summary>
        /// Creates a repository bound to the owning Document Manager application wrapper.
        /// 创建绑定到所属 Document Manager 应用包装器的文档仓库。
        /// </summary>
        internal SwDmDocumentCollection(SwDmApplication dmApp)
        {
            m_DmApp = dmApp;
            m_Documents = new List<ISwDmDocument>();
        }

        public void AddRange(IEnumerable<IXDocument> ents, CancellationToken cancellationToken) => RepositoryHelper.AddRange(ents, cancellationToken);

        public IEnumerator<IXDocument> GetEnumerator() => m_Documents.GetEnumerator();

        public THandler GetHandler<THandler>(IXDocument doc) where THandler : IDocumentHandler => throw new NotImplementedException();

        public void RegisterHandler<THandler>(Func<THandler> handlerFact) where THandler : IDocumentHandler => throw new NotImplementedException();

        public void UnregisterHandler<THandler>() where THandler : IDocumentHandler => throw new NotImplementedException();

        public void RemoveRange(IEnumerable<IXDocument> ents, CancellationToken cancellationToken)
        {
            foreach (var doc in ents)
            {
                doc.Close();
            }
        }

        /// <summary>
        /// Resolves an already tracked document by path, file name, or title.
        /// 按完整路径、文件名或标题解析已跟踪的文档。
        /// </summary>
        public bool TryGet(string name, out ISwDmDocument ent)
        {
            if (System.IO.Path.IsPathRooted(name))
            {
                ent = m_Documents.FirstOrDefault(
                    d => string.Equals(d.Path, name, StringComparison.CurrentCultureIgnoreCase));
            }
            else if (System.IO.Path.HasExtension(name))
            {
                ent = m_Documents.FirstOrDefault(
                    d => string.Equals(System.IO.Path.GetFileName(d.Path), name,
                    StringComparison.CurrentCultureIgnoreCase));
            }
            else
            {
                ent = m_Documents.FirstOrDefault(
                    d => string.Equals(System.IO.Path.GetFileNameWithoutExtension(d.Path),
                    name, StringComparison.CurrentCultureIgnoreCase));
            }

            if (ent?.IsAlive == false) 
            {
                ent.Close();
                ent = null;
            }

            return ent != null;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerable Filter(bool reverseOrder, params RepositoryFilterQuery[] filters) => RepositoryHelper.FilterDefault(this, filters, reverseOrder);

        /// <summary>
        /// Pre-creates a document wrapper of the requested xCAD type.
        /// 按请求的 xCAD 类型预创建文档包装器。
        /// </summary>
        public T PreCreate<T>() where T : IXDocument
            => RepositoryHelper.PreCreate<IXDocument, T>(this,
                () => new SwDmUnknownDocument(m_DmApp, null, false, OnDocumentCreated, OnDocumentClosed, null),
                () => new SwDmUnknownDocument3D(m_DmApp, null, false, OnDocumentCreated, OnDocumentClosed, null),
                () => new SwDmPart(m_DmApp, null, false, OnDocumentCreated, OnDocumentClosed, null),
                () => new SwDmAssembly(m_DmApp, null, false, OnDocumentCreated, OnDocumentClosed, null),
                () => new SwDmDrawing(m_DmApp, null, false, OnDocumentCreated, OnDocumentClosed, null));

        /// <summary>
        /// Registers a newly created document and updates the active document pointer.
        /// 注册新创建的文档，并更新当前活动文档指针。
        /// </summary>
        internal void OnDocumentCreated(ISwDmDocument doc)
        {
            m_Documents.Add(doc);
            Active = doc;
        }

        /// <summary>
        /// Unregisters a closed document and promotes the first remaining document as active.
        /// 注销已关闭文档，并将剩余列表中的第一个文档设为活动文档。
        /// </summary>
        internal void OnDocumentClosed(ISwDmDocument doc)
        {
            m_Documents.Remove(doc);
            Active = m_Documents.FirstOrDefault();
        }

        public void Dispose()
        {
            foreach (var doc in m_Documents) 
            {
                doc.Close();
            }
        }
    }

    /// <summary>
    /// Helper extensions for pre-creating a document from a file path.
    /// 根据文件路径预创建文档的辅助扩展方法。
    /// </summary>
    public static class ISwDmDocumentCollectionExtension 
    {
        /// <summary>
        /// Chooses the proper document wrapper type from the SOLIDWORKS file extension.
        /// 根据 SOLIDWORKS 文件扩展名选择合适的文档包装器类型。
        /// </summary>
        public static ISwDmDocument PreCreateFromPath(this ISwDmDocumentCollection docs, string path) 
        {
            ISwDmDocument doc;

            switch (SwDmDocument.GetDocumentType(path)) 
            {
                case SwDmDocumentType.swDmDocumentPart:
                    doc = docs.PreCreate<ISwDmPart>();
                    break;

                case SwDmDocumentType.swDmDocumentAssembly:
                    doc = docs.PreCreate<ISwDmAssembly>();
                    break;

                case SwDmDocumentType.swDmDocumentDrawing:
                    doc = docs.PreCreate<ISwDmDrawing>();
                    break;

                default:
                    throw new NotSupportedException("Document type is not supported");
            }

            doc.Path = path;
            return doc;
        }
    }
}
