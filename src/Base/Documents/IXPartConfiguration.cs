// -*- coding: utf-8 -*-
// src/Base/Documents/IXPartConfiguration.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义零件配置接口，表示零件文档的配置信息，
// 提供切割清单项目集合和材料属性的访问功能。
//*********************************************************************

using Xarial.XCad.Features;
using System.Collections.Generic;

namespace Xarial.XCad.Documents
{
    /// <summary>
    /// Represents the configuration of <see cref="IXPart"/>
    /// 表示 <see cref="IXPart"/> 的配置
    /// </summary>
    public interface IXPartConfiguration : IXConfiguration
    {
        /// <summary>
        /// Cut-list items in this configuration (if available)
        /// 此配置中的切割清单项目（若可用）
        /// </summary>
        IXCutListItemRepository CutLists { get; }

        /// <summary>
        /// Material of this part
        /// 此配置对应零件的材料
        /// </summary>
        IXMaterial Material { get; set; }
    }
}