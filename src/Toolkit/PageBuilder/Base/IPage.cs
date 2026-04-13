//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

namespace Xarial.XCad.Utils.PageBuilder.Base
{
    /// <summary>
    /// Represents a PropertyManager page root group.
    /// <para>表示 PropertyManager 页面根分组。</para>
    /// </summary>
    public interface IPage : IGroup
    {
        /// <summary>
        /// Gets binding manager for controls on the page.
        /// <para>获取页面控件的数据绑定管理器。</para>
        /// </summary>
        IBindingManager Binding { get; }
    }
}