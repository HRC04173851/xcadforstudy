// -*- coding: utf-8 -*-
// src/Base/Documents/IXDocument.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件定义 xCAD 框架的文档基础接口。
// IXDocument 是所有文档类型（零件、装配体、工程图等）的统一抽象。
//
// 设计原则：
// 1. 跨CAD平台：此接口不依赖任何特定 CAD 软件的 API
// 2. 统一访问：通过统一接口访问不同 CAD 软件的文档
// 3. 延迟实现：具体实现由各 CAD 软件的插件完成（如 SolidWorks、Inventor）
//
// 核心功能：
// - 文档生命周期：创建、打开、保存、关闭
// - 文档属性：标题、路径、状态、版本
// - 子系统访问：特性、选择、尺寸、图层、视图等
// - 事件通知：保存、关闭、销毁等事件
// - 扩展数据：支持 Stream/Storage 方式的持久化数据存储
//*********************************************************************

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Xarial.XCad.Annotations;
using Xarial.XCad.Base;
using Xarial.XCad.Data;
using Xarial.XCad.Data.Enums;
using Xarial.XCad.Documents.Delegates;
using Xarial.XCad.Documents.Enums;
using Xarial.XCad.Documents.Services;
using Xarial.XCad.Features;

namespace Xarial.XCad.Documents
{
    /// <summary>
    /// Represents the base interface of all document types
    /// <para>中文：所有文档类型的基础接口</para>
    /// </summary>
    public interface IXDocument : IXObject, IXTransaction, IPropertiesOwner, IDimensionable, IDisposable
    {
        /// <summary>
        /// Current version of the document
        /// <para>中文：文档的当前版本</para>
        /// </summary>
        IXVersion Version { get; }

        /// <summary>
        /// Fired when user data stream is available for reading
        /// <para>中文：用户数据流可供读取时触发</para>
        /// </summary>
        event DataStoreAvailableDelegate StreamReadAvailable;

        /// <summary>
        /// Fired when user data storage is available for reading
        /// <para>中文：用户数据存储可供读取时触发</para>
        /// </summary>
        event DataStoreAvailableDelegate StorageReadAvailable;

        /// <summary>
        /// Fired when user data stream is available for writing
        /// <para>中文：用户数据流可供写入时触发</para>
        /// </summary>
        event DataStoreAvailableDelegate StreamWriteAvailable;

        /// <summary>
        /// Fired when user data storage is available for writing
        /// <para>中文：用户数据存储可供写入时触发</para>
        /// </summary>
        event DataStoreAvailableDelegate StorageWriteAvailable;

        /// <summary>
        /// Fired when document is rebuilt
        /// <para>中文：文档重建时触发</para>
        /// </summary>
        event DocumentEventDelegate Rebuilt;

        /// <summary>
        /// Fired when documetn is saving
        /// <para>中文：文档正在保存时触发</para>
        /// </summary>
        event DocumentSaveDelegate Saving;

        /// <summary>
        /// Fired when document is closing
        /// <para>中文：文档正在关闭时触发</para>
        /// </summary>
        event DocumentCloseDelegate Closing;

        /// <summary>
        /// Fired when document is destroyed
        /// <para>中文：文档被销毁时触发</para>
        /// </summary>
        event DocumentEventDelegate Destroyed;

        /// <summary>
        /// Units assigned in this document
        /// <para>中文：此文档中使用的单位设置</para>
        /// </summary>
        IXUnits Units { get; }

        /// <summary>
        /// Document specific options
        /// <para>中文：文档专有选项</para>
        /// </summary>
        IXDocumentOptions Options { get; }

        /// <summary>
        /// Changes the title of this document
        /// <para>中文：文档标题（可读写）</para>
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// Document template
        /// <para>中文：文档模板路径</para>
        /// </summary>
        string Template { get; set; }

        /// <summary>
        /// Path to the document (if saved)
        /// <para>中文：文档的文件路径（已保存时有效）</para>
        /// </summary>
        string Path { get; set; }

        /// <summary>
        /// Checks if document has any unsaved changes
        /// <para>中文：检查文档是否包含未保存的更改</para>
        /// </summary>
        bool IsDirty { get; set; }

