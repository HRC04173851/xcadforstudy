// -*- coding: utf-8 -*-
// src/Base/Features/CustomFeature/Attributes/MissingDefinitionErrorMessage.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 当自定义特征定义未注册时，提供用户友好的错误消息特性。
// 可用于显示扩展下载页面或联系邮箱等信息。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.Features.CustomFeature.Attributes
{
    /// <summary>
    /// Use this attribute to provide a user friendly message when definition of custom feature is not registered
    /// 当自定义特征定义未注册时，使用此特性提供用户友好错误信息
    /// </summary>
    /// <remarks>The message might point to the download page of your extension or list a contact e-mail</remarks>
    public class MissingDefinitionErrorMessage : Attribute
    {
        /// <summary>
        /// Missing custom feature definition error message
        /// 自定义特征定义缺失时显示的错误消息
        /// </summary>
        public string Message { get; }

        ///<summary>Constructor to specify option</summary>
        /// <param name="msg">Default message to display when custom feature cannot be loaded
        /// The provided text is displayed in the What's Wrong dialog of</param>
        public MissingDefinitionErrorMessage(string msg)
        {
            Message = msg;
        }
    }
}
