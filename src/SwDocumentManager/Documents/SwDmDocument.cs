//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using SolidWorks.Interop.swdocumentmgr;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
using Xarial.XCad.Features;
using Xarial.XCad.Geometry;
using Xarial.XCad.Services;
using Xarial.XCad.SolidWorks.Data;
using Xarial.XCad.SwDocumentManager.Data;
using Xarial.XCad.Toolkit.Data;
using Xarial.XCad.UI;

namespace Xarial.XCad.SwDocumentManager.Documents
{
    /// <summary>
    /// Base document contract for all Document Manager-backed xCAD documents.
    /// 所有基于 Document Manager 的 xCAD 文档共用的基础约定。
    /// </summary>
    public interface ISwDmDocument : ISwDmObject, IXDocument
    {
        ISwDMDocument Document { get; }
        new ISwDmVersion Version { get; }
        new ISwDmCustomPropertiesCollection Properties { get; }

        TObj CreateObjectFromDispatch<TObj>(object disp)
            where TObj : ISwDmObject;
    }

    /// <summary>
    /// Core document wrapper responsible for open, save, close, and repository integration.
    /// 核心文档包装器，负责打开、保存、关闭以及与 xCAD 仓库体系的集成。
    /// </summary>
    [DebuggerDisplay("{" + nameof(Title) + "}")]
    internal abstract class SwDmDocument : SwDmObject, ISwDmDocument
    {
        /// <summary>
        /// <see cref="ISwDmComponent.CachedPath"/> returns the last path when file was saved within SOLIDWORKS
        /// If files were renamed with Pack&Go, SOLIDWORKS File Utilities, PDM or Document Manager cached path will not be changed until opened
        /// `<see cref="ISwDmComponent.CachedPath"/>` 返回的是文件上次在 SOLIDWORKS 中保存时记录的路径。
        /// 如果文件通过 Pack and Go、SOLIDWORKS File Utilities、PDM 或 Document Manager 改名，缓存路径要等到文件被真正打开后才会刷新。
        /// </summary>
        internal class ChangedReferencesCollection 
        {
            private readonly string[] m_OriginalReferences;
            private readonly string[] m_NewReferences;

            internal ChangedReferencesCollection(ISwDMDocument doc) 
            {
                ((ISwDMDocument8)doc).GetChangedReferences(out object origRefs, out object newRefs);

                m_OriginalReferences = (string[])origRefs ?? new string[0];
                m_NewReferences = (string[])newRefs ?? new string[0];

                if (m_OriginalReferences.Length != m_NewReferences.Length) 
                {
                    throw new Exception("Count of original references does not match count of new references");
                }
            }

            internal IEnumerable<string> EnumerateByFileName(string filePath) 
            {
                for (int i = 0; i < m_OriginalReferences.Length; i++)
                {
                    var origRef = m_OriginalReferences[i];

                    if (string.Equals(System.IO.Path.GetFileName(origRef), System.IO.Path.GetFileName(filePath), StringComparison.CurrentCultureIgnoreCase))
                    {
                        yield return m_NewReferences[i];
                    }
                }
            }
        }

        /// <summary>
        /// Determines the SOLIDWORKS native document type from the file extension.
        /// 根据文件扩展名判断 SOLIDWORKS 原生文档类型。
        /// </summary>
        internal static SwDmDocumentType GetDocumentType(string path)
        {
            SwDmDocumentType docType;

            if (!string.IsNullOrEmpty(path))
            {
                switch (System.IO.Path.GetExtension(path).ToLower())
                {
                    case ".sldprt":
                    case ".sldblk":
                    case ".prtdot":
                    case ".sldlfp":
                        docType = SwDmDocumentType.swDmDocumentPart;
                        break;

                    case ".sldasm":
                    case ".asmdot":
                        docType = SwDmDocumentType.swDmDocumentAssembly;
                        break;

                    case ".slddrw":
                    case ".drwdot":
                        docType = SwDmDocumentType.swDmDocumentDrawing;
                        break;

                    default:
                        throw new NotSupportedException("Only native SOLIDWORKS files can be opened");
                }
            }
            else
            {
                throw new NotSupportedException("Cannot extract document type when path is not specified");
            }

            return docType;
        }

        #region Not Supported

