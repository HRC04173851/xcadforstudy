//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using Xarial.XCad.UI.PropertyPage.Base;
using Xarial.XCad.UI.PropertyPage.Enums;
using Xarial.XCad.Utils.PageBuilder.Base;

namespace Xarial.XCad.Utils.PageBuilder.Constructors
{
    /// <summary>
    /// Base constructor for non-group PropertyManager controls.
    /// <para>用于非分组 PropertyManager 控件的构造器基类。</para>
    /// </summary>
    /// <typeparam name="TControl">Control type.<para>控件类型。</para></typeparam>
    /// <typeparam name="TGroup">Parent group type.<para>父分组类型。</para></typeparam>
    /// <typeparam name="TPage">Page type.<para>页面类型。</para></typeparam>
    public abstract class ControlConstructor<TControl, TGroup, TPage> : PageElementConstructor<TControl, TGroup, TPage>
        where TControl : IControl
        where TPage : IPage
        where TGroup : IGroup
    {
    }
}