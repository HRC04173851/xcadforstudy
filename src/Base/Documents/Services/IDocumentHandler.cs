// -*- coding: utf-8 -*-
// src/Base/Documents/Services/IDocumentHandler.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义文档处理器接口，用于管理文档生命周期事件（如初始化和释放）。
// 该接口允许自定义处理器在文档创建时被调用，支持应用程序级别的文档操作扩展。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.Documents.Services
{
    /// <summary>
    /// Document handler to be used in <see cref="IXDocumentRepository.RegisterHandler{THandler}(Func{THandler})"/> documents manager
    /// 文档处理器接口，用于 <see cref="IXDocumentRepository.RegisterHandler{THandler}(Func{THandler})"/> 注册的文档管理流程
    /// </summary>
    public interface IDocumentHandler : IDisposable
    {
        /// <summary>
        /// Called when model document is initialized (created)
        /// 当模型文档初始化（创建）时调用
        /// </summary>
        /// <param name="app">Pointer to application</param>
        /// <param name="doc">Pointer to this model document</param>
        void Init(IXApplication app, IXDocument doc);
    }
}
