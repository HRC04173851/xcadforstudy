// -*- coding: utf-8 -*-
// tests/Toolkit.Tests/ServiceCollectionTests.cs

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xarial.XCad;
using Xarial.XCad.Toolkit;
using Xarial.XCad.Toolkit.Exceptions;

namespace Toolkit.Tests
{
    /// <summary>
    /// 测试 ServiceCollection 依赖注入容器的核心功能。
    /// 包括：服务替换、生命周期管理、Dispose 释放、以及服务未注册时的异常处理。
    /// </summary>
    public class ServiceCollectionTests
    {
        /// <summary>
        /// 测试用接口和实现类。
        /// </summary>
        public interface I1
        {
        }

        public class C1_1 : I1
        {
        }

        public class C1_2 : I1
        {
        }

        public interface I2
        {
        }

        public class C2_1 : I2
        {
        }

        public class C2_2 : I2
        {
        }

        public interface I3
        {
        }

        /// <summary>
        /// C3：实现 IDisposable 的服务，用于测试 Dispose 释放。
        /// </summary>
        public class C3 : I3, IDisposable
        {
            public bool IsDisposed { get; private set; }

            public void Dispose()
            {
                IsDisposed = true;
            }
        }

        public interface I4
        {
        }

        public class C4 : I4, IDisposable
        {
            public bool IsDisposed { get; private set; }

            public void Dispose()
            {
                IsDisposed = true;
            }
        }

        /// <summary>
        /// 测试用例目的：验证服务替换功能（replaceExisting=true）。
        /// 当同一个接口注册多个实现时，replaceExisting=true 会用新的实现替换旧的。
        /// 预期结果：I1 返回 C1_2，I2 返回 C2_1（未被替换）
        /// </summary>
        [Test]
        public void ReplaceTest()
        {
            var svcColl = new ServiceCollection();

            // 注册 I1 的第一个实现，允许替换
            svcColl.Add<I1, C1_1>(ServiceLifetimeScope_e.Transient, true);
            // 替换为 C1_2
            svcColl.Add<I1, C1_2>(ServiceLifetimeScope_e.Transient, true);
            // 注册 I2，不允许替换（false）
            svcColl.Add<I2, C2_1>(ServiceLifetimeScope_e.Transient, false);
            // 尝试替换 C2_2（但 replaceExisting=false，无效）
            svcColl.Add<I2, C2_2>(ServiceLifetimeScope_e.Transient, false);

            var svcProv = svcColl.CreateProvider();

            var s1 = svcProv.GetService<I1>();
            var s2 = svcProv.GetService<I2>();

            // 验证 I1 返回的是 C1_2（被替换了）
            Assert.IsAssignableFrom<C1_2>(s1);
            // 验证 I2 返回的是 C2_1（替换无效）
            Assert.IsAssignableFrom<C2_1>(s2);
        }

        /// <summary>
        /// 测试用例目的：验证服务生命周期（Transient vs Singleton）的行为差异。
        /// - Transient：每次 GetService 返回新实例
        /// - Singleton：每次 GetService 返回同一实例
        /// </summary>
        [Test]
        public void LifetimeTest()
        {
            var svcColl = new ServiceCollection();

            svcColl.Add<I1, C1_1>(ServiceLifetimeScope_e.Transient);
            svcColl.Add<I2, C2_1>(ServiceLifetimeScope_e.Singleton);

            var svcProv = svcColl.CreateProvider();

            var s1_1 = svcProv.GetService<I1>();
            var s1_2 = svcProv.GetService<I1>();
            var s2_1 = svcProv.GetService<I2>();
            var s2_2 = svcProv.GetService<I2>();

            // 验证 Transient 返回不同实例
            Assert.AreNotEqual(s1_1, s1_2);
            // 验证 Singleton 返回相同实例
            Assert.AreEqual(s2_1, s2_2);
        }

        /// <summary>
        /// 测试用例目的：验证服务容器的 Dispose 行为。
        /// - Singleton 服务在 Dispose 时会被释放
        /// - Transient 服务不会在 Dispose 时被释放（因为它们不由容器管理）
        /// </summary>
        [Test]
        public void DisposeTest()
        {
            var svcColl = new ServiceCollection();

            // 注册 Singleton 服务
            svcColl.Add<I2, C2_1>(ServiceLifetimeScope_e.Singleton);
            svcColl.Add<I3, C3>(ServiceLifetimeScope_e.Singleton); // C3 实现 IDisposable
            // 注册 Transient 服务
            svcColl.Add<I4, C4>(ServiceLifetimeScope_e.Transient); // C4 实现 IDisposable

            var svcProv = svcColl.CreateProvider();

            var s0 = svcProv.GetService<I2>();
            var s1 = (C3)svcProv.GetService<I3>();
            var s2 = (C4)svcProv.GetService<I4>();

            // 释放服务容器
            ((IDisposable)svcProv).Dispose();

            // 验证 Singleton 服务被正确释放
            Assert.That(s1.IsDisposed == true);
            // 验证 Transient 服务未被释放（由调用方管理）
            Assert.That(s2.IsDisposed == false);
        }

        /// <summary>
        /// 测试用例目的：验证获取未注册服务时抛出 ServiceNotRegisteredException。
        /// </summary>
        [Test]
        public void NotRegisteredServiceTest()
        {
            var svcColl = new ServiceCollection();

            svcColl.Add<I3, C3>(ServiceLifetimeScope_e.Singleton);

            var svcProv = svcColl.CreateProvider();

            var s1 = svcProv.GetService<I3>();

            // 验证获取未注册服务 I4 抛出异常
            Assert.Throws<ServiceNotRegisteredException>(() => svcProv.GetService<I4>());
        }
    }
}
