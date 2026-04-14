// -*- coding: utf-8 -*-
// src/Base/Exceptions/ICriticalException.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 标识可能已损坏当前进程的错误异常接口，表明该错误具有严重性可能影响进程稳定性
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.Exceptions
{
    /// <summary>
    /// Exception indicates that the error might have corrupted the current process
    /// 此异常表示错误可能已损坏当前进程
    /// </summary>
    public interface ICriticalException
    {
    }
}
