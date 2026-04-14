// -*- coding: utf-8 -*-
// src/SolidWorks/Documents/SwAssemblyConfiguration.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件实现 SolidWorks 装配体配置（Configuration）的封装。
// 装配体配置用于定义装配体在不同工况下的不同状态，
// 包括组件的压缩状态、配置特定的配合关系等。配置还管理组件的引用集合。
//*********************************************************************

using SolidWorks.Interop.sldworks;
using System;
using Xarial.XCad.Documents;

namespace Xarial.XCad.SolidWorks.Documents
{
    public interface ISwAssemblyConfiguration : ISwConfiguration, IXAssemblyConfiguration
    {
    }

    internal class SwAssemblyConfiguration : SwConfiguration, ISwAssemblyConfiguration
    {
        internal SwAssemblyConfiguration(IConfiguration conf, SwAssembly assm, SwApplication app, bool created) 
            : base(conf, assm, app, created)
        {
            Components = new SwAssemblyComponentCollection(assm, conf);
        }

        public IXComponentRepository Components { get; }
    }
}