        public string Template { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
        public IXFeatureRepository Features => throw new NotImplementedException();
        public IXSelectionRepository Selections => throw new NotSupportedException();
        public IXDimensionRepository Dimensions => throw new NotSupportedException();
        public event DocumentEventDelegate Rebuilt
        {
            add => throw new NotSupportedException();
            remove => throw new NotSupportedException();
        }
        public TObj DeserializeObject<TObj>(Stream stream) where TObj : IXObject => throw new NotSupportedException();
        public void Rebuild() => throw new NotSupportedException();
        public IOperationGroup PreCreateOperationGroup() => throw new NotSupportedException();
        public IXUnits Units => throw new NotSupportedException();
        public IXModelViewRepository ModelViews => throw new NotSupportedException();
        public IXAnnotationRepository Annotations => throw new NotSupportedException();

        #endregion

        IXVersion IXDocument.Version => Version;
        IXPropertyRepository IPropertiesOwner.Properties => Properties;

        public ISwDMDocument Document => m_Creator.Element;

        public ISwDmVersion Version => SwDmApplicationFactory.CreateVersion((SwDmVersion_e)Document.GetVersion());

        public virtual string Title 
        {
            get 
            {
                var path = Path;

                if (!string.IsNullOrEmpty(path))
                {
                    if (IsFileExtensionShown)
                    {
                        return System.IO.Path.GetFileName(path);
                    }
                    else
                    {
                        return System.IO.Path.GetFileNameWithoutExtension(path);
                    }
                }
                else 
                {
                    return "";
                }
            }
            set => throw new NotSupportedException("This property is read-only");
        }

        public string Path 
        {
            get 
            {
                if (IsCommitted)
                {
                    return Document.FullName;
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
                    throw new NotSupportedException("Path cannot be changed for an opened document");
                }
                else 
                {
                    m_Creator.CachedProperties.Set(value);
                }
            }
        }

        public virtual bool IsDirty { get; set; }

        private bool? m_IsReadOnly;

        public DocumentState_e State 
        {
            get 
            {
                if (IsCommitted)
                {
                    if (m_IsReadOnly.Value)
                    {
                        return DocumentState_e.ReadOnly;
                    }
                    else 
                    {
                        return DocumentState_e.Default;
                    }
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
                    throw new Exception("This property is read-only");
                }
                else 
                {
                    if (value != DocumentState_e.Default && value != DocumentState_e.ReadOnly) 
                    {
                        throw new NotSupportedException("Only default and read-only states are supported");
                    }

                    m_Creator.CachedProperties.Set(value);
                }
            }
        }
        
        private bool? m_IsClosed;

        public override bool IsAlive 
        {
            get 
            {
                if (m_IsClosed.HasValue)
                {
                    return !m_IsClosed.Value;
                }
                else
                {
                    try
                    {
                        //This not causing exception - so does not work - keeping as placeholder for future
                        // 该调用当前不会触发异常，因此还不能可靠地作为存活性判断，只先保留占位逻辑。
                        var testVers = Document.GetVersion();

                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }
            }
        }

        public IXDocumentDependencies Dependencies { get; }

        public bool IsCommitted => m_Creator.IsCreated;

        public ISwDmCustomPropertiesCollection Properties => m_Properties.Value;

        public event DataStoreAvailableDelegate StreamReadAvailable;
        public event DataStoreAvailableDelegate StorageReadAvailable;
        public event DataStoreAvailableDelegate StreamWriteAvailable;
        public event DataStoreAvailableDelegate StorageWriteAvailable;
        
        public event DocumentSaveDelegate Saving;
        public event DocumentCloseDelegate Closing;
        public event DocumentEventDelegate Destroyed;

        protected readonly IElementCreator<ISwDMDocument> m_Creator;

        internal ChangedReferencesCollection ChangedReferences => m_ChangedReferencesLazy.Value;

        protected readonly Action<ISwDmDocument> m_CreateHandler;
        protected readonly Action<ISwDmDocument> m_CloseHandler;

        private readonly Lazy<ISwDmCustomPropertiesCollection> m_Properties;
        private readonly Lazy<ChangedReferencesCollection> m_ChangedReferencesLazy;

        internal SwDmDocument(SwDmApplication dmApp, ISwDMDocument doc, bool isCreated, 
            Action<ISwDmDocument> createHandler, Action<ISwDmDocument> closeHandler,
            bool? isReadOnly = null) : base(doc, dmApp, null)
        {
            m_IsReadOnly = isReadOnly;

            m_CreateHandler = createHandler;
            m_CloseHandler = closeHandler;

            Dependencies = new SwDmDocumentDependencies(this);

            m_Creator = new ElementCreator<ISwDMDocument>(OpenDocument, doc, isCreated);

            m_Properties = new Lazy<ISwDmCustomPropertiesCollection>(() => new SwDmDocumentCustomPropertiesCollection(this));
            m_ChangedReferencesLazy = new Lazy<ChangedReferencesCollection>(() => new ChangedReferencesCollection(Document));
        }

        public override object Dispatch => Document;

        public int UpdateStamp => Document.GetLastUpdateStamp();
        
        public override bool Equals(IXObject other)
        {
            if (!object.ReferenceEquals(this, other)
                && other is ISwDmDocument
                && !IsCommitted && !((ISwDmDocument)other).IsCommitted)
            {
                return !string.IsNullOrEmpty(Path) && !string.IsNullOrEmpty(((ISwDmDocument)other).Path)
                    && string.Equals(Path, ((ISwDmDocument)other).Path, StringComparison.CurrentCultureIgnoreCase);
            }
            else
            {
                return base.Equals(other);
            }
        }

        /// <summary>
        /// Opens the underlying document on first commit and raises storage availability notifications.
        /// 在首次提交时打开底层文档，并触发流与存储可用通知。
        /// </summary>
        private ISwDMDocument OpenDocument(CancellationToken cancellationToken) 
        {
            m_IsReadOnly = State.HasFlag(DocumentState_e.ReadOnly);

            var doc = OpenDocument(Path, State);

            StreamReadAvailable?.Invoke(this);
            StorageReadAvailable?.Invoke(this);

            return doc;
        }

        /// <summary>
        /// Opens a document by path and translates Document Manager open errors into xCAD exceptions.
        /// 按路径打开文档，并把 Document Manager 的打开错误转换为 xCAD 异常。
        /// </summary>
        private ISwDMDocument OpenDocument(string path, DocumentState_e state)
        {
            var isReadOnly = state.HasFlag(DocumentState_e.ReadOnly);

            var docType = GetDocumentType(path);

            if (!IsDocumentTypeCompatible(docType))
            {
                throw new DocumentPathIncompatibleException(this);
            }

            var doc = OwnerApplication.SwDocMgr.GetDocument(path, docType,
                isReadOnly, out SwDmDocumentOpenError err);

            if (doc != null)
            {
                return doc;
            }
            else
            {
                string errDesc;

                switch (err)
                {
                    case SwDmDocumentOpenError.swDmDocumentOpenErrorFail:
                        errDesc = "Generic error";
                        break;

                    case SwDmDocumentOpenError.swDmDocumentOpenErrorNonSW:
                        errDesc = "Not a native SOLIDWORKS file";
                        break;

                    case SwDmDocumentOpenError.swDmDocumentOpenErrorFileNotFound:
                        errDesc = "File not found";
                        break;

                    case SwDmDocumentOpenError.swDmDocumentOpenErrorFileReadOnly:
                        throw new DocumentWriteAccessDeniedException(path, (int)err);

                    case SwDmDocumentOpenError.swDmDocumentOpenErrorNoLicense:
                        errDesc = "No Document Manager license found";
                        break;

                    case SwDmDocumentOpenError.swDmDocumentOpenErrorFutureVersion:
                        errDesc = "Opening future version of the file";
                        break;

                    default:
                        errDesc = "Unknown error";
                        break;
                }

                throw new OpenDocumentFailedException(path, (int)err, errDesc);
            }
        }

        protected abstract bool IsDocumentTypeCompatible(SwDmDocumentType docType);

        /// <summary>
        /// Closes the native document and detaches it from the repository.
        /// 关闭底层文档，并将其从文档仓库中解除注册。
        /// </summary>
        public void Close()
        {
            if (!m_IsClosed.HasValue || !m_IsClosed.Value)
            {
                Document.CloseDoc();
                Closing?.Invoke(this, DocumentCloseType_e.Destroy);

                m_CloseHandler.Invoke(this);
                m_IsClosed = true;
                Destroyed?.Invoke(this);
            }
        }

        /// <summary>
        /// Commits a pre-created document and publishes it into the document repository.
        /// 提交预创建文档，并将其发布到文档仓库中。
        /// </summary>
        public virtual void Commit(CancellationToken cancellationToken)
        {
            m_Creator.Create(cancellationToken);
            m_CreateHandler.Invoke(this);
        }

        /// <summary>
        /// Opens a named third-party structured storage inside the document.
        /// 打开文档内指定名称的第三方结构化存储。
        /// </summary>
        public IStorage OpenStorage(string name, AccessType_e access)
        {
            if (this.IsVersionNewerOrEqual(SwDmVersion_e.Sw2015)) 
            {
                return new SwDm3rdPartyStorage((ISwDMDocument19)Document, name, access);
            }
            else 
            {
                throw new NotSupportedException("This API is only available in SOLIDWORKS 2015 or newer");
            }
        }

        /// <summary>
        /// Opens a named third-party stream inside the document.
        /// 打开文档内指定名称的第三方数据流。
        /// </summary>
        public Stream OpenStream(string name, AccessType_e access)
        {
            if (this.IsVersionNewerOrEqual(SwDmVersion_e.Sw2015))
            {
                return new SwDm3rdPartyStream((ISwDMDocument19)Document, name, access);
            }
            else
            {
                throw new NotSupportedException("This API is only available in SOLIDWORKS 2015 or newer");
            }
        }

        /// <summary>
        /// Saves the current document back to its existing path.
        /// 将当前文档保存回其现有路径。
        /// </summary>
        public void Save()
            => PerformSave(DocumentSaveType_e.SaveCurrent, Path, f =>
            {
                if (!string.Equals(f, Path))
                {
                    throw new NotSupportedException("File name can be changed for SaveAs file only");
                }

                return true;
            }, (d, f) => d.Save());

        /// <summary>
        /// Creates a deferred Save As operation that can be committed later.
        /// 创建一个延迟执行的另存为操作，稍后可通过提交执行。
        /// </summary>
        public IXSaveOperation PreCreateSaveAsOperation(string filePath)
            => new SwDmSaveOperation(this, filePath);

        /// <summary>
        /// Central save pipeline that raises events, exposes custom storages, and processes save results.
        /// 统一的保存流程：触发事件、开放自定义存储写入，并处理保存结果。
        /// </summary>
        internal void PerformSave(DocumentSaveType_e saveType, string path, Func<string, bool> canSave,
            Func<ISwDMDocument, string, SwDmDocumentSaveError> saveFunc) 
        {
            var saveArgs = new DocumentSaveArgs();
            saveArgs.FileName = path;

            Saving?.Invoke(this, saveType, saveArgs);

            if (!saveArgs.Cancel)
            {
                if (canSave.Invoke(saveArgs.FileName))
                {
                    StreamWriteAvailable?.Invoke(this);
                    StorageWriteAvailable?.Invoke(this);

                    var res = saveFunc.Invoke(Document, saveArgs.FileName);

                    if (ProcessSaveResult(res))
                    {
                        IsDirty = false;
                    }
                }
            }
        }

        /// <summary>
        /// Converts save error codes into exceptions meaningful to xCAD callers.
        /// 将保存错误码转换为对 xCAD 调用方更有意义的异常。
        /// </summary>
        private bool ProcessSaveResult(SwDmDocumentSaveError res)
        {
            if (res != SwDmDocumentSaveError.swDmDocumentSaveErrorNone)
            {
                string errDesc = "";

                switch (res)
                {
                    case SwDmDocumentSaveError.swDmDocumentSaveErrorFail:
                        errDesc = "Generic save error";
                        break;

                    case SwDmDocumentSaveError.swDmDocumentSaveErrorReadOnly:
                        errDesc = "Cannot save read-only file";
                        break;
                }

                throw new SaveDocumentFailedException((int)res, errDesc);
            }

            return true;
        }

        /// <summary>
        /// Checks the Windows Explorer setting to decide whether titles should include file extensions.
        /// 检查 Windows 资源管理器设置，以决定标题是否应包含文件扩展名。
        /// </summary>
        private bool IsFileExtensionShown
        {
            get
            {
                try
                {
                    const string REG_KEY = @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced";
                    const int UNCHECKED = 0;
                    var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(REG_KEY);

                    if (key != null)
                    {
                        return (int)key.GetValue("HideFileExt") == UNCHECKED;
                    }
                }
                catch
                {
                }

                return false;
            }
        }

        public IXDocumentOptions Options => throw new NotImplementedException();

        public TObj CreateObjectFromDispatch<TObj>(object disp) where TObj : ISwDmObject
            => SwDmObjectFactory.FromDispatch<TObj>(disp, this);

        public void Dispose()
        {
            if (m_IsClosed != true)
            {
                if (IsAlive)
                {
                    Close();
                }
            }
        }
    }

    /// <summary>
    /// Temporary document wrapper that resolves its concrete type after the file path is known.
    /// 临时未知文档包装器，会在已知文件路径后再解析为具体文档类型。
    /// </summary>
    internal class SwDmUnknownDocument : SwDmDocument, IXUnknownDocument
    {
        private SwDmDocument m_SpecificDoc;

        public SwDmUnknownDocument(SwDmApplication dmApp, SwDMDocument doc, bool isCreated,
            Action<ISwDmDocument> createHandler, Action<ISwDmDocument> closeHandler, bool? isReadOnly = null) 
            : base(dmApp, doc, isCreated, createHandler, closeHandler, isReadOnly)
        {
            if (isCreated)
            {
                m_CreateHandler.Invoke((ISwDmDocument)GetSpecific());
            }
        }

        public override void Commit(CancellationToken cancellationToken)
        {
            m_Creator.Create(cancellationToken);
            m_CreateHandler.Invoke((ISwDmDocument)GetSpecific());
        }

        /// <summary>
        /// Converts the unknown wrapper into a concrete part, assembly, or drawing wrapper.
        /// 把未知文档包装器转换为具体的零件、装配体或工程图包装器。
        /// </summary>
        public IXDocument GetSpecific()
        {
            if (m_SpecificDoc != null)
            {
                return m_SpecificDoc;
            }

            var model = IsCommitted ? Document : null;

            var isReadOnly = State.HasFlag(DocumentState_e.ReadOnly);

            switch (GetDocumentType(Path))
            {
                case SwDmDocumentType.swDmDocumentPart:
                    m_SpecificDoc = new SwDmPart(OwnerApplication, model, IsCommitted, m_CreateHandler, m_CloseHandler, isReadOnly);
                    break;

                case SwDmDocumentType.swDmDocumentAssembly:
                    m_SpecificDoc = new SwDmAssembly(OwnerApplication, model, IsCommitted, m_CreateHandler, m_CloseHandler, isReadOnly);
                    break;

                case SwDmDocumentType.swDmDocumentDrawing:
                    m_SpecificDoc = new SwDmDrawing(OwnerApplication, model, IsCommitted, m_CreateHandler, m_CloseHandler, isReadOnly);
                    break;

                default:
                    throw new Exception("Invalid document type");
            }

            if (!IsCommitted) 
            {
                //TODO: implement copy cache on ElementCreator
                // TODO：未来可把预创建阶段缓存的属性复制到具体文档包装器中。
                m_SpecificDoc.Path = Path;
            }

            return m_SpecificDoc;
        }

        protected override bool IsDocumentTypeCompatible(SwDmDocumentType docType) => true;
    }

    /// <summary>
    /// Placeholder 3D unknown document used when the exact part/assembly type is still unresolved.
    /// 当尚未确定是零件还是装配体时使用的三维未知文档占位实现。
    /// </summary>
    internal class SwDmUnknownDocument3D : SwDmUnknownDocument, ISwDmDocument3D
    {
        public SwDmUnknownDocument3D(SwDmApplication dmApp, SwDMDocument doc, bool isCreated, Action<ISwDmDocument> createHandler, Action<ISwDmDocument> closeHandler, bool? isReadOnly = null)
            : base(dmApp, doc, isCreated, createHandler, closeHandler, isReadOnly)
        {
        }

        IXModelView3DRepository IXDocument3D.ModelViews => throw new NotSupportedException();
        public ISwDmConfigurationCollection Configurations => throw new NotSupportedException();
        public IXDocumentEvaluation Evaluation => throw new NotSupportedException();
        public IXDocumentGraphics Graphics => throw new NotSupportedException();
        IXConfigurationRepository IXDocument3D.Configurations => throw new NotSupportedException();
        TSelObject IXObjectContainer.ConvertObject<TSelObject>(TSelObject obj) => throw new NotSupportedException();
        IXDocument3DSaveOperation IXDocument3D.PreCreateSaveAsOperation(string filePath) => throw new NotSupportedException();
    }

    /// <summary>
    /// Extension helpers for version-based document capability checks.
    /// 基于版本判断文档能力的扩展辅助方法。
    /// </summary>
    public static class SwDmDocumentExtension 
    {
        public static bool IsVersionNewerOrEqual(this ISwDmDocument doc, SwDmVersion_e version)
            => doc.Version.IsVersionNewerOrEqual(version);
    }
}
