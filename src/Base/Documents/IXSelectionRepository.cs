//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
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
