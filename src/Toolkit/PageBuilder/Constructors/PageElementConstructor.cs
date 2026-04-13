//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using Xarial.XCad.UI.PropertyPage.Base;
using Xarial.XCad.Utils.PageBuilder.Base;
using Xarial.XCad.Utils.PageBuilder.PageElements;

namespace Xarial.XCad.Utils.PageBuilder.Constructors
{
    /// <summary>
    /// Base constructor abstraction for page elements.
    /// <para>页面元素构造器的抽象基类。</para>
    /// </summary>
    /// <typeparam name="TElem">Element control type.<para>元素控件类型。</para></typeparam>
    /// <typeparam name="TGroup">Parent group type.<para>父分组类型。</para></typeparam>
    /// <typeparam name="TPage">Page type.<para>页面类型。</para></typeparam>
    public abstract class PageElementConstructor<TElem, TGroup, TPage> : IPageElementConstructor
            where TGroup : IGroup
            where TPage : IPage
            where TElem : IControl
    {
        /// <summary>
        /// Explicit interface implementation delegating to typed create method.
        /// <para>显式接口实现，委托到强类型创建方法。</para>
        /// </summary>
        IControl IPageElementConstructor.Create(IGroup parentGroup, IAttributeSet atts, IMetadata[] metadata, ref int idRange) => Create(parentGroup, atts, metadata, ref idRange);

        /// <summary>
        /// Creates page element instance.
        /// <para>创建页面元素实例。</para>
        /// </summary>
        protected abstract TElem Create(IGroup parentGroup, IAttributeSet atts, IMetadata[] metadata, ref int numberOfUsedIds);
    }
}