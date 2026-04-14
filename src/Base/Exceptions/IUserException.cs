// -*- coding: utf-8 -*-
// src/Base/Exceptions/IUserException.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 表示异常的消息可以显示给用户的接口，用于标识用户友好的异常类型
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.Exceptions
{
    /// <summary>
    /// Indicates that the message of this exception can be displayed to the user
    /// 表示此异常的消息可以显示给用户
    /// </summary>
    public interface IUserException
    {
    }
}
