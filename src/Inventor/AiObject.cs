// -*- coding: utf-8 -*-
// src/Inventor/AiObject.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// Inventor对象基类，实现IXObject接口。
// 提供对象的所有权管理（应用程序和文档），封装Inventor COM对象的调度。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xarial.XCad.Data;
using Xarial.XCad.Documents;
using Xarial.XCad.Inventor.Documents;

namespace Xarial.XCad.Inventor
{
    public interface IAiObject : IXObject
    {
        object Dispatch { get; }
    }

    internal class AiObject : IAiObject
    {
        IXApplication IXObject.OwnerApplication => OwnerApplication;
        IXDocument IXObject.OwnerDocument => OwnerDocument;

        public virtual object Dispatch { get; }

        internal virtual AiDocument OwnerDocument { get; }
        internal AiApplication OwnerApplication { get; }

        public virtual bool IsAlive => true;

        public ITagsManager Tags => throw new NotImplementedException();

        internal AiObject(object dispatch, AiDocument ownerDoc, AiApplication ownerApp) 
        {
            Dispatch = dispatch;
            OwnerDocument = ownerDoc;
            OwnerApplication = ownerApp;
        }

        public virtual bool Equals(IXObject other)
        {
            throw new NotImplementedException();
        }

        public void Serialize(Stream stream)
        {
            throw new NotImplementedException();
        }
    }
}
