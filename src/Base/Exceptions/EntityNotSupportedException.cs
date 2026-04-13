//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xarial.XCad.Base;

namespace Xarial.XCad.Exceptions
{
    /// <summary>
    /// Exception is thrown if <see cref="IXRepository.PreCreate{T}"/> does not support the specific entity
    /// 当 <see cref="IXRepository.PreCreate{T}"/> 不支持指定实体类型时抛出此异常
    /// </summary>
    public class EntityNotSupportedException : NotSupportedException
    {
        /// <summary>
        /// Default constructor
        /// 默认构造函数
        /// </summary>
        public EntityNotSupportedException(Type entType, IReadOnlyList<Type> supportedTypes)
            : base($"Entity {entType.FullName} is not supported by the repository. Supported types: {string.Join(", ", supportedTypes.Select(t=>t.FullName))}")
        {
        }
    }
}
