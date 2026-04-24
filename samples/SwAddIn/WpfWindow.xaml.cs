// -*- coding: utf-8 -*-
// samples/SwAddIn/WpfWindow.xaml.cs

//*********************************************************************
//xCAD
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SwAddInExample
{
    /// <summary>
    /// WPF popup window displayed via xCAD's popup window API.
    /// Can be docked, floated, or shown as a modal dialog.
    /// 中文：通过 xCAD 弹出窗口 API 显示的 WPF 窗口。可停靠、浮动或作为模态对话框显示。
    /// </summary>
    public partial class WpfWindow : Window
    {
        /// <summary>
        /// Tracks whether the user confirmed (true) or cancelled (false/null) the dialog.
        /// Set to true in OnOk, false in OnCancel, null initially.
        /// 中文：跟踪用户是否确认（true）或取消（false/null）对话框。在 OnOk 中设为 true，在 OnCancel 中设为 false，初始为 null。
        /// </summary>
        public bool? IsOk { get; private set; }

        /// <summary>
        /// Constructor: initializes WPF components and sets IsOk to null (undetermined).
        /// 中文：构造函数：初始化 WPF 组件并将 IsOk 设为 null（未确定）。
        /// </summary>
        public WpfWindow()
        {
            InitializeComponent();
            IsOk = null;
        }

        /// <summary>
        /// Ok button click handler: sets IsOk to true and closes the window.
        /// 中文：确定按钮点击处理程序：将 IsOk 设为 true 并关闭窗口。
        /// </summary>
        private void OnOk(object sender, RoutedEventArgs e)
        {
            IsOk = true;
            this.Close();
        }

        /// <summary>
        /// Cancel button click handler: sets IsOk to false and closes the window.
        /// 中文：取消按钮点击处理程序：将 IsOk 设为 false 并关闭窗口。
        /// </summary>
        private void OnCancel(object sender, RoutedEventArgs e)
        {
            IsOk = false;
            this.Close();
        }
    }
}
