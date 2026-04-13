//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using Xarial.XCad.UI.PropertyPage.Base;
using Xarial.XCad.Utils.PageBuilder.Base;

namespace Xarial.XCad.Utils.PageBuilder.Core
{
    /// <summary>
    /// Base implementation of control-data binding for PropertyManager page.
    /// <para>PropertyManager 页面控件与数据模型绑定的基类实现。</para>
    /// </summary>
    /// <typeparam name="TDataModel">Data model type.<para>数据模型类型。</para></typeparam>
    public abstract class Binding<TDataModel> : IBinding
    {
        /// <summary>
        /// Raised when binding value changes.
        /// <para>绑定值变化时触发。</para>
        /// </summary>
        public event Action<IBinding> Changed;
        /// <summary>
        /// Raised after control receives data from model.
        /// <para>控件从模型接收数据后触发。</para>
        /// </summary>
        public event Action<IBinding> ControlUpdated;
        /// <summary>
        /// Raised after model receives value from control.
        /// <para>模型从控件接收数据后触发。</para>
        /// </summary>
        public event Action<IBinding> ModelUpdated;
        
        public IControl Control { get; }

        public abstract IMetadata[] Metadata { get; }

        public bool Silent { get; }

        /// <summary>
        /// Initializes binding with control and update mode.
        /// <para>使用控件与更新模式初始化绑定。</para>
        /// </summary>
        public Binding(IControl control, bool silent)
        {
            Control = control;
            Control.ValueChanged += OnControlValueChanged;
            Silent = silent;
        }

        /// <summary>
        /// Pushes value from model to control.
        /// <para>将模型值推送到控件。</para>
        /// </summary>
        public void UpdateControl()
        {
            Control.Update();
            SetUserControlValue();
            ControlUpdated?.Invoke(this);
        }

        /// <summary>
        /// Pulls value from control to model.
        /// <para>将控件值回写到模型。</para>
        /// </summary>
        public void UpdateDataModel()
        {
            SetDataModelValue(Control.GetValue());
            ModelUpdated?.Invoke(this);
        }

        protected void RaiseChangedEvent() 
            => Changed?.Invoke(this);

        protected abstract void SetDataModelValue(object value);

        protected abstract void SetUserControlValue();

        private void OnControlValueChanged(IControl sender, object newValue)
        {
            if (!(sender is IGroup))
            {
                SetDataModelValue(newValue);
            }

            ModelUpdated?.Invoke(this);
            RaiseChangedEvent();
        }
    }
}