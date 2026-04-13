//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;

namespace Xarial.XCad
{
    /// <summary>
    /// Lifetime of the service
    /// 服务的生命周期范围
    /// </summary>
    public enum ServiceLifetimeScope_e
    {
        /// <summary>
        /// Keeps the same instance
        /// 保持同一实例（单例）
        /// </summary>
        /// 
        Singleton,
        /// <summary>
        /// Creates new instance for every service access
        /// 每次访问服务时创建新实例（短暂生命周期）
        /// </summary>
        Transient
    }

    /// <summary>
    /// Collection of services
    /// 服务集合，用于注册和识别服务
    /// </summary>
    public interface IXServiceCollection
    {
        /// <summary>
        /// Adds new service or replaces existing one
        /// 添加新服务或替换已存在的服务
        /// </summary>
        /// <param name="svcType">Service type 服务类型</param>
        /// <param name="svcFactory">Service factory 服务工厂</param>
        /// <param name="lifetime">Lifetime of the service 服务生命周期</param>
        /// <param name="replace">True to replace if service is registered, False if not 为 true 则如已注册则替换</param>
        void Add(Type svcType, Func<object> svcFactory, ServiceLifetimeScope_e lifetime = ServiceLifetimeScope_e.Singleton, bool replace = true);

        /// <summary>
        /// Creates service provider from this service
        /// 从此服务集合创建服务提供程序
        /// </summary>
        /// <returns></returns>
        IServiceProvider CreateProvider();

        /// <summary>
        /// Creates a clone of these services
        /// 创建当前服务集合的克隆
        /// </summary>
        /// <returns></returns>
        IXServiceCollection Clone();
    }

    /// <summary>
    /// Extension methods of <see cref="IXServiceCollection"/>
    /// </summary>
    public static class XServiceCollectionExtension 
    {
        /// <inheritdoc cref="IXServiceCollection.Add(Type, Func{object}, ServiceLifetimeScope_e, bool)"/>
        /// <param name="svcColl">Services collection</param>
        /// <typeparam name="TService">Service interface</typeparam>
        /// <typeparam name="TImplementation">Service implementation</typeparam>
        public static void Add<TService, TImplementation>(this IXServiceCollection svcColl, ServiceLifetimeScope_e lifetime = ServiceLifetimeScope_e.Transient, bool replace = true)
            where TImplementation : class, TService => Add(svcColl, typeof(TService), typeof(TImplementation), lifetime, replace);

        /// <inheritdoc cref="Add{TService, TImplementation}(IXServiceCollection, ServiceLifetimeScope_e, bool)"/>
        /// <param name="factory">Service creation factory</param>
        public static void Add<TService>(this IXServiceCollection svcColl, Func<TService> factory, ServiceLifetimeScope_e lifetime = ServiceLifetimeScope_e.Transient, bool replace = true)
            => svcColl.Add(typeof(TService), new Func<object>(() => factory.Invoke()), lifetime, replace);

        /// <inheritdoc cref="IXServiceCollection.Add(Type, Func{object}, ServiceLifetimeScope_e, bool)"/>
        /// <param name="svcType">Type of service</param>
        /// <param name="impType">Service implementation</param>
        public static void Add(this IXServiceCollection svcColl, Type svcType, Type impType, ServiceLifetimeScope_e lifetime = ServiceLifetimeScope_e.Transient, bool replace = true)
            => svcColl.Add(svcType, () => Activator.CreateInstance(impType), lifetime, replace);
    }
}
