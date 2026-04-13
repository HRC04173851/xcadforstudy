//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using SolidWorks.Interop.sldworks;
using System;
using Xarial.XCad.Documents;

namespace Xarial.XCad.SolidWorks.Documents
{
    /// <summary>
    /// SolidWorks 装配体配置接口，支持装配体专属配置（如组件状态）。
    /// </summary>
    public interface ISwAssemblyConfiguration : ISwConfiguration, IXAssemblyConfiguration
    {
    }

    /// <summary>
    /// SolidWorks 装配体配置实现类，封装装配体配置下的组件列表。
    /// </summary>
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