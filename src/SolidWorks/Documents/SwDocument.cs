// -*- coding: utf-8 -*-
// src/SolidWorks/Documents/SwDocument.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件是 SolidWorks 文档的基类实现，封装了 SolidWorks 文档操作的常见逻辑。
// 负责文档的创建、打开、关闭、保存、事件管理等核心功能。
// 所有特定文档类型（零件、装配体、工程图）都继承自此类。
//*********************************************************************

using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using Xarial.XCad.Annotations;
using Xarial.XCad.Base;
using Xarial.XCad.Data;
using Xarial.XCad.Data.Enums;
using Xarial.XCad.Documents;
using Xarial.XCad.Documents.Delegates;
using Xarial.XCad.Documents.Enums;
using Xarial.XCad.Documents.Exceptions;
using Xarial.XCad.Documents.Services;
using Xarial.XCad.Documents.Structures;
using Xarial.XCad.Exceptions;
using Xarial.XCad.Features;
using Xarial.XCad.Geometry;
using Xarial.XCad.Services;
using Xarial.XCad.SolidWorks.Annotations;
using Xarial.XCad.SolidWorks.Data;
using Xarial.XCad.SolidWorks.Data.EventHandlers;
using Xarial.XCad.SolidWorks.Documents.EventHandlers;
using Xarial.XCad.SolidWorks.Documents.Exceptions;
using Xarial.XCad.SolidWorks.Documents.Services;
using Xarial.XCad.SolidWorks.Enums;
using Xarial.XCad.SolidWorks.Features;
using Xarial.XCad.SolidWorks.UI;
using Xarial.XCad.SolidWorks.Utils;
using Xarial.XCad.Toolkit.Data;
using Xarial.XCad.Toolkit.Utils;
using Xarial.XCad.UI;

namespace Xarial.XCad.SolidWorks.Documents
{
    /// <summary>
    /// SolidWorks 文档的统一接口。
    /// <para>
    /// 职责边界：
    /// <list type="bullet">
    /// <item><description>提供对底层 SolidWorks IModelDoc2 COM 对象的访问</description></item>
    /// <item><description>管理文档生命周期（创建、打开、关闭、保存）</description></item>
    /// <item><description>提供特性、选择集、尺寸、属性等子系统的统一访问</description></item>
    /// </list>
    /// 不负责：文档的 UI 呈现、多文档管理（由 SwDocumentCollection 负责）
    /// </para>
    /// <para>
    /// 使用场景：
    /// <list type="bullet">
    /// <item><description>打开或创建 SolidWorks 文档后的统一操作入口</description></item>
    /// <item><description>跨文档类型（如零件、装配体、工程图）的通用操作</description></item>
    /// </list>
    /// </para>
    /// </summary>
    public interface ISwDocument : ISwObject, IXDocument, IDisposable
    {
        /// <summary>
        /// 获取底层 SolidWorks IModelDoc2 COM 对象。
        /// <para>中文：这是与 SolidWorks 交互的核心接口，几乎所有文档操作都通过此对象完成。</para>
        /// </summary>
        IModelDoc2 Model { get; }
        new ISwFeatureManager Features { get; }
        new ISwSelectionCollection Selections { get; }
        new ISwDimensionsCollection Dimensions { get; }
        new ISwCustomPropertiesCollection Properties { get; }
        new ISwVersion Version { get; }
        new TSwObj DeserializeObject<TSwObj>(Stream stream)
            where TSwObj : ISwObject;
        
        /// <summary>
        /// Creates xCAD object from a SOLIDWORKS dispatch object
        /// </summary>
        /// <typeparam name="TObj">Type of xCAD object</typeparam>
        /// <param name="disp">SOLIDWORKS specific COM object instance</param>
        /// <returns>xCAD object</returns>
        TObj CreateObjectFromDispatch<TObj>(object disp)
            where TObj : ISwObject;
    }

    /// <summary>
    /// SolidWorks 文档的抽象基类。
    /// <para>
    /// 核心职责：
    /// <list type="bullet">
    /// <item><description>实现 ISwDocument 接口，提供文档的创建、打开、关闭、保存等基础操作</description></item>
    /// <item><description>通过 ElementCreator 模式管理文档生命周期，支持延迟创建和缓存</description></item>
    /// <item><description>处理 3D Interconnect 禁用逻辑，确保外部导入的 CAD 文件能正常操作</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// 事件管理：
    /// <list type="bullet">
    /// <item><description>Destroy 事件：文档销毁/关闭时触发</description></item>
    /// <item><description>Rebuild 事件：文档重建后触发</description></item>
    /// <item><description>Save 事件：文档保存前后触发</description></item>
    /// </list>
    /// </para>
    /// </summary>
    [DebuggerDisplay("{" + nameof(Title) + "}")]
    internal abstract class SwDocument : SwObject, ISwDocument
    {
        /// <summary>
        /// 3D Interconnect 禁用助手。
        /// <para>
        /// 为什么需要此类：SolidWorks 2020+ 支持 3D Interconnect 功能，
        /// 会自动将外部 CAD 格式（如 STEP、IGES）转换为 SolidWorks 格式。
        /// 这可能导致文件路径解析和某些操作出现问题。
        /// 本类在打开非 SolidWorks 原生格式文件时临时禁用 3D Interconnect，
        /// 打开完成后再恢复原设置。
        /// </para>
        /// </summary>
        private class Interconnect3DDisabler : IDisposable
        {
            private readonly ISldWorks m_App;
            private readonly bool? m_Is3DInterconnectEnabled;

            internal Interconnect3DDisabler(ISldWorks app)
            {
                m_App = app;

                // 仅在 SolidWorks 2020+ 版本检查 3D Interconnect 状态
                if (m_App.IsVersionNewerOrEqual(SwVersion_e.Sw2020))
                {
                    var enable3DInterconnect = m_App.GetUserPreferenceToggle(
                        (int)swUserPreferenceToggle_e.swMultiCAD_Enable3DInterconnect);

                    if (enable3DInterconnect)
                    {
                        m_Is3DInterconnectEnabled = enable3DInterconnect;

                        // 临时禁用 3D Interconnect
                        m_App.SetUserPreferenceToggle(
                            (int)swUserPreferenceToggle_e.swMultiCAD_Enable3DInterconnect, false);
                    }
                }
            }

