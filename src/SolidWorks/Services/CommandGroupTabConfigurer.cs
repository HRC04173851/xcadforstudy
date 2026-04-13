//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.UI.Commands.Structures;

namespace Xarial.XCad.SolidWorks.Services
{
    /// <summary>
    /// Configuration information of the tab
    /// <para>中文：命令组页签（Ribbon Tab）的配置数据</para>
    /// </summary>
    public class CommandGroupTabConfiguration 
    {
        /// <summary>
        /// Name of the tab
        /// </summary>
        public string TabName { get; set; }

        /// <summary>
        /// True if the tab should be created, False if not
        /// </summary>
        public bool Include { get; set; }
    }

    /// <summary>
    /// Service to configure tabs
    /// <para>中文：命令组页签配置服务接口</para>
    /// </summary>
    public interface ICommandGroupTabConfigurer
    {
        /// <summary>
        /// Called when command tab is created
        /// </summary>
        /// <param name="cmdGrpSpec">Specification of the group</param>
        /// <param name="config">Configuration of the tab</param>
        void ConfigureTab(CommandGroupSpec cmdGrpSpec, CommandGroupTabConfiguration config);
    }

    /// <summary>
    /// Default tab configuration
    /// <para>中文：默认页签配置实现（占位实现，不修改默认行为）</para>
    /// </summary>
    /// <remarks>This configurer uses the default options and acts as a placeholder.
    /// User can register custom <see cref="ICommandGroupTabConfigurer"/> service to configure the behavior of tabs</remarks>
    internal class DefaultCommandGroupTabConfigurer : ICommandGroupTabConfigurer
    {
        public void ConfigureTab(CommandGroupSpec cmdGrpSpec, CommandGroupTabConfiguration config)
        {
        }
    }
}
