// -*- coding: utf-8 -*-
// samples/SwAddIn/WinUserControl.cs

//*********************************************************************
//xCAD
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Xarial.XCad.UI.PropertyPage;

namespace SwAddInExample
{
    /// <summary>
    /// Windows Forms UserControl used as an alternative custom control inside the PropertyManager Page (PMP).
    /// To use this instead of the WPF control, change [CustomControl(typeof(WpfUserControl))]
    /// to [CustomControl(typeof(WinUserControl))] in PmpData.cs.
    /// 中文：用作属性管理器页面（PMP）备选自定义控件的 Windows 窗体用户控件。
    /// 中文：若要用此控件替代 WPF 控件，在 PmpData.cs 中将
    /// 中文：[CustomControl(typeof(WpfUserControl))] 改为 [CustomControl(typeof(WinUserControl))]。
    /// </summary>
    public partial class WinUserControl : UserControl, IXCustomControl
    {
        /// <summary>
        /// Constructor: initializes the Windows Forms designer-generated components.
        /// 中文：构造函数：初始化 Windows 窗体设计器生成的组件。
        /// </summary>
        public WinUserControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// The current value of the custom control, synchronized with the PMP data model.
        /// 中文：自定义控件的当前值，与属性管理器页面数据模型同步。
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Event fired when the control's value changes, allowing the PMP to update its data model.
        /// 中文：控件值更改时触发的事件，使属性管理器页面能够更新其数据模型。
        /// </summary>
        public event CustomControlValueChangedDelegate ValueChanged;
    }
}
