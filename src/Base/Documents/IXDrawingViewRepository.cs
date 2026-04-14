// -*- coding: utf-8 -*-
// src/Base/Documents/IXDrawingViewRepository.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义工程图视图仓储接口，管理工程图中的视图集合，
// 提供视图创建事件通知功能。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Base;
using Xarial.XCad.Documents.Delegates;

namespace Xarial.XCad.Documents
{
    /// <summary>
    /// Represents the collection of drawing views
    /// 表示工程图视图集合
    /// </summary>
    public interface IXDrawingViewRepository : IXRepository<IXDrawingView>
    {
        /// <summary>
         /// Raised when new view is created
         /// 新建工程图视图时触发
         /// </summary>
        event DrawingViewCreatedDelegate ViewCreated;
    }
}
