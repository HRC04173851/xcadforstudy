// -*- coding: utf-8 -*-
// src/Base/Exceptions/EntityNotFoundException.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 当请求的实体在存储库中不存在时抛出此异常，表示指定的实体未找到
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.Exceptions
{
    /// <summary>
    /// Exception indicates that specific entity is note present within the repository
    /// 此异常表示指定的实体不存在于存储库中
    /// </summary>
    public class EntityNotFoundException : KeyNotFoundException
    {
        /// <summary>
        /// Defaut constructor
        /// 默认构造函数
        /// </summary>
        /// <param name="name">Name of the entity 实体名称</param>
        public EntityNotFoundException(string name) : base($"Entity '{name}' is not found in the repository")
        {
        }
    }
}
