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
using System.Threading.Tasks;

namespace Xarial.XCad.Base
{
    /// <summary>
    /// Represents the element which can be precreated
    /// <para>中文：表示可以预创建（延迟提交）的元素</para>
    /// </summary>
    /// <Remarks>Those elements usually got created within the <see cref="IXRepository{TEnt}.AddRange(IEnumerable{TEnt}, CancellationToken)"/>
    public interface IXTransaction
    {
        /// <summary>
        /// Identifies if this element is created or a template
        /// <para>中文：标识此元素是已创建的对象还是模板（未提交状态）</para>
        /// </summary>
        bool IsCommitted { get; }

        /// <summary>
        /// Commits this transaction
        /// <para>中文：提交此事务，将模板对象实际创建到文档中</para>
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        void Commit(CancellationToken cancellationToken);
    }

    /// <summary>
    /// Represents the <see cref="IXTransaction"/> which supports async operation
    /// <para>中文：表示支持异步操作的 <see cref="IXTransaction"/> 事务接口</para>
    /// </summary>
    public interface IXAsyncTransaction
    {/// <summary>
     /// Identifies if this element is created or a template
     /// <para>中文：标识此元素是已创建的对象还是模板（未提交状态）</para>
     /// </summary>
        bool IsCommitted { get; }

        /// <summary>
        /// Commits this transaction
        /// <para>中文：异步提交此事务，将模板对象实际创建到文档中</para>
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        Task CommitAsync(CancellationToken cancellationToken);
    }

    /// <summary>
    /// Additional methods for <see cref="IXTransaction"/>
    /// <para>中文：<see cref="IXTransaction"/> 的附加扩展方法</para>
    /// </summary>
    public static class XTransactionExtension
    {
        /// <summary>
        /// Commits the transaction with default cancellation token
        /// <para>中文：使用默认取消令牌提交事务</para>
        /// </summary>
        /// <param name="transaction">Transaction to commit</param>
        public static void Commit(this IXTransaction transaction) 
            => transaction.Commit(CancellationToken.None);

        /// <summary>
        /// Commits async transaction with the default cancellation token
        /// <para>中文：使用默认取消令牌异步提交事务</para>
        /// </summary>
        /// <param name="transaction">Async transaction to commit</param>
        /// <returns>Task</returns>
        public static Task CommitAsync(this IXAsyncTransaction transaction)
            => transaction.CommitAsync(CancellationToken.None);
    }
}
