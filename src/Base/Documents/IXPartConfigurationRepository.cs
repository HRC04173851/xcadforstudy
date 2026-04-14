// -*- coding: utf-8 -*-
// src/Base/Documents/IXPartConfigurationRepository.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义零件配置仓储接口，继承自配置仓储接口，
// 提供零件配置的创建和激活功能。
//*********************************************************************

using Xarial.XCad.Base;

namespace Xarial.XCad.Documents
{
    /// <summary>
    /// Represents the collection of configurations in <see cref="IXPart"/>
    /// 表示 <see cref="IXPart"/> 的配置集合
    /// </summary>
    public interface IXPartConfigurationRepository : IXConfigurationRepository
    {
        /// <inheritdoc/>
        new IXPartConfiguration Active { get; set; }

        /// <inheritdoc/>
        new IXPartConfiguration PreCreate();
    }
}
