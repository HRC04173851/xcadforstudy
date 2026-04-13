//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.Toolkit.Exceptions
{
    /// <summary>
    /// Exception indicates that the requested service type is not registered in the service container.
    /// <para>异常指示请求的服务类型尚未在服务容器中注册。</para>
    /// </summary>
    public class ServiceNotRegisteredException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceNotRegisteredException"/> class.
        /// <para>初始化 <see cref="ServiceNotRegisteredException"/> 类的新实例。</para>
        /// </summary>
        /// <param name="serviceType">Service type.<para>未注册的服务类型。</para></param>
        public ServiceNotRegisteredException(Type serviceType) : base($"Service '{serviceType.FullName}' is not registered")
        { 
        }
    }
}
