//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using SolidWorks.Interop.swdocumentmgr;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xarial.XCad.Data;
using Xarial.XCad.Documents;
using Xarial.XCad.SwDocumentManager.Documents;
using Xarial.XCad.SwDocumentManager.Features;
using Xarial.XCad.Toolkit.Data;

namespace Xarial.XCad.SwDocumentManager
{
    /// <summary>
    /// Base xCAD object backed by a Document Manager dispatch.
    /// 基于 Document Manager 调度对象（Dispatch）的 xCAD 基础对象。
    /// </summary>
    public interface ISwDmObject : IXObject
    {
        /// <summary>
        /// Native COM dispatch pointer or raw wrapped object.
        /// 原生 COM 调度指针或被包装的底层对象。
        /// </summary>
        object Dispatch { get; }
    }

    /// <summary>
    /// Default implementation shared by most Document Manager wrappers.
    /// 大多数 Document Manager 包装类型共用的默认实现，负责保存所有权关系与标签容器。
    /// </summary>
    internal class SwDmObject : ISwDmObject
    {
        #region NotSuppoted

        public virtual bool IsAlive => throw new NotSupportedException();

        public virtual void Serialize(Stream stream)
            => throw new NotSupportedException();

        #endregion

        IXApplication IXObject.OwnerApplication => OwnerApplication;
        IXDocument IXObject.OwnerDocument => OwnerDocument;

        internal SwDmApplication OwnerApplication { get; }
        internal SwDmDocument OwnerDocument { get; }

        public ITagsManager Tags => m_TagsLazy.Value;

        private readonly Lazy<ITagsManager> m_TagsLazy;

        /// <summary>
        /// Stores the raw COM dispatch and its owning application/document context.
        /// 保存原始 COM 调度对象以及其所属的应用程序、文档上下文。
        /// </summary>
        public SwDmObject(object disp, SwDmApplication ownerApp, SwDmDocument ownerDoc)
        {
            Dispatch = disp;

            OwnerApplication = ownerApp;
            OwnerDocument = ownerDoc;

            m_TagsLazy = new Lazy<ITagsManager>(() => new LocalTagsManager());
        }

        public virtual object Dispatch { get; }

        /// <summary>
        /// Two wrappers are considered equal when they point to the same underlying dispatch object.
        /// 当两个包装器引用同一个底层调度对象时，将其视为同一 xCAD 对象。
        /// </summary>
        public virtual bool Equals(IXObject other)
        {
            if (other is ISwDmObject)
            {
                return (other as ISwDmObject).Dispatch == Dispatch;
            }
            else 
            {
                return false;
            }
        }
    }

    /// <summary>
    /// Converts raw Document Manager dispatch objects into strongly typed xCAD wrappers.
    /// 把原始 Document Manager 调度对象转换为强类型的 xCAD 包装器。
    /// </summary>
    public static class SwDmObjectFactory 
    {
        internal static TObj FromDispatch<TObj>(object disp, SwDmDocument doc)
            where TObj : ISwDmObject
        {
            return (TObj)FromDispatch(disp, doc, doc.OwnerApplication);
        }

        private static ISwDmObject FromDispatch(object disp, SwDmDocument doc, SwDmApplication app)
        {
            switch (disp) 
            {
                case ISwDMConfiguration conf:
                    // 根据所属文档类型，把配置映射为零件配置或装配体配置包装器。
                    switch (doc) 
                    {
                        case SwDmAssembly assm:
                            return new SwDmAssemblyConfiguration(conf, assm);

                        case SwDmPart part:
                            return new SwDmPartConfiguration(conf, part);

                        default:
                            throw new NotSupportedException("This document type is not supported for configuration");
                    }

                case ISwDMCutListItem cutList:
                    return new SwDmCutListItem((ISwDMCutListItem2)cutList, (SwDmPart)doc);

                case ISwDMComponent comp:
                    var ext = Path.GetExtension(((ISwDMComponent6)comp).PathName);
                    // 组件包装器依赖被引用文件的 CAD 文档类型来决定具体实现。
                    switch (ext.ToLower()) 
                    {
                        case ".sldprt":
                            return new SwDmPartComponent((SwDmAssembly)doc, comp);

                        case ".sldasm":
                            return new SwDmAssemblyComponent((SwDmAssembly)doc, comp);

                        default:
                            throw new NotSupportedException();
                    }

                case ISwDMSheet sheet:
                    return new SwDmSheet(sheet, (SwDmDrawing)doc);

                case ISwDMView view:
                    return new SwDmDrawingView(view, (SwDmDrawing)doc);

                default:
                    return new SwDmObject(disp, app, doc);
            }
        }
    }
}