            public void Dispose()
            {
                // 恢复 3D Interconnect 原始状态
                if (m_Is3DInterconnectEnabled.HasValue)
                {
                    m_App.SetUserPreferenceToggle(
                            (int)swUserPreferenceToggle_e.swMultiCAD_Enable3DInterconnect, m_Is3DInterconnectEnabled.Value);
                }
            }
        }

        /// <summary>
        /// SolidWorks 原生文件扩展名到文档类型的映射表。
        /// <para>
        /// 为什么需要这个映射：SolidWorks 支持多种文件扩展名，
        /// 包括 .sldprt（零件）、.sldasm（装配体）、.slddrw（工程图）等。
        /// 此映射用于根据文件扩展名快速确定文档类型。
        /// </para>
        /// <para>
        /// 使用 StringComparer.CurrentCultureIgnoreCase 实现大小写不敏感的匹配，
        /// 因为某些情况下手写扩展名可能大小写不一致。
        /// </para>
        /// </summary>
        protected static Dictionary<string, swDocumentTypes_e> m_NativeFileExts { get; }
        private bool? m_IsClosed;

        static SwDocument()
        {
            m_NativeFileExts = new Dictionary<string, swDocumentTypes_e>(StringComparer.CurrentCultureIgnoreCase)
            {
                // SolidWorks 原生格式
                { ".sldprt", swDocumentTypes_e.swDocPART },      // 零件文件
                { ".sldasm", swDocumentTypes_e.swDocASSEMBLY },  // 装配体文件
                { ".slddrw", swDocumentTypes_e.swDocDRAWING },   // 工程图文件
                // 轻量化格式
                { ".sldlfp", swDocumentTypes_e.swDocPART },      // 轻量级零件
                { ".sldblk", swDocumentTypes_e.swDocPART },      // 块文件
                // 模板格式（从模板创建新文档时使用）
                { ".prtdot", swDocumentTypes_e.swDocPART },      // 零件模板
                { ".asmdot", swDocumentTypes_e.swDocASSEMBLY },  // 装配体模板
                { ".drwdot", swDocumentTypes_e.swDocDRAWING }     // 工程图模板
            };
        }

        private DocumentEventDelegate m_DestroyedDel;
        private Action<SwDocument> m_HiddenDel;
        private DocumentCloseDelegate m_ClosingDel;

        public event DocumentEventDelegate Destroyed 
        {
            add 
            {
                m_DestroyedDel += value;
                AttachDestroyEventsIfNeeded();
            }
            remove 
            {
                m_DestroyedDel -= value;
                DetachDestroyEventsIfNeeded();
            }
        }

        internal event Action<SwDocument> Hidden
        {
            add
            {
                m_HiddenDel += value;
                AttachDestroyEventsIfNeeded();
            }
            remove
            {
                m_HiddenDel -= value;
                DetachDestroyEventsIfNeeded();
            }
        }

        public event DocumentCloseDelegate Closing
        {
            add
            {
                m_ClosingDel += value;
                AttachDestroyEventsIfNeeded();
            }
            remove
            {
                m_ClosingDel -= value;
                DetachDestroyEventsIfNeeded();
            }
        }

        public event DocumentEventDelegate Rebuilt 
        {
            add => m_DocumentRebuildEventHandler.Attach(value);
            remove => m_DocumentRebuildEventHandler.Detach(value);
        }

        public event DocumentSaveDelegate Saving
        {
            add => m_DocumentSavingEventHandler.Attach(value);
            remove => m_DocumentSavingEventHandler.Detach(value);
        }

        public event DataStoreAvailableDelegate StreamReadAvailable 
        {
            add => m_StreamReadAvailableHandler.Attach(value);
            remove => m_StreamReadAvailableHandler.Detach(value);
        }

        public event DataStoreAvailableDelegate StorageReadAvailable
        {
            add => m_StorageReadAvailableHandler.Attach(value);
            remove => m_StorageReadAvailableHandler.Detach(value);
        }

        public event DataStoreAvailableDelegate StreamWriteAvailable
        {
            add => m_StreamWriteAvailableHandler.Attach(value);
            remove => m_StreamWriteAvailableHandler.Detach(value);
        }

        public event DataStoreAvailableDelegate StorageWriteAvailable
        {
            add => m_StorageWriteAvailableHandler.Attach(value);
            remove => m_StorageWriteAvailableHandler.Detach(value);
        }

        IXFeatureRepository IXDocument.Features => Features;
        IXSelectionRepository IXDocument.Selections => Selections;
        IXDimensionRepository IDimensionable.Dimensions => Dimensions;
        IXPropertyRepository IPropertiesOwner.Properties => Properties;
        IXVersion IXDocument.Version => Version;
        IXModelViewRepository IXDocument.ModelViews => ModelViews;

        TObj IXDocument.DeserializeObject<TObj>(Stream stream)
            => DeserializeBaseObject<TObj>(stream);

        protected readonly IXLogger m_Logger;

        private readonly StreamReadAvailableEventsHandler m_StreamReadAvailableHandler;
        private readonly StorageReadAvailableEventsHandler m_StorageReadAvailableHandler;
        private readonly StreamWriteAvailableEventsHandler m_StreamWriteAvailableHandler;
        private readonly StorageWriteAvailableEventsHandler m_StorageWriteAvailableHandler;
        private readonly DocumentRebuildEventsHandler m_DocumentRebuildEventHandler;
        private readonly DocumentSavingEventHandler m_DocumentSavingEventHandler;
        
        /// <summary>
        /// 获取底层 SolidWorks 文档 COM 对象。
        /// <para>中文：通过 ElementCreator 模式获取已创建/打开的文档对象。</para>
        /// </summary>
        public IModelDoc2 Model => m_Creator.Element;

