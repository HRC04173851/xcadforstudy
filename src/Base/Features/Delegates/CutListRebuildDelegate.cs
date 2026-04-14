// -*- coding: utf-8 -*-
// src/Base/Features/Delegates/CutListRebuildDelegate.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 切割清单（Cut List）重建事件的委托定义。
// 当切割清单文件夹更新时触发此事件通知。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Documents;

namespace Xarial.XCad.Features.Delegates
{
    /// <summary>
    /// Delegate for <see cref="IXCutListItemRepository.CutListRebuild"/> event
    /// <see cref="IXCutListItemRepository.CutListRebuild"/> 事件委托
    /// </summary>
    /// <param name="cutList">Cut-list being rebuilt（正在重建的切割清单）</param>
    public delegate void CutListRebuildDelegate(IXCutListItemRepository cutList);
}
