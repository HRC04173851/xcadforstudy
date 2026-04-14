// -*- coding: utf-8 -*-
// src/Base/UI/PropertyPage/Enums/PageOptions_e.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义属性页的整体行为选项标志，包括确定取消按钮、锁定、钉住、预览等功能
//*********************************************************************

using System;

namespace Xarial.XCad.UI.PropertyPage.Enums
{
    /// <summary>
    /// Property page behavior/options flags
    /// 属性页行为与功能选项标志
    /// </summary>
    [Flags]
    public enum PageOptions_e
    {
        OkayButton = 1,
        CancelButton = 2,
        LockedPage = 4,
        CloseDialogButton = 8,
        MultiplePages = 16,
        PushpinButton = 32,
        PreviewButton = 128,
        DisableSelection = 256,
        AbortCommands = 1024,
        UndoButton = 2048,
        CanEscapeCancel = 4096,
        HandleKeystrokes = 8192,
        RedoButton = 16384,
        DisablePageBuildDuringHandlers = 32768,
        GrayOutDisabledSelectionListboxes = 65536,
        SupportsChainSelection = 131072,
        SupportsIsolate = 262144
    }
}