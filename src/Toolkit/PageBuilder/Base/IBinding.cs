//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using Xarial.XCad.UI.PropertyPage.Base;

namespace Xarial.XCad.Utils.PageBuilder.Base
{
    /// <summary>
    /// Represents two-way binding between data model and PropertyManager control.
    /// <para>表示数据模型与 PropertyManager 控件之间的双向绑定。</para>
    /// </summary>
    public interface IBinding
    {
        /// <summary>
        /// Indicates silent update mode to suppress callbacks.
        /// <para>指示静默更新模式，用于抑制回调。</para>
        /// </summary>
        bool Silent { get; }
        /// <summary>
        /// Metadata attached to the binding.
        /// <para>绑定关联的元数据集合。</para>
        /// </summary>
        IMetadata[] Metadata { get; }
        /// <summary>
        /// Raised when binding value changes.
        /// <para>绑定值变化时触发。</para>
        /// </summary>
        event Action<IBinding> Changed;

        /// <summary>
        /// Raised after model is updated from control.
        /// <para>控件更新到模型后触发。</para>
        /// </summary>
        event Action<IBinding> ModelUpdated;
        /// <summary>
        /// Raised after control is updated from model.
        /// <para>模型更新到控件后触发。</para>
        /// </summary>
        event Action<IBinding> ControlUpdated;

        /// <summary>
        /// Bound UI control.
        /// <para>被绑定的界面控件。</para>
        /// </summary>
        IControl Control { get; }

        /// <summary>
        /// Pushes model value to UI control.
        /// <para>将模型值刷新到界面控件。</para>
        /// </summary>
        void UpdateControl();

        /// <summary>
        /// Pulls UI control value back to model.
        /// <para>将界面控件值回写到模型。</para>
        /// </summary>
        void UpdateDataModel();
    }
}