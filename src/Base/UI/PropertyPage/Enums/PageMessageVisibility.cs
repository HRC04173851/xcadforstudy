// -*- coding: utf-8 -*-
// src/Base/UI/PropertyPage/Enums/PageMessageVisibility.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义属性页消息的可见性模式，包括隐藏、正常可见和重要提示等状态
//*********************************************************************

namespace Xarial.XCad.UI.PropertyPage.Enums
{
    /// <summary>
    /// Visibility mode of page message
    /// 页面消息可见性模式
    /// </summary>
    public enum PageMessageVisibility
    {
        None = 1,
        Hidden,
        Visible,
        Important
    }
}