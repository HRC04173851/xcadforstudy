// -*- coding: utf-8 -*-
// src/Base/Documents/IXSheetRepository.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义图纸仓储接口，管理工程图中的图纸集合，
// 提供图纸激活、创建事件通知及当前图纸的访问功能。
//*********************************************************************

using Xarial.XCad.Base;
using Xarial.XCad.Documents.Delegates;

namespace Xarial.XCad.Documents
{
    /// <summary>
    /// Represents sheets collection
    /// 表示图纸集合仓储
    /// </summary>
    public interface IXSheetRepository : IXRepository<IXSheet>
    {
        /// <summary>
        /// Fired when sheet is activated
        /// 图纸激活时触发
        /// </summary>
        event SheetActivatedDelegate SheetActivated;

        /// <summary>
        /// Fired when new sheet is created
        /// 新建图纸时触发
        /// </summary>
        event SheetCreatedDelegate SheetCreated;

        /// <summary>
        /// Returns or sets the active sheet in this sheets repository
        /// 获取或设置当前激活图纸
        /// </summary>
        IXSheet Active { get; set; }
    }
}
