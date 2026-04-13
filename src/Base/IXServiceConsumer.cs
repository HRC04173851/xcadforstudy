//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Delegates;

namespace Xarial.XCad
{
    /// <summary>
    /// Declares that this object consumes services
    /// 声明此对象使用服务
    /// </summary>
    public interface IXServiceConsumer
    {
        /// <summary>
        /// Event to configure services/>
        /// 配置服务的事件
        /// </summary>
        /// <remarks>This event may not be fired if ConfigureServices method is available is marked as virtual method and it is overridden in the derived class</remarks>
        event ConfigureServicesDelegate ConfigureServices;
    }
}
