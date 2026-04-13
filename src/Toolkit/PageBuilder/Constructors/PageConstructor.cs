//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using Xarial.XCad.Utils.PageBuilder.Base;

namespace Xarial.XCad.Utils.PageBuilder.Constructors
{
    /// <summary>
    /// Base constructor abstraction for page root object.
    /// <para>页面根对象的构造器抽象基类。</para>
    /// </summary>
    /// <typeparam name="TPage">Page type.<para>页面类型。</para></typeparam>
    public abstract class PageConstructor<TPage> : IPageConstructor<TPage>
        where TPage : IPage
    {
        /// <summary>
        /// Explicit interface implementation delegating to typed create method.
        /// <para>显式接口实现，委托到强类型创建方法。</para>
        /// </summary>
        TPage IPageConstructor<TPage>.Create(IAttributeSet atts) => Create(atts);

        /// <summary>
        /// Creates page instance from attributes.
        /// <para>根据特性集合创建页面实例。</para>
        /// </summary>
        protected abstract TPage Create(IAttributeSet atts);
    }
}