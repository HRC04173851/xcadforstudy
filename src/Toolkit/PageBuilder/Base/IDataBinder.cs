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

namespace Xarial.XCad.Utils.PageBuilder.Base
{
    /// <summary>
    /// Delegate to create control for a data member.
    /// <para>用于为数据成员创建控件的委托。</para>
    /// </summary>
    public delegate IControl CreateBindingControlDelegate(Type dataType, IAttributeSet atts,
        IGroup parent, IMetadata[] metadata, out int numberOfUsedIds);

    /// <summary>
    /// Delegate to create page root from attributes.
    /// <para>用于根据特性创建页面根对象的委托。</para>
    /// </summary>
    public delegate IPage CreateBindingPageDelegate(IAttributeSet atts);

    /// <summary>
    /// Creates bindings between data model and generated PropertyManager controls.
    /// <para>在数据模型与生成的 PropertyManager 控件之间创建绑定。</para>
    /// </summary>
    public interface IDataModelBinder
    {
        /// <summary>
        /// Builds page bindings and dependency metadata for specified data model type.
        /// <para>为指定数据模型类型构建页面绑定与依赖元数据。</para>
        /// </summary>
        void Bind<TDataModel>(CreateBindingPageDelegate pageCreator,
            CreateBindingControlDelegate ctrlCreator, CreateDynamicControlsDelegate dynCtrlDescCreator, IContextProvider modelSetter,
            out IEnumerable<IBinding> bindings, out IRawDependencyGroup dependencies, out IMetadata[] metadata);
    }
}