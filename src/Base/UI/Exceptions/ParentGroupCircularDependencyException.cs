// -*- coding: utf-8 -*-
// src\Base\UI\Exceptions\ParentGroupCircularDependencyException.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 当父命令组被设置为其自身的父级时引发此异常，防止命令组之间出现循环依赖关系。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.UI.Exceptions
{
    /// <summary>
    /// Exception indicates that the parent group is set as a parent of itself
    /// 表示父命令组出现循环依赖（将自身设为父组）
    /// </summary>
    public class ParentGroupCircularDependencyException : Exception
    {
        public ParentGroupCircularDependencyException(string grpId) 
            : base($"Group cannot be a parent of itself '{grpId}'") 
        {
        }
    }
}
