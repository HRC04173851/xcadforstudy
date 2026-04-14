// -*- coding: utf-8 -*-
// src\Base\UI\Exceptions\ParentGroupNotFoundException.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 当指定的父命令组ID不存在时引发此异常，常见于命令组配置错误或组已被删除的情况。
//*********************************************************************

using System;

namespace Xarial.XCad.UI.Exceptions
{
    /// <summary>
    /// Exception indicates that the specified parent group does not exist
    /// 表示指定父命令组不存在
    /// </summary>
    public class ParentGroupNotFoundException : Exception
    {
        public ParentGroupNotFoundException(string parentGrpId, string thisGrpId)
            : base($"Failed to find the parent group '{parentGrpId}' for '{thisGrpId}'") 
        {
        }
    }
}
