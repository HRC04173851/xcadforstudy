// -*- coding: utf-8 -*-
// src/Base/Documents/IXAssemblyConfigurationRepository.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义装配体配置仓储接口，继承自配置仓储接口，
// 提供装配体配置的创建和激活功能。
//*********************************************************************

using Xarial.XCad.Base;

namespace Xarial.XCad.Documents
{
    /// <summary>
    /// Represents the collection of configurations in <see cref="IXAssembly"/>
    /// 表示 <see cref="IXAssembly"/> 的配置集合
    /// </summary>
    public interface IXAssemblyConfigurationRepository : IXConfigurationRepository
    {
        /// <inheritdoc/>
        new IXAssemblyConfiguration Active { get; set; }

        /// <inheritdoc/>
        new IXAssemblyConfiguration PreCreate();
    }
}
