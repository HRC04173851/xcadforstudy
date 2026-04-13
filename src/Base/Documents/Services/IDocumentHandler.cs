//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
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
