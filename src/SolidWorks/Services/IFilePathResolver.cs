// -*- coding: utf-8 -*-
// src/SolidWorks/Services/IFilePathResolver.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义文件路径解析器接口，用于解析文档引用的相对路径，支持根据SOLIDWORKS搜索规则定位被引用的文件。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Documents;
using Xarial.XCad.SolidWorks.Documents;

namespace Xarial.XCad.SolidWorks.Services
{
    public interface IFilePathResolver
    {
        string ResolvePath(string parentDocPath, string path);
    }
}
