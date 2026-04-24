// -*- coding: utf-8 -*-
// samples/SwAddIn/WinForm.cs

//*********************************************************************
//xCAD
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SwAddInExample
{
    /// <summary>
    /// Windows Form used as a standalone dialog window within the SolidWorks add-in.
    /// Can be shown via xCAD's dialog creation API (e.g., app.CreateDialog<WinForm>()).
    /// 中文：用作 SolidWorks 插件内独立对话框窗口的 Windows 窗体。
    /// 中文：可通过 xCAD 的对话框创建 API（例如 app.CreateDialog<WinForm>()）显示此窗体。
    /// </summary>
    public partial class WinForm : Form
    {
        /// <summary>
        /// Constructor: initializes the Windows Forms designer-generated components.
        /// 中文：构造函数：初始化 Windows 窗体设计器生成的组件。
        /// </summary>
        public WinForm()
        {
            InitializeComponent();
        }
    }
}
