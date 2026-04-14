// -*- coding: utf-8 -*-
// src/Base/Documents/Enums/BomChildrenSolving_e.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 指定 BOM 中子件的显示方式，支持显示、隐藏或提升子件到上级层级。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.Documents.Enums
{
    /// <summary>
    /// Enumeration used in <see cref="IXConfiguration.BomChildrenSolving"/>
    /// 用于 <see cref="IXConfiguration.BomChildrenSolving"/> 的枚举
    /// </summary>
    public enum BomChildrenSolving_e
    {
        /// <summary>
        /// Show children of this configuration
        /// 显示该配置的子项
        /// </summary>
        Show,

        /// <summary>
        /// Hide children of this configuration in the BOM
        /// 在 BOM 中隐藏该配置的子项
        /// </summary>
        Hide,

        /// <summary>
        /// Promote children of this configuration (dissolve the assembly)
        /// 提升该配置子项到上级层级（装配体溶解）
        /// </summary>
        Promote
    }
}
