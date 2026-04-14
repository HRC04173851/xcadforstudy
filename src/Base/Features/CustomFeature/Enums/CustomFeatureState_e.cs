// -*- coding: utf-8 -*-
// src/Base/Features/CustomFeature/Enums/CustomFeatureState_e.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义在OnUpdateState回调中返回的特征状态标志枚举。
// 用于动态控制特征的删除、编辑、压缩、替换等行为限制。
//*********************************************************************

using System;

namespace Xarial.XCad.Features.CustomFeature.Enums
{
    /// <summary>
    /// State of the <see cref="IXCustomFeature"/> within the <see cref="IXCustomFeatureDefinition.OnUpdateState(IXApplication, Documents.IXDocument, IXCustomFeature)"/>
    /// <see cref="IXCustomFeature"/> 在状态更新回调中的状态标志
    /// </summary>
    [Flags]
    public enum CustomFeatureState_e
    {
        Default = 0,
        CannotBeDeleted = 1,
        NotEditable = 2,
        CannotBeSuppressed = 4,
        CannotBeReplaced = 8,
        EnableNote = 16,
        CannotBeRolledBack = 32
    }
}