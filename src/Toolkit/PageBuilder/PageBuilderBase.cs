//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using Xarial.XCad.UI.PropertyPage.Base;
using Xarial.XCad.UI.PropertyPage.Delegates;
using Xarial.XCad.Utils.PageBuilder.Base;
using Xarial.XCad.Utils.PageBuilder.Internal;

namespace Xarial.XCad.Utils.PageBuilder
{
    /// <summary>
    /// Provides current data context for page bindings and metadata.
    /// <para>为页面绑定与元数据提供当前数据上下文。</para>
    /// </summary>
    public interface IContextProvider 
    {
        /// <summary>
        /// Raised when current context changes.
        /// <para>当前上下文变化时触发。</para>
        /// </summary>
        event Action<IContextProvider, object> ContextChanged;
        /// <summary>
        /// Notifies listeners about context change.
        /// <para>通知侦听器上下文已变化。</para>
        /// </summary>
        void NotifyContextChanged(object context);
    }

    /// <summary>
    /// Basic implementation of context provider.
    /// <para>上下文提供器的基础实现。</para>
    /// </summary>
    public class BaseContextProvider : IContextProvider
    {
        /// <summary>
        /// Raised when current context changes.
        /// <para>当前上下文变化时触发。</para>
        /// </summary>
        public event Action<IContextProvider, object> ContextChanged;

        /// <summary>
        /// Broadcasts new context to subscribers.
        /// <para>向订阅者广播新的上下文对象。</para>
        /// </summary>
        public void NotifyContextChanged(object context)
        {
            ContextChanged?.Invoke(this, context);
        }
    }

    /// <summary>
    /// Base page builder that creates page hierarchy, bindings, and dependencies.
    /// <para>用于创建页面层级、数据绑定与依赖关系的页面构建器基类。</para>
    /// </summary>
    /// <typeparam name="TPage">Page type.<para>页面类型。</para></typeparam>
    /// <typeparam name="TGroup">Group type.<para>分组类型。</para></typeparam>
    /// <typeparam name="TControl">Control type.<para>控件类型。</para></typeparam>
    public class PageBuilderBase<TPage, TGroup, TControl>
        where TPage : IPage
        where TGroup : IGroup
        where TControl : IControl
    {
        private readonly IXApplication m_App;

        private readonly IDataModelBinder m_DataBinder;
        private readonly IPageConstructor<TPage> m_PageConstructor;

        private readonly ConstructorsContainer<TPage, TGroup> m_ControlConstructors;

        /// <summary>
        /// Initializes page builder with application, binder, page constructor, and element constructors.
        /// <para>使用应用程序、绑定器、页面构造器以及元素构造器初始化页面构建器。</para>
        /// </summary>
        public PageBuilderBase(IXApplication app, IDataModelBinder dataBinder,
            IPageConstructor<TPage> pageConstr,
            params IPageElementConstructor[] ctrlsContstrs)
        {
            m_App = app;

            m_DataBinder = dataBinder;
            m_PageConstructor = pageConstr;

            m_ControlConstructors = new ConstructorsContainer<TPage, TGroup>(ctrlsContstrs);
        }

        /// <summary>
        /// Creates page instance for specified model type.
        /// <para>为指定模型类型创建页面实例。</para>
        /// </summary>
        public virtual TPage CreatePage<TModel>(CreateDynamicControlsDelegate dynCtrlsHandler, IContextProvider modelProvider)
        {
            var page = default(TPage);

            m_DataBinder.Bind<TModel>(
                atts =>
                {
                    page = m_PageConstructor.Create(atts);
                    return page;
                },
                (Type type, IAttributeSet atts, IGroup parent, IMetadata[] metadata, out int numberOfUsedIds) =>
                {
                    numberOfUsedIds = 1;
                    return m_ControlConstructors.CreateElement(type, parent, atts, metadata, ref numberOfUsedIds);
                }, dynCtrlsHandler, modelProvider,
                    out IEnumerable<IBinding> bindings,
                    out IRawDependencyGroup dependencies,
                    out IMetadata[] allMetadata);

            page.Binding.Load(m_App, bindings, dependencies, allMetadata);
            UpdatePageDependenciesState(page);

            return page;
        }

        /// <summary>
        /// Updates dependency state after page construction.
        /// <para>在页面构建完成后更新依赖状态。</para>
        /// </summary>
        protected virtual void UpdatePageDependenciesState(TPage page)
        {
            page.Binding.Dependency.UpdateAll();
        }
    }
}