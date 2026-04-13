//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using Xarial.XCad.UI.PropertyPage.Base;
using Xarial.XCad.UI.PropertyPage.Services;

namespace Xarial.XCad.Utils.PageBuilder.Base
{
    /// <summary>
    /// Stores raw dependency registrations before runtime dependency manager initialization.
    /// <para>在运行时依赖管理器初始化前，存储原始依赖注册信息。</para>
    /// </summary>
    public interface IRawDependencyGroup
    {
        /// <summary>
        /// Mapping from tag to binding.
        /// <para>从标签到绑定对象的映射。</para>
        /// </summary>
        IReadOnlyDictionary<object, IBinding> TaggedBindings { get; }
        /// <summary>
        /// Binding dependency definitions by tag references.
        /// <para>按标签引用定义的绑定依赖关系。</para>
        /// </summary>
        IReadOnlyDictionary<IBinding, Tuple<object[], IDependencyHandler>> DependenciesTags { get; }
        /// <summary>
        /// Metadata dependency handlers per control.
        /// <para>每个控件的元数据依赖处理器。</para>
        /// </summary>
        IReadOnlyDictionary<IControl, Tuple<IMetadata[], IMetadataDependencyHandler>> MetadataDependencies { get; }

        /// <summary>
        /// Registers mapping between binding and tag.
        /// <para>注册绑定与标签之间的映射。</para>
        /// </summary>
        void RegisterBindingTag(IBinding binding, object tag);

        /// <summary>
        /// Registers dependency of binding on other tagged bindings.
        /// <para>注册绑定对其它标签绑定的依赖关系。</para>
        /// </summary>
        void RegisterDependency(IBinding binding, object[] dependentOnTags, IDependencyHandler dependencyHandler);

        /// <summary>
        /// Registers metadata dependency for a control.
        /// <para>为控件注册元数据依赖关系。</para>
        /// </summary>
        void RegisterMetadataDependency(IControl ctrl, IMetadata[] metadata, IMetadataDependencyHandler dependencyHandler);
    }
}