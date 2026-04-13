//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.UI.Exceptions
{
    /// <summary>
    /// Exception indicates that control of custom pane accessed when control has not been created yet
    /// 表示在自定义面板控件尚未创建时访问控件引发的异常
    /// </summary>
    public class CustomPanelControlNotCreatedException : Exception
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public CustomPanelControlNotCreatedException()
            : base($"Control is not created for this custom panel. Use {nameof(IXCustomPanel<object>.ControlCreated)} event to handle control creation")
        {
        }
    }
}
