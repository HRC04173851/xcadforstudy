// -*- coding: utf-8 -*-
// src/Toolkit/ServiceProvider.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件实现内部服务提供器 ServiceProvider。
// 实现单例/瞬态生命周期管理，按类型解析服务实例。
// 提供强类型服务解析的扩展方法 IServiceProviderExtension。
//*********************************************************************

using System;
using System.Collections.Generic;
using Xarial.XCad.Toolkit.Exceptions;

namespace Xarial.XCad.Toolkit
{
    /// <summary>
    /// Internal service provider implementing singleton/transient lifetimes.
    /// <para>实现单例/瞬态生命周期的内部服务提供器。</para>
    /// </summary>
    internal class ServiceProvider : IServiceProvider, IDisposable
    {
        private interface IServiceCreator : IDisposable
        {
            object CreateService();
        }

        private class TransientService : IServiceCreator
        {
            private readonly Func<object> m_Factory;

            internal TransientService(Func<object> factory) 
            {
                m_Factory = factory;
            }

            public object CreateService() => m_Factory.Invoke();

            public void Dispose() 
            {
                //No disposing
            }
        }

        private class SingletonService : IServiceCreator
        {
            private readonly Lazy<object> m_FactoryLazy;

            internal SingletonService(Func<object> factory)
            {
                m_FactoryLazy = new Lazy<object>(factory);
            }

            public object CreateService() => m_FactoryLazy.Value;

            public void Dispose()
            {
                if (m_FactoryLazy.IsValueCreated) 
                {
                    if (m_FactoryLazy.Value is IDisposable) 
                    {
                        ((IDisposable)m_FactoryLazy.Value).Dispose();
                    }
                }
            }
        }

        private readonly Dictionary<Type, IServiceCreator> m_Services;
        private bool m_IsDisposed;

        /// <summary>
        /// Initializes provider from service registrations.
        /// <para>根据服务注册信息初始化提供器。</para>
        /// </summary>
        internal ServiceProvider(Dictionary<Type, ServiceCollection.ServiceInfo> services)
        {
            m_Services = new Dictionary<Type, IServiceCreator>();

            foreach (var svc in services) 
            {
                switch (svc.Value.Lifetime) 
                {
                    case ServiceLifetimeScope_e.Singleton:
                        m_Services.Add(svc.Key, new SingletonService(svc.Value.Factory));
                        break;

                    case ServiceLifetimeScope_e.Transient:
                        m_Services.Add(svc.Key, new TransientService(svc.Value.Factory));
                        break;

                    default:
                        throw new NotSupportedException();
                }
            }
        }

        /// <summary>
        /// Resolves service instance by type.
        /// <para>按类型解析服务实例。</para>
        /// </summary>
        public object GetService(Type serviceType)
        {
            if (m_Services.TryGetValue(serviceType, out var svcFact))
            {
                return svcFact.CreateService();
            }
            else
            {
                throw new ServiceNotRegisteredException(serviceType);
            }
        }

        /// <summary>
        /// Disposes managed singleton services.
        /// <para>释放已创建的可释放单例服务。</para>
        /// </summary>
        public void Dispose()
        {
            if (!m_IsDisposed)
            {
                foreach (var svc in m_Services.Values)
                {
                    svc.Dispose();
                }

                m_IsDisposed = true;
            }
        }
    }

    /// <summary>
    /// Extensions for strongly-typed service resolution.
    /// <para>用于强类型服务解析的扩展方法。</para>
    /// </summary>
    public static class IServiceProviderExtension 
    {
        /// <summary>
        /// Resolves service by generic type parameter.
        /// <para>通过泛型参数解析服务实例。</para>
        /// </summary>
        public static TService GetService<TService>(this IServiceProvider provider) 
            => (TService)provider.GetService(typeof(TService));
    }
}
