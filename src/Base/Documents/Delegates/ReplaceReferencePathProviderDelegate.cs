// -*- coding: utf-8 -*-
// src/Base/Documents/Delegates/ReplaceReferencePathProviderDelegate.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 提供引用路径替换提供者的委托定义，用于在批量替换引用路径时根据源路径返回目标路径。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xarial.XCad.Documents.Delegates
{
    /// <summary>
    /// Delegate used in the <see cref="XDocumentDependenciesExtension.ReplaceAll(IXDocumentDependencies, ReplaceReferencePathProviderDelegate, Func{string, string})"/>
    /// 在 <see cref="XDocumentDependenciesExtension.ReplaceAll(IXDocumentDependencies, ReplaceReferencePathProviderDelegate, Func{string, string})"/> 中用于提供替换引用路径的委托
    /// </summary>
    /// <param name="srcPath">Path to be replaced</param>
    /// <returns>Replacement path (can be the same if reference does not need to be replaced)</returns>
    public delegate string ReplaceReferencePathProviderDelegate(string srcPath);
}
