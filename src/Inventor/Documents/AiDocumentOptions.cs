// -*- coding: utf-8 -*-
// src/Inventor/Documents/AiDocumentOptions.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// Inventor文档选项类，实现IXDocumentOptions接口。
// 提供文档相关的视图实体可见性选项配置功能。
//*********************************************************************

using System;
using Xarial.XCad.Documents;

namespace Xarial.XCad.Inventor.Documents
{
    internal class AiDocumentOptions : IXDocumentOptions
    {
        public IXViewEntityKindVisibilityOptions ViewEntityKindVisibility => throw new NotImplementedException();

        internal AiDocumentOptions() 
        {
        }
    }
}