        /// <summary>
        /// Gets or sets the state of the document
        /// <para>中文：获取或设置文档的状态</para>
        /// </summary>
        DocumentState_e State { get; set; }

        /// <summary>
        /// Returns views collection
        /// <para>中文：返回模型视图集合</para>
        /// </summary>
        IXModelViewRepository ModelViews { get; }

        /// <summary>
        /// Closes this document
        /// <para>中文：关闭此文档</para>
        /// </summary>
        void Close();

        /// <summary>
        /// Saves this document
        /// <para>中文：保存此文档</para>
        /// </summary>
        /// <exception cref="Exceptions.SaveNeverSavedDocumentException"/>
        /// <exception cref="Exceptions.SaveDocumentFailedException"/>
        void Save();

        /// <summary>
        /// Pre-creates save-as operation
        /// <para>中文：预创建另存为操作</para>
        /// </summary>
        /// <param name="filePath"></param>
        IXSaveOperation PreCreateSaveAsOperation(string filePath);

        /// <summary>
        /// Collection of annotations
        /// <para>中文：注释集合</para>
        /// </summary>
        IXAnnotationRepository Annotations { get; }

        /// <summary>
        /// Collection of features of this document
        /// <para>中文：此文档的特征集合</para>
        /// </summary>
        IXFeatureRepository Features { get; }

        /// <summary>
        /// Collection of selections of this document
        /// <para>中文：此文档的选择集合</para>
        /// </summary>
        IXSelectionRepository Selections { get; }
        
        /// <summary>
        /// Opens the user data stream from this document
        /// <para>中文：从此文档打开用户数据流</para>
        /// </summary>
        /// <param name="name">Name of the stream</param>
        /// <param name="access">Access type</param>
        /// <returns>Pointer to stream</returns>
        Stream OpenStream(string name, AccessType_e access);
        
        /// <summary>
        /// Opens the user data storage from this document
        /// <para>中文：从此文档打开用户数据存储</para>
        /// </summary>
        /// <param name="name">Name of the storage</param>
        /// <param name="access">Access type</param>
        /// <returns>Pointer to the storage</returns>
        IStorage OpenStorage(string name, AccessType_e access);

        /// <summary>
        /// Returns top level dependencies of this document
        /// <para>中文：返回此文档的顶层依赖项</para>
        /// </summary>
        /// <remarks>Dependencies might be uncommited if document is loaded view only or in the rapid mode. Use <see cref="IXTransaction.IsCommitted"/> to check the state and call <see cref="IXTransaction.Commit(System.Threading.CancellationToken)"/> to load document if needed.
        /// In most CADs this method will work with uncommitted documents</remarks>
        IXDocumentDependencies Dependencies { get; }

        /// <summary>
        /// Deserializes specific object from stream
        /// <para>中文：从流中反序列化指定对象</para>
        /// </summary>
        /// <param name="stream">Input stream with the serialized object</param>
        /// <returns>Deserialized object</returns>
        TObj DeserializeObject<TObj>(Stream stream)
            where TObj : IXObject;

        /// <summary>
        /// Regenerates this document
        /// <para>中文：重新生成（重建）此文档</para>
        /// </summary>
        void Rebuild();

        /// <summary>
        /// Starts the group of opertions
        /// <para>中文：开始一组操作（用于批量撤销支持）</para>
        /// </summary>
        /// <returns>Operation group template</returns>
        IOperationGroup PreCreateOperationGroup();

        /// <summary>
        /// Returns the time stamp of the change of the current model
        /// <para>中文：返回当前模型更改的时间戳</para>
        /// </summary>
        int UpdateStamp { get; }
    }

    /// <summary>
    /// Represents the unknown document type
    /// <para>中文：表示类型未知的文档</para>
    /// </summary>
    /// <remarks>This interface provides an access to the document whose specific type cannot be determined in advance
    /// (e.g. imported document types might be both parts and assemblies and it is not known until the document is opened)</remarks>
    public interface IXUnknownDocument : IXDocument
    {
        /// <summary>
        /// Retrieves the specific document from the unknown document
        /// <para>中文：从未知文档中获取具体类型的文档实例</para>
        /// </summary>
        /// <returns></returns>
        IXDocument GetSpecific();
    }
}