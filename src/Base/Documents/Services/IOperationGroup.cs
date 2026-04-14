// -*- coding: utf-8 -*-
// src/Base/Documents/Services/IOperationGroup.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义操作分组接口，用于将多个API命令组合为单个撤销/重做事务。
// 支持临时分组模式，允许在执行命令后自动恢复分组状态。
//*********************************************************************

using System;
using Xarial.XCad.Base;

namespace Xarial.XCad.Documents.Services
{
    /// <summary>
    /// Group of operations (commands)
    /// 操作（命令）分组
    /// </summary>
    /// <remarks>This allows to group APi command under as single command for undo-redo purposes</remarks>
    public interface IOperationGroup : IXTransaction, IDisposable
    {
        /// <summary>
        /// Name of the group
        /// 分组名称
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Indicates if this group is temporar and should be restored after the execution
        /// 指示该分组是否为临时分组（执行后恢复）
        /// </summary>
        bool IsTemp { get; set; }
    }
}