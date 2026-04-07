//*********************************************************************
//xCAD
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using SwAddInExample.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xarial.XCad.Base.Attributes;
using Xarial.XCad.UI.PropertyPage;

namespace SwAddInExample
{
    // WPF UserControl used as a custom control embedded inside the PropertyManager Page (PMP)
    // 中文：嵌入属性管理器页面（PMP）中作为自定义控件使用的 WPF 用户控件
    // Title and Icon attributes configure how this control is identified in the xCAD framework
    // 中文：Title 和 Icon 特性用于配置该控件在 xCAD 框架中的标题和图标
    [Title("WPF User Control")]
    [Icon(typeof(Resources), nameof(Properties.Resources.xarial))]
    public partial class WpfUserControl : UserControl, IDisposable, IXCustomControl
    {
        // IXCustomControl requires this event to notify the PMP when the control's value changes
        // 中文：IXCustomControl 要求此事件，以便在控件值更改时通知属性管理器页面
        public event CustomControlValueChangedDelegate ValueChanged;

        // The data context (ViewModel) for this WPF control
        // 中文：此 WPF 控件的数据上下文（视图模型）
        private CustomControlDataContext m_Context;

        // Constructor: initializes WPF components, sets up the data context, and subscribes to value change events
        // 中文：构造函数：初始化 WPF 组件，设置数据上下文，并订阅值变更事件
        public WpfUserControl()
        {
            InitializeComponent();
            m_Context = new CustomControlDataContext();
            // Subscribe so that when the ViewModel's value changes, the PMP is notified
            // 中文：订阅事件，当视图模型的值更改时通知属性管理器页面
            m_Context.ValueChanged += OnContextValueChanged;
            this.DataContext = m_Context;
        }

        // IXCustomControl.Value: the PMP reads/writes this property to synchronize data with the control
        // 中文：IXCustomControl.Value：属性管理器页面通过此属性与控件进行数据同步
        public object Value 
        {
            get => m_Context.Value;
            set => m_Context.Value = (OptsFlag)value;
        }

        // Forwards the internal ValueChanged event to the IXCustomControl.ValueChanged delegate
        // 中文：将内部 ValueChanged 事件转发给 IXCustomControl.ValueChanged 委托
        private void OnContextValueChanged(CustomControlDataContext sender, OptsFlag value)
        {
            ValueChanged?.Invoke(this, value);
        }

        // Dispose: cleanup resources; currently no unmanaged resources to release
        // 中文：Dispose：清理资源；当前没有需要释放的非托管资源
        public void Dispose()
        {
        }
    }
}
