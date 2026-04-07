//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Xarial.XCad.Base;
using Xarial.XCad.Base.Enums;
using Xarial.XCad.Documents;
using Xarial.XCad.Documents.Delegates;
using Xarial.XCad.Geometry;
using Xarial.XCad.Geometry.Structures;
using Xarial.XCad.SolidWorks.Documents.Exceptions;
using Xarial.XCad.SolidWorks.Documents.Services;
using Xarial.XCad.SolidWorks.Features;
using Xarial.XCad.SolidWorks.Geometry;
using Xarial.XCad.SolidWorks.Utils;
using Xarial.XCad.Utils.Diagnostics;

namespace Xarial.XCad.SolidWorks.Documents
{
    /// <summary>
    /// Represents a SolidWorks assembly document.
    /// <para>中文：表示 SolidWorks 装配体文档的接口。</para>
    /// </summary>
    public interface ISwAssembly : ISwDocument3D, IXAssembly
    {
        /// <summary>
        /// Gets the underlying SolidWorks <see cref="IAssemblyDoc"/> COM object.
        /// <para>中文：获取底层 SolidWorks <see cref="IAssemblyDoc"/> COM 对象。</para>
        /// </summary>
        IAssemblyDoc Assembly { get; }

        /// <summary>
        /// Gets the component currently being edited in the assembly context.
        /// <para>中文：获取装配体上下文中当前正在编辑的装配体组件。</para>
        /// </summary>
        new ISwComponent EditingComponent { get; }

        /// <summary>
        /// Gets the assembly-specific configuration collection.
        /// <para>中文：获取装配体文档专用的配置集合。</para>
        /// </summary>
        new ISwAssemblyConfigurationCollection Configurations { get; }
    }

    /// <summary>
    /// SolidWorks assembly document implementation.
    /// <para>中文：SolidWorks 装配体文档的实现类。</para>
    /// </summary>
    internal class SwAssembly : SwDocument3D, ISwAssembly
    {
        /// <summary>
        /// Raised when a new component is inserted into the assembly.
        /// <para>中文：当新的装配体组件被插入装配体时触发。</para>
        /// </summary>
        public event ComponentInsertedDelegate ComponentInserted
        {
            add => m_ComponentInsertedEventsHandler.Attach(value);
            remove => m_ComponentInsertedEventsHandler.Detach(value);
        }

        /// <summary>
        /// Raised before a component is deleted from the assembly.
        /// <para>中文：在装配体组件从装配体中被删除之前触发。</para>
        /// </summary>
        public event ComponentDeletingDelegate ComponentDeleting
        {
            add => m_ComponentDeletingEventsHandler.Attach(value);
            remove => m_ComponentDeletingEventsHandler.Detach(value);
        }

        /// <summary>
        /// Raised after a component has been deleted from the assembly.
        /// <para>中文：在装配体组件从装配体中被删除之后触发。</para>
        /// </summary>
        public event ComponentDeletedDelegate ComponentDeleted
        {
            add => m_ComponentDeletedEventsHandler.Attach(value);
            remove => m_ComponentDeletedEventsHandler.Detach(value);
        }

        /// <summary>
        /// Gets the underlying SolidWorks assembly document COM object.
        /// <para>中文：获取底层 SolidWorks 装配体文档 COM 对象。</para>
        /// </summary>
        public IAssemblyDoc Assembly => Model as IAssemblyDoc;

        // Lazy-initialized assembly configuration collection
        // 中文：延迟初始化的装配体配置集合
        private readonly Lazy<SwAssemblyConfigurationCollection> m_LazyConfigurations;

        // Assembly-specific evaluation (mass properties, bounding box, etc.)
        // 中文：装配体专用评估对象（质量属性、包围盒等）
        private readonly SwAssemblyEvaluation m_Evaluation;

        // Event handlers for component lifecycle events
        // 中文：装配体组件生命周期事件处理器
        private readonly ComponentInsertedEventsHandler m_ComponentInsertedEventsHandler;
        private readonly ComponentDeletingEventsHandler m_ComponentDeletingEventsHandler;
        private readonly ComponentDeletedEventsHandler m_ComponentDeletedEventsHandler;

        /// <summary>
        /// Initializes a new instance of <see cref="SwAssembly"/>.
        /// <para>中文：初始化 <see cref="SwAssembly"/> 的新实例，注册组件事件处理器，并延迟初始化配置集合和评估对象。</para>
        /// </summary>
        internal SwAssembly(IAssemblyDoc assembly, SwApplication app, IXLogger logger, bool isCreated)
            : base((IModelDoc2)assembly, app, logger, isCreated)
        {
            // Register event handlers for component insert/delete lifecycle
            // 中文：注册装配体组件插入/删除生命周期的事件处理器
            m_ComponentInsertedEventsHandler = new ComponentInsertedEventsHandler(this, app);
            m_ComponentDeletingEventsHandler = new ComponentDeletingEventsHandler(this, app);
            m_ComponentDeletedEventsHandler = new ComponentDeletedEventsHandler(this, app, logger);

            // Lazily create the configuration collection to defer COM calls until needed
            // 中文：延迟创建配置集合，推迟 COM 调用直到实际需要时
            m_LazyConfigurations = new Lazy<SwAssemblyConfigurationCollection>(() => new SwAssemblyConfigurationCollection(this, app));
            m_Evaluation = new SwAssemblyEvaluation(this);
        }

