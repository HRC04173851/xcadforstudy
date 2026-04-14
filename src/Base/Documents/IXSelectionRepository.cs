// -*- coding: utf-8 -*-
// src/Base/Documents/IXSelectionRepository.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件定义选择仓储（Selection Repository）的接口。
// Selection Repository 管理 CAD 文档中的选择操作和选择集。
//
// Selection Repository 核心功能：
// 1. 选择管理：Add、Remove、Clear 等操作
// 2. 选择事件：NewSelection、ClearSelection 事件通知
// 3. 选择标注：PreCreateCallout 创建选择气泡
// 4. 批量选择：ReplaceRange 替换整个选择集
//
// 选择对象类型：
// - IXSelObject：所有可选择对象的基接口
// - IXEntity：拓扑实体（Face、Edge、Vertex）
// - IXFeature：特征对象
// - IXComponent：装配体组件
//
// 选择过滤器：
// - 按类型过滤（Feature、Body、Component 等）
// - 按可见性过滤
// - 按状态过滤
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Xarial.XCad.Base;
using Xarial.XCad.Documents.Delegates;
using Xarial.XCad.Graphics;
using Xarial.XCad.UI;
using static System.Net.Mime.MediaTypeNames;

namespace Xarial.XCad.Documents
{
    /// <summary>
    /// Handles the selection objects
    /// 管理选择对象集合
    /// </summary>
    public interface IXSelectionRepository : IXRepository<IXSelObject>
    {
        /// <summary>
        /// Raised when new object is selected
        /// 新对象被选择时触发
        /// </summary>
        event NewSelectionDelegate NewSelection;

        /// <summary>
        /// Raised when the selection is cleared
        /// 选择集被清空时触发
        /// </summary>
        event ClearSelectionDelegate ClearSelection;

        /// <summary>
        /// Clears all current selections
        /// 清空当前全部选择
        /// </summary>
        void Clear();

        /// <summary>
        /// Replaces the selection (clears previous selection)
        /// 替换选择集（先清空原选择）
        /// </summary>
        /// <param name="ents">Entities to select</param>
        /// <param name="cancellationToken">Cancellation token</param>
        void ReplaceRange(IEnumerable<IXSelObject> ents, CancellationToken cancellationToken);

        /// <summary>
        /// Pre-creates selection callout instance
        /// 预创建选择标注气泡实例
        /// </summary>
        /// <returns>Instance of the selection callout</returns>
        IXSelCallout PreCreateCallout();
    }

    /// <summary>
    /// Additional methods for <see cref="IXSelectionRepository"/>
    /// <see cref="IXSelectionRepository"/> 的扩展方法
    /// </summary>
    public static class XSelectionRepositoryExtension 
    {
        /// <summary>
        /// Replaces the selection (clears previous selection)
        /// </summary>
        /// <param name="selRepo">Selection repository</param>
        /// <param name="ents">Entities to select</param>
        public static void ReplaceRange(this IXSelectionRepository selRepo, IEnumerable<IXSelObject> ents)
            => selRepo.ReplaceRange(ents, default);
    }
}
