// -*- coding: utf-8 -*-
// IXWorkUnit.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义工作单元接口及其操作委托，支持同步和异步操作，包含成功、错误、取消等结果类型
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Xarial.XCad.Base
{
    /// <summary>
    /// Operation to execute in the <see cref="IXWorkUnit"/>
    /// 在 <see cref="IXWorkUnit"/> 中执行的操作委托
    /// </summary>
    /// <param name="prg">Progress handler 进度处理程序</param>
    /// <param name="cancellationToken">Cancellation token 取消令牌</param>
    public delegate IXWorkUnitResult WorkUnitOperationDelegate(IXProgress prg, CancellationToken cancellationToken);

    /// <summary>
    /// Async operation to execute in the <see cref="IXAsyncWorkUnit"/>
    /// 在 <see cref="IXAsyncWorkUnit"/> 中执行的异步操作委托
    /// </summary>
    /// <param name="prg">Progress handler 进度处理程序</param>
    /// <param name="cancellationToken">Cancellation token 取消令牌</param>
    public delegate Task<IXWorkUnitResult> AsyncWorkUnitOperationDelegate(IXProgress prg, CancellationToken cancellationToken);

    /// <summary>
    /// Result of the work unit
    /// 工作单元的执行结果
    /// </summary>
    public interface IXWorkUnitResult
    {
    }

    /// <summary>
    /// Error result
    /// 工作单元的错误结果
    /// </summary>
    public interface IXWorkUnitErrorResult : IXWorkUnitResult
    {
        /// <summary>
        /// Error of the work unit
        /// 工作单元发生的异常
        /// </summary>
        Exception Error { get; }
    }

    /// <summary>
    /// Cancelled result
    /// 工作单元被取消的结果
    /// </summary>
    public interface IXWorkUnitCancelledResult : IXWorkUnitResult
    {
    }

    /// <summary>
    /// User specific result
    /// 用户自定义的工作单元结果
    /// </summary>
    /// <typeparam name="TRes">Type of result 结果类型</typeparam>
    public interface IXWorkUnitUserResult<TRes> : IXWorkUnitResult
    {
        /// <summary>
        /// Result
        /// 用户自定义结果值
        /// </summary>
        TRes Result { get; }
    }

    /// <summary>
    /// Work unit created by <see cref="Extensions.IXExtension.PreCreateWorkUnit"/>
    /// 由 <see cref="Extensions.IXExtension.PreCreateWorkUnit"/> 创建的工作单元
    /// </summary>
    public interface IXWorkUnit : IXTransaction
    {   
        /// <summary>
        /// RWork unit result
        /// 工作单元的执行结果
        /// </summary>
        IXWorkUnitResult Result { get; }

        /// <summary>
        /// Operation of this work unit
        /// 此工作单元的执行操作
        /// </summary>
        WorkUnitOperationDelegate Operation { get; set; }
    }

    /// <summary>
    /// Async <see cref="IXWorkUnit"/>
    /// 异步版本的 <see cref="IXWorkUnit"/>
    /// </summary>
    public interface IXAsyncWorkUnit : IXAsyncTransaction
    {
        /// <summary>
        /// RWork unit result
        /// 异步工作单元的执行结果
        /// </summary>
        IXWorkUnitResult Result { get; }

        /// <summary>
        /// Async operation of this work unit
        /// 此工作单元的异步执行操作
        /// </summary>
        AsyncWorkUnitOperationDelegate AsyncOperation { get; set; }
    }
}