        /// <summary>
        /// 文档的完整文件路径。
        /// <para>
        /// 返回值说明：
        /// <list type="bullet">
        /// <item><description>已提交的文档：尝试调用 Model.GetPathName()，如果 COM 对象已失效则返回缓存路径</description></item>
        /// <item><description>未提交的文档：返回创建时缓存的属性值</description></item>
        /// </list>
        /// </para>
        /// <para>
        /// 重要：已提交的文档的路径只能在创建时设置，不支持后续修改。
        /// 这是因为 SolidWorks 不允许在文档打开后更改其文件路径。
        /// </para>
        /// </summary>
        public string Path
        {
            get
            {
                if (IsCommitted)
                {
                    try
                    {
                        return Model.GetPathName();
                    }
                    catch
                    {
                        // COM 对象可能已失效（如文件被外部删除），返回缓存路径
                        return m_CachedFilePath;
                    }
                }
                else
                {
                    return m_Creator.CachedProperties.Get<string>();
                }
            }
            set
            {
                if (IsCommitted)
                {
                    // 已提交的文档不能更改路径，这是 SolidWorks 的限制
                    throw new NotSupportedException("Path can only be changed for the not commited document");
                }
                else
                {
                    m_Creator.CachedProperties.Set(value);
                }
            }
        }

        public string Template
        {
            get
            {
                if (IsCommitted)
                {
                    throw new NotSupportedException("Template cannot be retrieved for the created document");
                }
                else
                {
                    return m_Creator.CachedProperties.Get<string>();
                }
            }
            set
            {
                if (IsCommitted)
                {
                    throw new NotSupportedException("Template cannot be changed for the committed document");
                }
                else
                {
                    m_Creator.CachedProperties.Set(value);
                }
            }
        }

        public string Title
        {
            get
            {
                if (IsCommitted)
                {
                    return Model.GetTitle();
                }
                else 
                {
                    var userTitle = m_Creator.CachedProperties.Get<string>();

                    if (!string.IsNullOrEmpty(userTitle))
                    {
                        return userTitle;
                    }
                    else 
                    {
                        var path = Path;

                        if (!string.IsNullOrEmpty(path))
                        {
                            return System.IO.Path.GetFileName(path);
                        }
                        else 
                        {
                            return "";
                        }
                    }
                }
            }
            set 
            {
                if (IsCommitted)
                {
                    if (string.IsNullOrEmpty(Path))
                    {
                        Model.SetTitle2(value);
                    }
                    else 
                    {
                        throw new NotSupportedException("Title can only be changed for new document");
                    }
                }
                else 
                {
                    m_Creator.CachedProperties.Set(value);
                }
            }
        }
        
        public DocumentState_e State 
        {
            get
            {
                if (IsCommitted)
                {
                    return GetDocumentState();
                }
                else
                {
                    return m_Creator.CachedProperties.Get<DocumentState_e>();
                }
            }
            set
            {
                if (IsCommitted)
                {
                    var curState = GetDocumentState();

                    if (curState == value)
                    {
                        //do nothing
                    }
                    else if (((int)curState - (int)value) == (int)DocumentState_e.Hidden)
                    {
                        Model.Visible = true;
                    }
                    else if ((int)value - ((int)curState) == (int)DocumentState_e.Hidden)
                    {
                        Model.Visible = false;
                    }
                    else
                    {
                        throw new Exception("Only visibility can changed after the document is loaded");
                    }
                }
                else
                {
                    m_Creator.CachedProperties.Set(value);
                }
            }
        }

        private DocumentState_e GetDocumentState()
        {
            var state = DocumentState_e.Default;

            if (IsRapidMode)
            {
                state |= DocumentState_e.Rapid;
            }

            if (IsLightweightMode)
            {
                state |= DocumentState_e.Lightweight;
            }

            if (Model.IsOpenedReadOnly())
            {
                state |= DocumentState_e.ReadOnly;
            }

            if (Model.IsOpenedViewOnly())
            {
                state |= DocumentState_e.ViewOnly;
            }

            if (!Model.Visible)
            {
                state |= DocumentState_e.Hidden;
            }

            return state;
        }

        protected abstract bool IsRapidMode { get; }
        protected abstract bool IsLightweightMode { get; }

        private readonly Lazy<SwFeatureManager> m_FeaturesLazy;
        private readonly Lazy<ISwSelectionCollection> m_SelectionsLazy;
        private readonly Lazy<ISwDimensionsCollection> m_DimensionsLazy;
        private readonly Lazy<ISwCustomPropertiesCollection> m_PropertiesLazy;
        private readonly Lazy<SwAnnotationCollection> m_AnnotationsLazy;

        public IXDocumentDependencies Dependencies { get; }
        public ISwFeatureManager Features => m_FeaturesLazy.Value;
        public ISwSelectionCollection Selections => m_SelectionsLazy.Value;
        public ISwDimensionsCollection Dimensions => m_DimensionsLazy.Value;
        public ISwCustomPropertiesCollection Properties => m_PropertiesLazy.Value;
                
        public bool IsDirty 
        {
            get => Model.GetSaveFlag();
            set
            {
                if (value == true)
                {
                    Model.SetSaveFlag();

                    if (!Model.GetSaveFlag()) 
                    {
                        throw new DirtyFlagIsNotSetException();
                    }
                }
                else 
                {
                    throw new NotSupportedException("Dirty flag cannot be removed. Save document to remove dirty flag");
                }
            }
        }
        
        public bool IsCommitted => m_Creator.IsCreated;

        protected readonly IElementCreator<IModelDoc2> m_Creator;

        private bool m_AreEventsAttached;
        private bool m_AreDestroyEventsAttached;

        internal override SwDocument OwnerDocument => this;

        private bool m_IsDisposed;

        private readonly Lazy<ISwModelViewsCollection> m_ModelViewsLazy;

        /// <summary>
        /// 缓存的文件路径。
        /// <para>
        /// 为什么需要缓存：某些情况下 COM 指针可能失效（如文件被外部程序关闭），
        /// 此时无法通过 Model.GetPathName() 获取路径。
        /// 缓存路径用于在这些情况下仍能识别文档。
        /// </para>
        /// </summary>
        private string m_CachedFilePath;

        internal SwDocument(IModelDoc2 model, SwApplication app, IXLogger logger) 
            : this(model, app, logger, true)
        {
        }