        // Explicit interface implementations for configuration and component access
        // 中文：配置集合和组件访问的显式接口实现
        ISwAssemblyConfigurationCollection ISwAssembly.Configurations => m_LazyConfigurations.Value;
        IXAssemblyConfigurationRepository IXAssembly.Configurations => (this as ISwAssembly).Configurations;
        IXComponent IXAssembly.EditingComponent => EditingComponent;
        IXAssemblyEvaluation IXAssembly.Evaluation => m_Evaluation;

        /// <summary>
        /// Gets the SolidWorks document type identifier for an assembly document.
        /// <para>中文：获取装配体文档对应的 SolidWorks 文档类型标识符。</para>
        /// </summary>
        internal protected override swDocumentTypes_e? DocumentType => swDocumentTypes_e.swDocASSEMBLY;

        /// <summary>
        /// Returns true when the assembly is opened in view-only (LDR) mode, treated as rapid mode.
        /// <para>中文：当装配体以仅查看（大型设计审阅）模式打开时返回 true，视为快速模式。</para>
        /// </summary>
        protected override bool IsRapidMode => Model.IsOpenedViewOnly(); //TODO: when editing feature of LDR is available make this to be rapid mode
        // 中文：TODO：当大型设计审阅的特征编辑功能可用时，改为快速模式

        /// <summary>
        /// Returns true when any component in the assembly is in lightweight mode.
        /// <para>中文：当装配体中存在轻量化状态的组件时返回 true。</para>
        /// </summary>
        protected override bool IsLightweightMode => Assembly.GetLightWeightComponentCount() > 0;

        /// <summary>
        /// Gets the document evaluation object for the assembly.
        /// <para>中文：获取装配体文档的评估对象。</para>
        /// </summary>
        public override IXDocumentEvaluation Evaluation => m_Evaluation;

