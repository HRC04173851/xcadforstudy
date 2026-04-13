//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Diagnostics;
using System.Linq;
using Xarial.XCad.UI.PropertyPage.Base;
using Xarial.XCad.Utils.Reflection;

namespace Xarial.XCad.Utils.PageBuilder.PageElements
{
    /// <summary>
    /// Delegate for strongly-typed control value change notification.
    /// <para>用于强类型控件值变化通知的委托。</para>
    /// </summary>
    public delegate void ControlValueChangedDelegate<TVal>(Control<TVal> sender, TVal newValue);

    /// <summary>
    /// Generic base implementation of PropertyManager control wrapper.
    /// <para>PropertyManager 控件包装器的泛型基类实现。</para>
    /// </summary>
    /// <typeparam name="TVal">Control value type.<para>控件值类型。</para></typeparam>
    public abstract class Control<TVal> : IControl
    {
        event ControlObjectValueChangedDelegate IControl.ValueChanged
        {
            add
            {
                this.ValueChanged += OnValueChanged;
                m_ValueChangedHandler += value;
            }
            remove
            {
                this.ValueChanged -= OnValueChanged;
                m_ValueChangedHandler -= value;
            }
        }

        object IControl.GetValue() => GetSpecificValue();

        protected abstract event ControlValueChangedDelegate<TVal> ValueChanged;

        private ControlObjectValueChangedDelegate m_ValueChangedHandler;

        /// <summary>
        /// Control identifier.
        /// <para>控件标识符。</para>
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Arbitrary tag used by dependency logic.
        /// <para>由依赖逻辑使用的任意标签值。</para>
        /// </summary>
        public object Tag { get; private set; }
        
        public abstract bool Enabled { get; set; }
        public abstract bool Visible { get; set; }

        /// <summary>
        /// Metadata attached to control.
        /// <para>附加到控件的元数据。</para>
        /// </summary>
        public IMetadata[] Metadata { get; }

        /// <summary>
        /// Runtime value type of control.
        /// <para>控件值的运行时类型。</para>
        /// </summary>
        public virtual Type ValueType => typeof(TVal);

        /// <summary>
        /// Initializes control wrapper.
        /// <para>初始化控件包装对象。</para>
        /// </summary>
        protected Control(int id, object tag, IMetadata[] metadata)
        {
            Id = id;
            Tag = tag;
            Metadata = metadata;
        }

        /// <summary>
        /// Updates control state from backend source.
        /// <para>从后端数据源更新控件状态。</para>
        /// </summary>
        public virtual void Update() 
        {
        }

        public void Dispose() => Dispose(true);

        /// <summary>
        /// Sets control value from boxed object.
        /// <para>从装箱对象设置控件值。</para>
        /// </summary>
        public void SetValue(object value)
        {
            var destVal = (TVal)value.Cast(ValueType);

            SetSpecificValue(destVal);
        }

        /// <summary>
        /// Shows tooltip near control.
        /// <para>在控件附近显示提示信息。</para>
        /// </summary>
        public abstract void ShowTooltip(string title, string msg);

        /// <summary>
        /// Moves UI focus to control.
        /// <para>将界面焦点移动到控件。</para>
        /// </summary>
        public abstract void Focus();

        protected virtual void Dispose(bool disposing)
        {
        }

        protected abstract TVal GetSpecificValue();

        protected abstract void SetSpecificValue(TVal value);

        private void OnValueChanged(Control<TVal> sender, TVal newValue)
        {
            if (m_ValueChangedHandler != null)
            {
                m_ValueChangedHandler.Invoke(sender, newValue);
            }
            else
            {
                Debug.Assert(false, "Generic event handler and specific event handler should be synchronised");
            }
        }
    }
}