        internal SwDocument(IModelDoc2 model, SwApplication app, IXLogger logger, bool created) : base(model, null, app)
        {
            m_Logger = logger;

            m_Creator = new ElementCreator<IModelDoc2>(CreateDocument, CommitCache, model, created);

            m_FeaturesLazy = new Lazy<SwFeatureManager>(() => new SwDocumentFeatureManager(this, app, new Context(this)));
            m_SelectionsLazy = new Lazy<ISwSelectionCollection>(() => new SwSelectionCollection(this, app));
            m_DimensionsLazy = new Lazy<ISwDimensionsCollection>(() => new SwFeatureManagerDimensionsCollection(this.Features, new Context(this)));
            m_PropertiesLazy = new Lazy<ISwCustomPropertiesCollection>(() => new SwFileCustomPropertiesCollection(this, app));

            m_AnnotationsLazy = new Lazy<SwAnnotationCollection>(CreateAnnotations);

            m_ModelViewsLazy = new Lazy<ISwModelViewsCollection>(() => new SwModelViewsCollection(this, app));

            Units = new SwUnits(this);

            Options = new SwDocumentOptions(this);

            Dependencies = new SwDocumentDependencies(this, m_Logger);

            m_StreamReadAvailableHandler = new StreamReadAvailableEventsHandler(this, app);
            m_StreamWriteAvailableHandler = new StreamWriteAvailableEventsHandler(this, app);
            m_StorageReadAvailableHandler = new StorageReadAvailableEventsHandler(this, app);
            m_StorageWriteAvailableHandler = new StorageWriteAvailableEventsHandler(this, app);
            m_DocumentRebuildEventHandler = new DocumentRebuildEventsHandler(this, app);
            m_DocumentSavingEventHandler = new DocumentSavingEventHandler(this, app);

            m_AreEventsAttached = false;

            if (IsCommitted)
            {
                m_CachedFilePath = model.GetPathName();
                AttachEvents();
            }

            m_IsDisposed = false;
        }

        public override object Dispatch => Model;

        internal void SetModel(IModelDoc2 model) => m_Creator.Set(model);

        private IModelDoc2 CreateDocument(CancellationToken cancellationToken)
        {
            //if (((SwDocumentCollection)OwnerApplication.Documents).TryFindExistingDocumentByPath(Path, out _))
            //{
            //    throw new DocumentAlreadyOpenedException(Path);
            //}

            var docType = -1;

            if (DocumentType.HasValue)
            {
                docType = (int)DocumentType.Value;
            }

            var origVisible = true;

            if (docType != -1)
            {
                origVisible = OwnerApplication.Sw.GetDocumentVisible(docType);
            }

            IModelDoc2 model = null;

            try
            {
                if (docType != -1)
                {
                    var visible = !State.HasFlag(DocumentState_e.Hidden);

                    OwnerApplication.Sw.DocumentVisible(visible, docType);
                }

                if (string.IsNullOrEmpty(Path))
                {
                    model = CreateNewDocument();
                }
                else
                {
                    m_CachedFilePath = Path;
                    model = OpenDocument();
                }

                return model;
            }
            finally 
            {
                if (model != null)
                {
                    this.Bind(model);
                }
                
                if (docType != -1)
                {
                    OwnerApplication.Sw.DocumentVisible(origVisible, docType);
                }
            }
        }

        protected abstract SwAnnotationCollection CreateAnnotations();

        protected virtual void CommitCache(IModelDoc2 model, CancellationToken cancellationToken)
        {
            if (m_FeaturesLazy.IsValueCreated) 
            {
                m_FeaturesLazy.Value.CommitCache(cancellationToken);
            }
        }

        internal protected abstract swDocumentTypes_e? DocumentType { get; }

        public ISwVersion Version 
        {
            get 
            {
                string[] versHistory;

                if (!string.IsNullOrEmpty(Path))
                {
                    versHistory = OwnerApplication.Sw.VersionHistory(Path) as string[];
                }
                else
                {
                    if (IsCommitted)
                    {
                        versHistory = Model.VersionHistory() as string[];
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(Path))
                        {
                            versHistory = OwnerApplication.Sw.VersionHistory(Path) as string[];
                        }
                        else
                        {
                            throw new Exception("Path is not specified");
                        }
                    }
                }

                var vers = GetVersion(versHistory);

                return SwApplicationFactory.CreateVersion(vers);
            }
        }

