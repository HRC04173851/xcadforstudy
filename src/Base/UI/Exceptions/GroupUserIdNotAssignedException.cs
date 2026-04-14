// -*- coding: utf-8 -*-
// src\Base\UI\Exceptions\GroupUserIdNotAssignedException.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 当通过反射从枚举创建命令组规范时未分配组用户ID时引发此异常，需通过特性或显式赋值指定用户ID。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.UI.Commands;
using Xarial.XCad.UI.Commands.Attributes;

namespace Xarial.XCad.UI.Exceptions
{
    /// <summary>
    /// Indicates that no user id assigned in <see cref="XCommandManagerExtension.CreateSpecFromEnum"/>
    /// 表示在 <see cref="XCommandManagerExtension.CreateSpecFromEnum"/> 中未分配组用户 ID
    /// </summary>
    public class GroupUserIdNotAssignedException : Exception
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public GroupUserIdNotAssignedException() : base($"User id must be specified or assigned via {typeof(CommandGroupInfoAttribute).FullName} attribute") 
        {
        }
    }
}
