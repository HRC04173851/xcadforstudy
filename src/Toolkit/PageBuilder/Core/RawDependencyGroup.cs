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
using Xarial.XCad.Utils.PageBuilder.Base;

namespace Xarial.XCad.Utils.PageBuilder.Core
{
    /// <summary>
    /// Stores raw binding and metadata dependency registrations.
    /// <para>存储原始绑定依赖与元数据依赖的注册信息。</para>
    /// </summary>
    public class RawDependencyGroup : IRawDependencyGroup
    {
        /// <summary>
        /// Gets tag-to-binding map.
        /// <para>获取标签到绑定对象的映射。</para>
        /// </summary>
        public IReadOnlyDictionary<object, IBinding> TaggedBindings => m_TaggedBindings;
        /// <summary>
        /// Gets binding dependency definitions keyed by binding.
        /// <para>获取以绑定为键的依赖定义集合。</para>
        /// </summary>
        public IReadOnlyDictionary<IBinding, Tuple<object[], IDependencyHandler>> DependenciesTags => m_DependenciesTags;
        /// <summary>
        /// Gets metadata dependency definitions keyed by control.
        /// <para>获取以控件为键的元数据依赖定义集合。</para>
        /// </summary>
        public IReadOnlyDictionary<IControl, Tuple<IMetadata[], IMetadataDependencyHandler>> MetadataDependencies => m_MetadataDependencies;

        private readonly Dictionary<object, IBinding> m_TaggedBindings;
        private readonly Dictionary<IBinding, Tuple<object[], IDependencyHandler>> m_DependenciesTags;
        private readonly Dictionary<IControl, Tuple<IMetadata[], IMetadataDependencyHandler>> m_MetadataDependencies;

        /// <summary>
        /// Initializes empty raw dependency group.
        /// <para>初始化空的原始依赖分组。</para>
        /// </summary>
        public RawDependencyGroup()
        {
            m_TaggedBindings = new Dictionary<object, IBinding>();
            m_DependenciesTags = new Dictionary<IBinding, Tuple<object[], IDependencyHandler>>();
            m_MetadataDependencies = new Dictionary<IControl, Tuple<IMetadata[], IMetadataDependencyHandler>>();
        }

        /// <summary>
        /// Registers unique tag for a binding.
        /// <para>为绑定注册唯一标签。</para>
        /// </summary>
        public void RegisterBindingTag(IBinding binding, object tag)
        {
            if (!TaggedBindings.ContainsKey(tag))
            {
                m_TaggedBindings.Add(tag, binding);
            }
            else
            {
                throw new Exception("Tag is not unique");
            }
        }

        /// <summary>
        /// Registers dependency handler for a binding.
        /// <para>为绑定注册依赖处理器。</para>
        /// </summary>
        public void RegisterDependency(IBinding binding, object[] dependentOnTags, IDependencyHandler dependencyHandler)
        {
            m_DependenciesTags.Add(binding, new Tuple<object[], IDependencyHandler>(dependentOnTags, dependencyHandler));
        }

        /// <summary>
        /// Registers metadata dependency handler for control.
        /// <para>为控件注册元数据依赖处理器。</para>
        /// </summary>
        public void RegisterMetadataDependency(IControl ctrl, IMetadata[] metadata, IMetadataDependencyHandler dependencyHandler)
        {
            m_MetadataDependencies.Add(ctrl, new Tuple<IMetadata[], IMetadataDependencyHandler>(metadata, dependencyHandler));
        }
    }
}