        /// <summary>
        /// Gets the component currently being edited in the assembly context.
        /// Returns null if editing the root assembly itself.
        /// <para>中文：获取当前在装配体上下文中正在编辑的装配体组件。
        /// 如果正在编辑根装配体本身则返回 null。</para>
        /// </summary>
        public ISwComponent EditingComponent 
        {
            get
            {
                var comp = Assembly.GetEditTargetComponent();

                // IsRoot() returns true when the edit target is the top-level assembly, not a sub-component
                // 中文：IsRoot() 为 true 表示编辑目标是顶层装配体，而非子组件
                if (comp != null && !comp.IsRoot())
                {
                    return this.CreateObjectFromDispatch<ISwComponent>(comp);
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Commits any pending cache changes for the assembly, including its active configuration's component collection.
        /// <para>中文：提交装配体的所有待处理缓存变更，包括活动配置的组件集合。</para>
        /// </summary>
        protected override void CommitCache(IModelDoc2 model, CancellationToken cancellationToken)
        {
            base.CommitCache(model, cancellationToken);

            // Only commit configuration cache if it was already instantiated
            // 中文：仅在配置集合已实例化的情况下提交配置缓存
            if (m_LazyConfigurations.IsValueCreated) 
            {
                // Only commit component cache if the active configuration was lazily loaded
                // 中文：仅在活动配置已延迟加载时提交组件集合的缓存
                if (m_LazyConfigurations.Value.ActiveNonCommittedConfigurationLazy.IsValueCreated) 
                {
                    ((SwComponentCollection)((SwAssemblyConfiguration)m_LazyConfigurations.Value.ActiveNonCommittedConfigurationLazy.Value).Components)
                        .CommitCache(cancellationToken);
                }
            }
        }

        /// <summary>
        /// Creates the annotation collection for this assembly document.
        /// <para>中文：为装配体文档创建注解集合。</para>
        /// </summary>
        protected override SwAnnotationCollection CreateAnnotations()
            => new SwDocument3DAnnotationCollection(this);

        /// <summary>
        /// Creates the assembly-specific configuration collection.
        /// <para>中文：创建装配体文档专用的配置集合。</para>
        /// </summary>
        protected override SwConfigurationCollection CreateConfigurations()
            => new SwAssemblyConfigurationCollection(this, OwnerApplication);

        /// <summary>
        /// Returns true only if the given document type is an assembly document.
        /// <para>中文：仅当给定文档类型为装配体文档时返回 true。</para>
        /// </summary>
        protected override bool IsDocumentTypeCompatible(swDocumentTypes_e docType) => docType == swDocumentTypes_e.swDocASSEMBLY;
    }

    /// <summary>
    /// Collection of top-level components in a SolidWorks assembly for a specific configuration.
    /// <para>中文：特定配置下 SolidWorks 装配体顶层装配体组件的集合。</para>
    /// </summary>
    internal class SwAssemblyComponentCollection : SwComponentCollection
    {
        // Reference to the owning assembly document
        // 中文：对所属装配体文档的引用
        private readonly SwAssembly m_Assm;

        // The SolidWorks configuration this collection is scoped to
        // 中文：此集合所属的 SolidWorks 配置
        private readonly IConfiguration m_Conf;

        /// <summary>
        /// Initializes a new instance bound to the given assembly and configuration.
        /// <para>中文：初始化绑定到指定装配体和配置的新实例。</para>
        /// </summary>
        public SwAssemblyComponentCollection(SwAssembly assm, IConfiguration conf) : base(assm)
        {
            m_Assm = assm;
            m_Conf = conf;
        }

        /// <summary>
        /// Returns true when the bound configuration is the currently active configuration.
        /// <para>中文：当绑定的配置是当前活动配置时返回 true。</para>
        /// </summary>
        protected bool IsActiveConfiguration => m_Assm.Model.GetActiveConfiguration() == m_Conf;

        /// <summary>
        /// Attempts to find a component by name. For non-active configurations, searches
        /// within the configuration-specific component tree.
        /// <para>中文：尝试按名称查找装配体组件。对于非活动配置，在配置专用的组件树中搜索。</para>
        /// </summary>
        protected override bool TryGetByName(string name, out IXComponent ent)
        {
            var comp = RootAssembly.Assembly.GetComponentByName(name);

            if (comp != null)
            {
                if (!IsActiveConfiguration)
                {
                    // For non-active configurations, get the root component of this configuration
                    // 中文：对于非活动配置，获取该配置的根组件
                    var rootComp = m_Conf.GetRootComponent3(true);

                    var compId = comp.GetID();

                    comp = null;

                    //finding the correspodning configuration specific component
                    // 中文：查找与该配置对应的特定配置组件

                    foreach (var corrComp in (rootComp.GetChildren() as object[] ?? new object[0]).Cast<Component2>())
                    {
                        // Match by component ID to find the configuration-specific instance
                        // 中文：通过组件 ID 匹配，找到配置专用的实例
                        if (corrComp.GetID() == compId) 
                        {
                            comp = corrComp;
                            break;
                        }
                    }
                }
            }

            if (comp != null)
            {
                ent = RootAssembly.CreateObjectFromDispatch<SwComponent>(comp);
                return true;
            }
            else
            {
                ent = null;
                return false;
            }
        }

        /// <summary>
        /// Iterates over the direct child components, returning them in feature-tree order.
        /// Throws if the configuration is a SpeedPak simplified configuration.
        /// <para>中文：按特征树顺序迭代直接子装配体组件。若配置为 SpeedPak简化配置则抛出异常。</para>
        /// </summary>
        protected override IEnumerable<IComponent2> IterateChildren()
        {
            ValidateSpeedPak();

            // Use ordered collection to respect feature manager tree ordering
            // 中文：使用有序集合以遵循特征管理器树的排列顺序
            return new OrderedComponentsCollection(
                    () => (m_Conf.GetRootComponent3(!IsActiveConfiguration).GetChildren() as object[] ?? new object[0]).Cast<IComponent2>().ToArray(),
                    m_Assm.Model.IFirstFeature(),
                    m_Assm.OwnerApplication.Logger);
        }

        /// <summary>
        /// Gets the total number of components (including sub-assemblies' children) in the assembly.
        /// <para>中文：获取装配体中组件的总数（包含子装配体的子级）。</para>
        /// </summary>
        protected override int GetTotalChildrenCount()
        {
            ValidateSpeedPak();
            // false = include all levels (not top-level only)
            // 中文：false 表示包含所有层级（非仅顶层）
            return m_Assm.Assembly.GetComponentCount(false);
        }

        /// <summary>
        /// Gets the count of direct (top-level) child components in the assembly.
        /// <para>中文：获取装配体直接子级（顶层）装配体组件的数量。</para>
        /// </summary>
        protected override int GetChildrenCount()
        {
            ValidateSpeedPak();
            // true = top-level only
            // 中文：true 表示仅统计顶层组件
            return m_Assm.Assembly.GetComponentCount(true);
        }

        /// <summary>
        /// Validates that the current configuration is not a SpeedPak simplified configuration,
        /// which does not expose component data.
        /// <para>中文：验证当前配置不是 SpeedPak简化配置——SpeedPak 配置不公开组件数据。</para>
        /// </summary>
        private void ValidateSpeedPak() 
        {
            if (m_Conf.IsSpeedPak())
            {
                throw new SpeedPakConfigurationComponentsException();
            }
        }
    }
}