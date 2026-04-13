//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.Geometry
{
    /// <summary>
    /// Indicates that this object can be resilient to the regeneration operations
    /// 表示对象可在模型重建后保持可追踪（具备韧性引用）
    /// </summary>
    public interface ISupportsResilience : IXObject
    {
        /// <summary>
        /// Is object resilient to regeneration
        /// 对象是否已具备重建韧性
        /// </summary>
        bool IsResilient { get; }

        /// <summary>
        /// Converts this object to resilient object
        /// 将当前对象转换为韧性对象
        /// </summary>
        /// <returns>Resilient object</returns>
        IXObject CreateResilient();
    }

    /// <inheritdoc/>
    /// <typeparam name="T">Specific object type</typeparam>
    public interface ISupportsResilience<T> : ISupportsResilience
        where T : IXObject
    {
        /// <summary>
        /// Specific implementation of resilient object
        /// 返回强类型韧性对象
        /// </summary>
        /// <returns>Resilient object</returns>
        new T CreateResilient();
    }
}
