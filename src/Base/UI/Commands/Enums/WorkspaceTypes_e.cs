// -*- coding: utf-8 -*-
// src/Base/UI/Commands/Enums/WorkspaceTypes_e.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 提供各类工作区类型的枚举，定义命令在无文档、零件、装配体等不同环境中的是否可用
//*********************************************************************

using System;

namespace Xarial.XCad.UI.Commands.Enums
{
    /// <summary>
    /// Provides the enumeration of various workspaces
    /// 提供各类工作区枚举
    /// </summary>
    [Flags]
    public enum WorkspaceTypes_e
    {
        /// <summary>
        /// Environment when no documents are loaded
        /// </summary>
        NoDocuments = 1,

        /// <summary>
        /// Part document
        /// </summary>
        Part = 2 << 0,

        /// <summary>
        /// Assembly document
        /// </summary>
        Assembly = 2 << 1,

        /// <summary>
        /// Drawing document
        /// </summary>
        Drawing = 2 << 2,

        /// <summary>
        /// Part document edited in the context of the assembly
        /// </summary>
        InContextPart = 2 << 3,

        /// <summary>
        /// All SOLIDWORKS documents
        /// </summary>
        AllDocuments = Part | Assembly | Drawing,

        /// <summary>
        /// All environments
        /// </summary>
        All = AllDocuments | NoDocuments
    }
}