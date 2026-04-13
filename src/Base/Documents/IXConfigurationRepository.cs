//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using Xarial.XCad.Base;
using Xarial.XCad.Documents.Delegates;

namespace Xarial.XCad.Documents
{
    /// <summary>
    /// Represents the collection of configurations in <see cref="IXDocument3D"/>
    /// 表示 <see cref="IXDocument3D"/> 的配置集合
    /// </summary>
    public interface IXConfigurationRepository : IXRepository<IXConfiguration>
    {
        /// <summary>
        /// Fired when configuration is activated
        /// 配置被激活时触发
        /// </summary>
        event ConfigurationActivatedDelegate ConfigurationActivated;

        /// <summary>
        /// Returns the currently active configuration or activates the specific configuration
        /// 获取当前激活配置或切换激活到指定配置
        /// </summary>
        IXConfiguration Active { get; set; }
    }
}
