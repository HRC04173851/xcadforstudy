// -*- coding: utf-8 -*-
// samples/SwAddIn/ComUserControl.cs

//*********************************************************************
//xCAD
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xerial.com/license/
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

namespace SwAddInExample
{
    /// <summary>
    /// COM-visible UserControl for hosting within SolidWorks PropertyManager Page.
    /// Marked [ComVisible(true)] to allow SolidWorks to host this control via COM interop.
    /// 中文：用于托管在 SolidWorks 属性管理器页面中的 COM 可视化 UserControl。
    /// 中文：标记为 [ComVisible(true)] 以允许 SolidWorks 通过 COM 互操作托管此控件。
    /// </summary>
    [ComVisible(true)]
    public partial class ComUserControl : UserControl
    {
        /// <summary>
        /// Default constructor initializes the Windows Forms designer-generated components.
        /// 中文：默认构造函数，初始化 Windows 窗体设计器生成的组件。
        /// </summary>
        public ComUserControl()
        {
            InitializeComponent();
        }
    }
}
