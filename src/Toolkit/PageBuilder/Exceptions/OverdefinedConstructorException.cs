//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;

namespace Xarial.XCad.Utils.PageBuilder.Exceptions
{
    /// <summary>
    /// Exception indicates duplicate constructor registration for the same key type.
    /// <para>异常指示同一个键类型重复注册了构造器。</para>
    /// </summary>
    public class OverdefinedConstructorException : Exception
    {
        /// <summary>
        /// Initializes exception for duplicated constructor definition.
        /// <para>为重复构造器定义初始化异常。</para>
        /// </summary>
        internal OverdefinedConstructorException(Type constrType, Type keyType)
            : base($"Constructor of type {constrType.FullName} is already registered for {keyType.FullName}")
        {
        }
    }
}