        public override bool Equals(IXObject other)
        {
            if (object.ReferenceEquals(this, other))
            {
                return true;
            }
            
            if(other == null)
            {
                return false;
            }

            if (other is ISwDocument)
            {
                if (IsCommitted && ((ISwDocument)other).IsCommitted)
                {
                    var model1 = Model;
                    var model2 = ((ISwDocument)other).Model;

                    if (object.ReferenceEquals(model1, model2))
                    {
                        return true;
                    }

                    bool isAlive1;
                    bool isAlive2;

                    string title1 = "";
                    string title2 = "";

                    try
                    {
                        title1 = model1.GetTitle();
                        isAlive1 = true;
                    }
                    catch
                    {
                        isAlive1 = false;
                    }

                    try
                    {
                        title2 = model2.GetTitle();
                        isAlive2 = true;
                    }
                    catch
                    {
                        isAlive2 = false;
                    }

                    if (isAlive1 && isAlive2)
                    {
                        //NOTE: in some cases drawings can have the same title so it might not be safe to only compare by titles
                        if (string.Equals(title1, title2, StringComparison.CurrentCultureIgnoreCase))
                        {
                            return OwnerApplication.Sw.IsSame(model1, model2) == (int)swObjectEquality.swObjectSame;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else if (!isAlive1 && !isAlive2)
                    {
                        if (!string.IsNullOrEmpty(m_CachedFilePath))
                        {
                            return string.Equals(m_CachedFilePath, ((SwDocument)other).m_CachedFilePath, StringComparison.CurrentCultureIgnoreCase);
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else 
                    {
                        return false;
                    }
                }
                else if (!IsCommitted && !((ISwDocument)other).IsCommitted)
                {
                    if (!string.IsNullOrEmpty(Path))
                    {
                        return string.Equals(Path, ((ISwDocument)other).Path, StringComparison.CurrentCultureIgnoreCase);
                    }
                    else 
                    {
                        return false;
                    }
                }
                else 
                {
                    return false;
                }
            }
            else 
            {
                return false;
            }
        }

        public override bool IsAlive 
        {
            get 
            {
                var model = Model;

                try
                {
                    var title = model.GetTitle();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        public int UpdateStamp => Model.GetUpdateStamp();

        public IXUnits Units { get; }
        public virtual IXDocumentOptions Options { get; }

        public virtual ISwModelViewsCollection ModelViews => m_ModelViewsLazy.Value;

        public IXAnnotationRepository Annotations => m_AnnotationsLazy.Value;

        private SwVersion_e GetVersion(string[] versHistory)
        {
            if (versHistory?.Any() == true)
            {
                var latestVers = versHistory.Last();

                var majorRev = int.Parse(latestVers.Substring(0, latestVers.IndexOf('[')));

                switch (majorRev)
                {
                    case 44:
                    case 243:
                    case 483:
                    case 629:
                    case 822:
                    case 1008:
                    case 1137:
                        return SwVersion_e.SwPrior2000;
                    case 1500:
                        return SwVersion_e.Sw2000;
                    case 1750:
                        return SwVersion_e.Sw2001;
                    case 1950:
                        return SwVersion_e.Sw2001Plus;
                    case 2200:
                        return SwVersion_e.Sw2003;
                    case 2500:
                        return SwVersion_e.Sw2004;
                    case 2800:
                        return SwVersion_e.Sw2005;
                    case 3100:
                        return SwVersion_e.Sw2006;
                    case 3400:
                        return SwVersion_e.Sw2007;
                    case 3800:
                        return SwVersion_e.Sw2008;
                    case 4100:
                        return SwVersion_e.Sw2009;
                    case 4400:
                        return SwVersion_e.Sw2010;
                    case 4700:
                        return SwVersion_e.Sw2011;
                    case 5000:
                        return SwVersion_e.Sw2012;
                    case 6000:
                        return SwVersion_e.Sw2013;
                    case 7000:
                        return SwVersion_e.Sw2014;
                    case 8000:
                        return SwVersion_e.Sw2015;
                    case 9000:
                        return SwVersion_e.Sw2016;
                    case 10000:
                        return SwVersion_e.Sw2017;
                    case 11000:
                        return SwVersion_e.Sw2018;
                    case 12000:
                        return SwVersion_e.Sw2019;
                    case 13000:
                        return SwVersion_e.Sw2020;
                    case 14000:
                        return SwVersion_e.Sw2021;
                    case 15000:
                        return SwVersion_e.Sw2022;
                    case 16000:
                        return SwVersion_e.Sw2023;
                    case 17000:
                        return SwVersion_e.Sw2024;
                    case 18000:
                        return SwVersion_e.Sw2025;
                    default:
                        throw new NotSupportedException($"'{latestVers}' version is not recognized");
                }
            }
            else
            {
                throw new NullReferenceException($"Version information is not found");
            }
        }

        private IModelDoc2 CreateNewDocument() 
        {
            var docTemplate = Template;

            GetPaperSize(out var paperSize, out var paperWidth, out var paperHeight);

            if (string.IsNullOrEmpty(docTemplate))
            {
                if (!DocumentType.HasValue) 
                {
                    throw new Exception("Cannot find the default template for unknown document type");
                }

                var useDefTemplates = OwnerApplication.Sw.GetUserPreferenceToggle((int)swUserPreferenceToggle_e.swAlwaysUseDefaultTemplates);

                try
                {
                    OwnerApplication.Sw.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swAlwaysUseDefaultTemplates, true);

                    docTemplate = OwnerApplication.Sw.GetDocumentTemplate(
                        (int)DocumentType.Value, "", (int)paperSize, paperWidth, paperHeight);
                }
                finally
                {
                    OwnerApplication.Sw.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swAlwaysUseDefaultTemplates, useDefTemplates);
                }
            }

            if (!string.IsNullOrEmpty(docTemplate))
            {
                var doc = OwnerApplication.Sw.NewDocument(docTemplate, (int)paperSize, paperWidth, paperHeight) as IModelDoc2;

                if (doc != null)
                {
                    if (!string.IsNullOrEmpty(Title))
                    {
                        //TODO: need to communicate exception if title is not set, do not throw it from here as the doc won't be registered
                        doc.SetTitle2(Title);
                    }

                    return doc;
                }
                else 
                {
                    throw new NewDocumentCreateException(docTemplate);
                }
            }
            else 
            {
                throw new DefaultTemplateNotFoundException();
            }
        }
        
        /// <summary>
        /// 打开现有文档。
        /// <para>
        /// 打开逻辑分为两个分支：
        /// <list type="bullet">
        /// <item><description>原生格式（.sldprt/.sldasm/.slddrw）：使用 OpenDoc6 API，支持多种打开选项</description></item>
        /// <item><description>外部格式（STEP/IGES等）：使用 LoadFile4 API，自动禁用 3D Interconnect</description></item>
        /// </list>
        /// </para>
        /// <para>
        /// 打开选项处理：
        /// <list type="bullet">
        /// <item><description>ReadOnly：只读模式打开</description></item>
        /// <item><description>ViewOnly：仅查看模式（不能编辑）</description></item>
        /// <item><description>Silent：静默模式（不显示UI）</description></item>
        /// <item><description>Rapid：快速模式（工程图打开到详图模式，装配体打开到轻量级）</description></item>
        /// <item><description>Lightweight：强制加载轻量级组件</description></item>
        /// </list>
        /// </para>
        /// </summary>
        private IModelDoc2 OpenDocument()
        {
            IModelDoc2 model;
            int errorCode = -1;

            // 根据文件扩展名判断是否为 SolidWorks 原生格式
            if (m_NativeFileExts.TryGetValue(System.IO.Path.GetExtension(Path), out swDocumentTypes_e docType))
            {
                swOpenDocOptions_e opts = 0;

                // 处理各种文档状态标志，构建打开选项
                if (State.HasFlag(DocumentState_e.ReadOnly))
                {
                    opts |= swOpenDocOptions_e.swOpenDocOptions_ReadOnly;
                }

                if (State.HasFlag(DocumentState_e.ViewOnly))
                {
                    opts |= swOpenDocOptions_e.swOpenDocOptions_ViewOnly;
                }

                if (State.HasFlag(DocumentState_e.Silent))
                {
                    opts |= swOpenDocOptions_e.swOpenDocOptions_Silent;
                }

                // Rapid 模式：工程图打开到详图模式（2020+），装配体打开到视图模式
                if (State.HasFlag(DocumentState_e.Rapid))
                {
                    if (docType == swDocumentTypes_e.swDocDRAWING)
                    {
                        if (OwnerApplication.IsVersionNewerOrEqual(SwVersion_e.Sw2020))
                        {
                            opts |= swOpenDocOptions_e.swOpenDocOptions_OpenDetailingMode;
                        }
                    }
                    else if (docType == swDocumentTypes_e.swDocASSEMBLY)
                    {
                        opts |= swOpenDocOptions_e.swOpenDocOptions_ViewOnly;

                        // 2021 SP4.1+ 支持 LDR 编辑模式
                        if (OwnerApplication.IsVersionNewerOrEqual(SwVersion_e.Sw2021, 4, 1))
                        {
                            opts |= swOpenDocOptions_e.swOpenDocOptions_LDR_EditAssembly;
                        }
                    }
                    else if (docType == swDocumentTypes_e.swDocPART)
                    {
                        // 零件文档不支持 Rapid 选项
                    }
                }

                // Lightweight 模式：仅对工程图和装配体有效
                if (State.HasFlag(DocumentState_e.Lightweight))
                {
                    if (docType == swDocumentTypes_e.swDocDRAWING
                        || docType == swDocumentTypes_e.swDocASSEMBLY)
                    {
                        opts |= swOpenDocOptions_e.swOpenDocOptions_OverrideDefaultLoadLightweight | swOpenDocOptions_e.swOpenDocOptions_LoadLightweight;
                    }
                    else if (docType == swDocumentTypes_e.swDocPART)
                    {
                        // 零件文档不支持 Lightweight 选项
                    }
                }
                else
                {
                    // 非轻量级模式：工程图和装配体显式覆盖默认加载行为
                    if (docType == swDocumentTypes_e.swDocDRAWING || docType == swDocumentTypes_e.swDocASSEMBLY)
                    {
                        opts |= swOpenDocOptions_e.swOpenDocOptions_OverrideDefaultLoadLightweight;
                    }
                    else if (docType == swDocumentTypes_e.swDocPART)
                    {
                        // 零件文档不支持 Lightweight 选项
                    }
                }

                // 验证文件类型兼容性
                if (!IsDocumentTypeCompatible(docType))
                {
                    throw new DocumentPathIncompatibleException(this);
                }

                int warns = -1;
                // 使用 OpenDoc6 打开原生格式，支持更详细的选项和错误返回
                model = OwnerApplication.Sw.OpenDoc6(Path, (int)docType, (int)opts, "", ref errorCode, ref warns);
            }
            else
            {
                // 非原生格式（外部 CAD 格式如 STEP、IGES 等）
                // 使用 LoadFile4 API 加载，此时需要禁用 3D Interconnect
                using (new Interconnect3DDisabler(OwnerApplication.Sw))
                {
                    model = OwnerApplication.Sw.LoadFile4(Path, "", null, ref errorCode);
                }

                if (model != null)
                {
                    // 验证加载后的文档类型与预期兼容
                    if (!IsDocumentTypeCompatible((swDocumentTypes_e)model.GetType()))
                    {
                        throw new DocumentPathIncompatibleException(this);
                    }
                }
            }

            // 处理打开失败的情况，将错误代码转换为友好的错误消息
            if (model == null)
            {
                string error = "";

                switch ((swFileLoadError_e)errorCode)
                {
                    case swFileLoadError_e.swAddinInteruptError:
                        error = "文件打开被用户中断";
                        break;
                    case swFileLoadError_e.swApplicationBusy:
                        error = "应用程序忙碌，请稍后重试";
                        break;
                        error = "Application is busy";
                        break;
                    case swFileLoadError_e.swFileCriticalDataRepairError:
                        error = "File has critical data corruption";
                        break;
                    case swFileLoadError_e.swFileNotFoundError:
                        error = "File not found at the specified path";
                        break;
                    case swFileLoadError_e.swFileRequiresRepairError:
                        error = "File has non-critical data corruption and requires repair";
                        break;
                    case swFileLoadError_e.swFileWithSameTitleAlreadyOpen:
                        error = "A document with the same name is already open";
                        break;
                    case swFileLoadError_e.swFutureVersion:
                        error = "The document was saved in a future version of SOLIDWORKS";
                        break;
                    case swFileLoadError_e.swGenericError:
                        error = "Unknown error while opening file";
                        break;
                    case swFileLoadError_e.swInvalidFileTypeError:
                        error = "Invalid file type";
                        break;
                    case swFileLoadError_e.swLiquidMachineDoc:
                        error = "File encrypted by Liquid Machines";
                        break;
                    case swFileLoadError_e.swLowResourcesError:
                        error = "File is open and blocked because the system memory is low, or the number of GDI handles has exceeded the allowed maximum";
                        break;
                    case swFileLoadError_e.swNoDisplayData:
                        error = "File contains no display data";
                        break;
                }

                throw new OpenDocumentFailedException(Path, errorCode, error);
            }

            return model;
        }

        protected abstract bool IsDocumentTypeCompatible(swDocumentTypes_e docType);

        protected virtual void GetPaperSize(out swDwgPaperSizes_e size, out double width, out double height) 
        {
            size = swDwgPaperSizes_e.swDwgPapersUserDefined;
            width = -1;
            height = -1;
        }

        //NOTE: closing of document migth note neecsserily unload if from memory (if this document is used in active assembly or drawing)
        //do not dispose or set m_IsClosed flag in this function
        public void Close()
            => OwnerApplication.Sw.CloseDoc(Model.GetTitle());
        
        public void Dispose()
        {
            if (!m_IsDisposed)
            {
                m_IsDisposed = true;

                if (m_IsClosed != true)
                {
                    if (IsAlive)
                    {
                        Close();
                    }
                }

                Dispose(true);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (m_SelectionsLazy.IsValueCreated)
            {
                m_SelectionsLazy.Value.Dispose();
            }

            if (m_DimensionsLazy.IsValueCreated)
            {
                m_DimensionsLazy.Value.Dispose();
            }

            if (m_PropertiesLazy.IsValueCreated)
            {
                m_PropertiesLazy.Value.Dispose();
            }

            if (disposing)
            {
                m_StreamReadAvailableHandler.Dispose();
                m_StreamWriteAvailableHandler.Dispose();
                m_StorageReadAvailableHandler.Dispose();
                m_StorageWriteAvailableHandler.Dispose();

                DetachEvents();
            }
        }

        internal void AttachEvents()
        {
            if (!m_AreEventsAttached)
            {
                m_AreEventsAttached = true;

                AttachDestroyEventsIfNeeded();

                switch (Model)
                {
                    case PartDoc part:
                        part.FileSavePostNotify += OnFileSavePostNotify;
                        break;

                    case AssemblyDoc assm:
                        assm.FileSavePostNotify += OnFileSavePostNotify;
                        break;

                    case DrawingDoc drw:
                        drw.FileSavePostNotify += OnFileSavePostNotify;
                        break;
                }
            }
            else 
            {
                Debug.Assert(false, "Events already attached");
            }
        }

        private void AttachDestroyEventsIfNeeded()
        {
            if (!m_AreDestroyEventsAttached && (m_DestroyedDel != null || m_HiddenDel != null || m_ClosingDel != null))
            {
                switch (Model)
                {
                    case PartDoc part:
                        part.DestroyNotify2 += OnDestroyNotify;
                        break;

                    case AssemblyDoc assm:
                        assm.DestroyNotify2 += OnDestroyNotify;
                        break;

                    case DrawingDoc drw:
                        drw.DestroyNotify2 += OnDestroyNotify;
                        break;
                }

                m_AreDestroyEventsAttached = true;
            }
        }

        private void DetachDestroyEventsIfNeeded()
        {
            if (m_AreDestroyEventsAttached && (m_DestroyedDel == null && m_HiddenDel == null && m_ClosingDel == null))
            {
                switch (Model)
                {
                    case PartDoc part:
                        part.DestroyNotify2 -= OnDestroyNotify;
                        break;

                    case AssemblyDoc assm:
                        assm.DestroyNotify2 -= OnDestroyNotify;
                        break;

                    case DrawingDoc drw:
                        drw.DestroyNotify2 -= OnDestroyNotify;
                        break;
                }

                m_AreDestroyEventsAttached = false;
            }
        }

        private int OnFileSavePostNotify(int saveType, string fileName)
        {
            if (saveType == (int)swFileSaveTypes_e.swFileSaveAs)
            {
                m_CachedFilePath = fileName;
            }
            
            return HResult.S_OK;
        }

        private void DetachEvents()
        {
            switch (Model)
            {
                case PartDoc part:
                    part.DestroyNotify2 -= OnDestroyNotify;
                    part.FileSavePostNotify -= OnFileSavePostNotify;
                    break;

                case AssemblyDoc assm:
                    assm.DestroyNotify2 -= OnDestroyNotify;
                    assm.FileSavePostNotify -= OnFileSavePostNotify;
                    break;

                case DrawingDoc drw:
                    drw.DestroyNotify2 -= OnDestroyNotify;
                    drw.FileSavePostNotify -= OnFileSavePostNotify;
                    break;
            }
        }

        private int OnDestroyNotify(int destroyType)
        {
            try
            {
                if (destroyType == (int)swDestroyNotifyType_e.swDestroyNotifyDestroy)
                {
                    m_Logger.Log($"Destroying '{Model.GetTitle()}' document", XCad.Base.Enums.LoggerMessageSeverity_e.Debug);

                    try
                    {
                        m_ClosingDel?.Invoke(this, DocumentCloseType_e.Destroy);
                    }
                    catch (Exception ex)
                    {
                        m_Logger.Log(ex);
                    }

                    m_DestroyedDel?.Invoke(this);

                    m_IsClosed = true;

                    Dispose();
                }
                else if (destroyType == (int)swDestroyNotifyType_e.swDestroyNotifyHidden)
                {
                    try
                    {
                        m_ClosingDel?.Invoke(this, DocumentCloseType_e.Hide);
                    }
                    catch (Exception ex)
                    {
                        m_Logger.Log(ex);
                    }

                    m_HiddenDel?.Invoke(this);

                    m_Logger.Log($"Hiding '{Model.GetTitle()}' document", XCad.Base.Enums.LoggerMessageSeverity_e.Debug);
                }
                else
                {
                    Debug.Assert(false, "Not supported type of destroy");
                }
            }
            catch (Exception ex)
            {
                m_Logger.Log(ex);
            }
            
            return HResult.S_OK;
        }

        public Stream OpenStream(string name, AccessType_e access)
            => new Sw3rdPartyStream(Model, name, access);

        public IStorage OpenStorage(string name, AccessType_e access)
            => new Sw3rdPartyStorage(Model, name, access);

        public virtual void Commit(CancellationToken cancellationToken)
            => m_Creator.Create(cancellationToken);

        public void Save()
        {
            if (!string.IsNullOrEmpty(Path))
            {
                int errs = -1;
                int warns = -1;

                if (!Model.Save3((int)swSaveAsOptions_e.swSaveAsOptions_Silent, ref errs, ref warns))
                {
                    throw new SaveDocumentFailedException(errs, SwSaveOperation.ParseSaveError((swFileSaveError_e)errs));
                }
            }
            else 
            {
                throw new SaveNeverSavedDocumentException();
            }
        }

        public TSwObj DeserializeObject<TSwObj>(Stream stream)
            where TSwObj : ISwObject
            => DeserializeBaseObject<TSwObj>(stream);

        private TObj DeserializeBaseObject<TObj>(Stream stream)
            where TObj : IXObject
        {
            stream.Seek(0, SeekOrigin.Begin);

            byte[] buffer;

            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                buffer = memoryStream.ToArray();
            }

            var obj = Model.Extension.GetObjectByPersistReference3(buffer, out int err);

            if (obj != null)
            {
                return (TObj)CreateObjectFromDispatch<ISwObject>(obj);
            }
            else
            {
                string reason;

                switch ((swPersistReferencedObjectStates_e)err)
                {
                    case swPersistReferencedObjectStates_e.swPersistReferencedObject_Deleted:
                        reason = "Object is deleted";
                        break;

                    case swPersistReferencedObjectStates_e.swPersistReferencedObject_Invalid:
                        reason = "Object is invalid";
                        break;

                    case swPersistReferencedObjectStates_e.swPersistReferencedObject_Suppressed:
                        reason = "Object is suppressed";
                        break;

                    default:
                        reason = "Unknown reason";
                        break;
                }

                throw new ObjectSerializationException($"Failed to serialize object: {reason}", err);
            }
        }

        public TObj CreateObjectFromDispatch<TObj>(object disp) where TObj : ISwObject
            => SwObjectFactory.FromDispatch<TObj>(disp, this, OwnerApplication);

        public void Rebuild() 
        {
            if (Model.ForceRebuild3(false)) 
            {
                //do not throw exception - in some cases rebuild is happening, but false is returned
                //throw new Exception("Failed to rebuild the model");
            }
        }

        public IOperationGroup PreCreateOperationGroup() => new SwUndoObjectGroup(this);

        public abstract IXSaveOperation PreCreateSaveAsOperation(string filePath);
    }

    internal class SwUnknownDocument : SwDocument, IXUnknownDocument
    {
        public SwUnknownDocument(IModelDoc2 model, SwApplication app, IXLogger logger, bool isCreated) 
            : base(model, app, logger, isCreated)
        {
        }

        protected override bool IsLightweightMode => throw new NotSupportedException();
        protected override bool IsRapidMode => throw new NotSupportedException();
        protected override SwAnnotationCollection CreateAnnotations() => throw new NotSupportedException();

        internal protected override swDocumentTypes_e? DocumentType 
        {
            get 
            {
                if (IsCommitted)
                {
                    return (swDocumentTypes_e)Model.GetType();
                }
                else 
                {
                    if (!string.IsNullOrEmpty(Path))
                    {
                        if (m_NativeFileExts.TryGetValue(
                            System.IO.Path.GetExtension(Path), out swDocumentTypes_e type))
                        {
                            return type;
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else if (!string.IsNullOrEmpty(Template))
                    {
                        if (m_NativeFileExts.TryGetValue(
                            System.IO.Path.GetExtension(Template), out swDocumentTypes_e type))
                        {
                            return type;
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else 
                    {
                        return null;
                    }
                }
            }
        }

        public override void Commit(CancellationToken cancellationToken)
        {
            if (((SwDocumentCollection)OwnerApplication.Documents).TryFindExistingDocumentByPath(Path, out SwDocument curDoc))
            {
                m_SpecificDoc = curDoc;
                m_Creator.Init(curDoc.Model, CancellationToken.None);
            }
            else
            {
                base.Commit(cancellationToken);
            }
        }

        private IXDocument m_SpecificDoc;

        public IXDocument GetSpecific()
        {
            if (m_SpecificDoc != null)
            {
                return m_SpecificDoc;
            }

            var model = Model;

            if (model == null) 
            {
                throw new Exception("Model is not yet created, cannot get specific document");
            }

            switch (DocumentType)
            {
                case swDocumentTypes_e.swDocPART:
                    m_SpecificDoc = new SwPart(model as IPartDoc, OwnerApplication, m_Logger, true);
                    break;

                case swDocumentTypes_e.swDocASSEMBLY:
                    m_SpecificDoc = new SwAssembly(model as IAssemblyDoc, OwnerApplication, m_Logger, true);
                    break;

                case swDocumentTypes_e.swDocDRAWING:
                    m_SpecificDoc = new SwDrawing(model as IDrawingDoc, OwnerApplication, m_Logger, true);
                    break;

                default:
                    throw new Exception("Invalid document type");
            }

            return m_SpecificDoc;
        }

        protected override bool IsDocumentTypeCompatible(swDocumentTypes_e docType) => true;

        public override IXSaveOperation PreCreateSaveAsOperation(string filePath) => throw new NotSupportedException();
    }

    internal class SwUnknownDocument3D : SwUnknownDocument, ISwDocument3D
    {
        public SwUnknownDocument3D(IModelDoc2 model, SwApplication app, IXLogger logger, bool isCreated) 
            : base(model, app, logger, isCreated)
        {
        }

        public IXConfigurationRepository Configurations => throw new NotImplementedException();
        public IXDocumentEvaluation Evaluation => throw new NotImplementedException();
        public IXDocumentGraphics Graphics => throw new NotImplementedException();
        ISwConfigurationCollection ISwDocument3D.Configurations => throw new NotImplementedException();
        IXConfigurationRepository IXDocument3D.Configurations => throw new NotImplementedException();
        ISwModelViews3DCollection ISwDocument3D.ModelViews => throw new NotImplementedException();
        IXModelView3DRepository IXDocument3D.ModelViews => throw new NotImplementedException();
        TSelObject IXObjectContainer.ConvertObject<TSelObject>(TSelObject obj) => throw new NotImplementedException();
        TSelObject ISwDocument3D.ConvertObject<TSelObject>(TSelObject obj) => throw new NotImplementedException();
        IXDocument3DSaveOperation IXDocument3D.PreCreateSaveAsOperation(string filePath) => throw new NotImplementedException();
    }

    internal static class SwDocumentExtension 
    {
        internal static Image GetThumbnailImage(this SwDocument doc) 
        {
            using (var thumbnail = new ShellThumbnail(doc.Path)) 
            {
                return Image.FromHbitmap(thumbnail.BitmapHandle);
            }
        }

        internal static void SetUserPreferenceToggle(this SwDocument doc, swUserPreferenceToggle_e option, bool value) 
            => doc.Model.Extension.SetUserPreferenceToggle((int)option, (int)swUserPreferenceOption_e.swDetailingNoOptionSpecified, value);

        internal static bool GetUserPreferenceToggle(this SwDocument doc, swUserPreferenceToggle_e option)
            => doc.Model.Extension.GetUserPreferenceToggle((int)option, (int)swUserPreferenceOption_e.swDetailingNoOptionSpecified);

        internal static void Bind(this SwDocument doc, IModelDoc2 model)
        {
            if (!doc.IsCommitted)
            {
                doc.SetModel(model);
            }

            if (doc.IsCommitted)
            {
                if (!(doc is SwUnknownDocument))
                {
                    doc.AttachEvents();
                }
            }
        }
    }
}