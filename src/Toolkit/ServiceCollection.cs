// -*- coding: utf-8 -*-
// src/Toolkit/ServiceCollection.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件实现 Toolkit 依赖注入的服务注册容器 ServiceCollection。
// 提供服务的添加、替换和深拷贝功能。
// 支持单例和瞬态生命周期管理，创建不可变服务提供器。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xarial.XCad.Toolkit
{
    /// <summary>
    /// Service registration container for Toolkit dependency injection.
    /// <para>Toolkit 依赖注入的服务注册容器。</para>
    /// </summary>
    public class ServiceCollection : IXServiceCollection
    {
        internal class ServiceInfo 
        {
            internal Func<object> Factory { get; }
            internal ServiceLifetimeScope_e Lifetime { get; }

            internal ServiceInfo(Func<object> factory, ServiceLifetimeScope_e lifetime)
            {
                Factory = factory;
                Lifetime = lifetime;
            }
        }

        private readonly Dictionary<Type, ServiceInfo> m_Services;

        private bool m_IsProviderCreated;

        /// <summary>
        /// Initializes empty service collection.
        /// <para>初始化空的服务集合。</para>
        /// </summary>
        public ServiceCollection() : this(new Dictionary<Type, ServiceInfo>()) 
        {
        }

        private ServiceCollection(Dictionary<Type, ServiceInfo> svcs)
        {
            m_Services = svcs;
            m_IsProviderCreated = false;
        }

        /// <summary>
        /// Adds or replaces service registration.
        /// <para>添加或替换服务注册项。</para>
        /// </summary>
        public void Add(Type svcType, Func<object> svcFactory, ServiceLifetimeScope_e lifetime = ServiceLifetimeScope_e.Singleton, bool replace = true)
        {
            if (replace || !m_Services.ContainsKey(svcType))
            {
                m_Services[svcType] = new ServiceInfo(svcFactory, lifetime);
            }
        }

        /// <summary>
        /// Creates immutable service provider from current registrations.
        /// <para>根据当前注册项创建不可变服务提供器。</para>
        /// </summary>
        public IServiceProvider CreateProvider()
        {
            if (!m_IsProviderCreated)
            {
                m_IsProviderCreated = true;
                return new ServiceProvider(m_Services);
            }
            else 
            {
                throw new Exception("Provider is already created");
            }
        }

        /// <summary>
        /// Creates deep copy of service collection.
        /// <para>创建服务集合的深拷贝。</para>
        /// </summary>
        public IXServiceCollection Clone()
            => new ServiceCollection(m_Services.ToDictionary(x => x.Key, x => new ServiceInfo(x.Value.Factory, x.Value.Lifetime)));
    }
}
