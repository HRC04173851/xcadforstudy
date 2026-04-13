//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Exceptions;

namespace Xarial.XCad.Geometry.Exceptions
{
    /// <summary>
    /// Indicates that mass property cannot be created
    /// 表示无法创建几何评估结果（如质量属性）
    /// </summary>
    public class EvaluationFailedException : NullReferenceException, IUserException
    {
        /// <summary>
        /// Default constructor
        /// 默认构造函数
        /// </summary>
        public EvaluationFailedException() : base("Cannot perform the evaluation for this model. Make sure that model contains the valid geometry")
        {
        }

        /// <summary>
        /// Specific evaluation exception
        /// 指定评估失败信息的构造函数
        /// </summary>
        /// <param name="error">Error description</param>
        public EvaluationFailedException(string error) : base(error)
        {
        }
    }
}
