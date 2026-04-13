//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System.Collections.Generic;
using Xarial.XCad.UI.PropertyPage.Base;
using Xarial.XCad.Utils.PageBuilder.Base;

namespace Xarial.XCad.Utils.PageBuilder.Core
{
    /// <summary>
    /// Default binding manager implementation.
    /// <para>默认的数据绑定管理器实现。</para>
    /// </summary>
    public class BindingManager : IBindingManager
    {
        /// <summary>
        /// Loaded bindings collection.
        /// <para>已加载的绑定集合。</para>
        /// </summary>
        public IEnumerable<IBinding> Bindings { get; set; }
        /// <summary>
        /// Dependency manager instance.
        /// <para>依赖管理器实例。</para>
        /// </summary>
        public IDependencyManager Dependency { get; set; }
        /// <summary>
        /// Page metadata collection.
        /// <para>页面元数据集合。</para>
        /// </summary>
        public IMetadata[] Metadata { get; set; }

        /// <summary>
        /// Loads binding graph and initializes dependencies.
        /// <para>加载绑定图并初始化依赖关系。</para>
        /// </summary>
        public void Load(IXApplication app, IEnumerable<IBinding> bindings,
            IRawDependencyGroup dependencies, IMetadata[] metadata)
        {
            Bindings = bindings;
            Dependency = new DependencyManager();
            Metadata = metadata;

            Dependency.Init(app, dependencies);
        }
    }
}