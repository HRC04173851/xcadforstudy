//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

namespace Xarial.XCad.Documents
{
    /// <summary>
    /// Represents the configuration of the assembly
    /// 表示装配体配置
    /// </summary>
    public interface IXAssemblyConfiguration : IXConfiguration 
    {
        /// <summary>
        /// Components in this assembly configuration
        /// 此装配配置中的组件集合
        /// </summary>
        IXComponentRepository Components { get; }
    }
}