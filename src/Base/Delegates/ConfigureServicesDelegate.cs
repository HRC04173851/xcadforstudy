//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.Delegates
{
    /// <summary>
    /// Delegate for <see cref="IXServiceConsumer.ConfigureServices"/> event
    /// <see cref="IXServiceConsumer.ConfigureServices"/> 事件的委托类型
    /// </summary>
    /// <param name="sender">Sender of this event 事件发送方</param>
    /// <param name="collection">Collection of services to configure 要配置的服务集合</param>
    public delegate void ConfigureServicesDelegate(IXServiceConsumer sender, IXServiceCollection collection);
}
