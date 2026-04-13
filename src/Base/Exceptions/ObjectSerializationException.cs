//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Xarial.XCad.Exceptions
{
    /// <summary>
    /// Exception indicates an error with serialization and deserialization
    /// 此异常表示序列化与反序列化过程中发生错误
    /// </summary>
    public class ObjectSerializationException : SerializationException, IUserException
    {
        /// <summary>
        /// CAD specific error code
        /// CAD特定的错误代码
        /// </summary>
        public int ErrorCode { get; }

        /// <summary>
        /// Default constructor
        /// 默认构造函数
        /// </summary>
        /// <param name="message">User friendly error 用户友好的错误描述</param>
        /// <param name="errCode">CAD specific error code CAD特定的错误代码</param>
        public ObjectSerializationException(string message, int errCode) : base(message)
        {
            ErrorCode = errCode;
        }
    }
}
