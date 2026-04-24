// -*- coding: utf-8 -*-
// tests/Toolkit.Tests/EventsHandlerTests.cs

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xarial.XCad.Toolkit.Services;

namespace Toolkit.Tests
{
    /// <summary>
    /// 测试 EventsHandler 事件订阅/取消订阅功能。
    /// EventsHandler 是一个抽象基类，用于管理事件的订阅和取消订阅操作。
    /// </summary>
    public class EventsHandlerTests
    {
        /// <summary>
        /// EventsHandlerMock：EventsHandler 的测试用实现。
        /// 使用 StringBuilder 记录订阅(s)和取消订阅(u)操作。
        /// </summary>
        private class EventsHandlerMock : EventsHandler<Action>
        {
            private StringBuilder m_Log;

            public EventsHandlerMock(StringBuilder log)
            {
                m_Log = log;
            }

            /// <summary>
            /// 订阅事件时追加 "s" 到日志。
            /// </summary>
            protected override void SubscribeEvents()
            {
                m_Log.Append("s");
            }

            /// <summary>
            /// 取消订阅事件时追加 "u" 到日志。
            /// </summary>
            protected override void UnsubscribeEvents()
            {
                m_Log.Append("u");
            }

            /// <summary>
            /// 手动触发事件，用于测试。
            /// </summary>
            public void RaiseEvent()
            {
                Delegate?.Invoke();
            }
        }

        /// <summary>
        /// ObserverMock：模拟观察者，通过事件访问器连接 EventsHandler。
        /// 提供 add/remove 访问器以测试 Attach/Detach 功能。
        /// </summary>
        private class ObserverMock
        {
            public EventsHandlerMock Handler { get; }

            public event Action Event
            {
                add
                {
                    Handler.Attach(value);
                }
                remove
                {
                    Handler.Detach(value);
                }
            }

            public ObserverMock(StringBuilder log)
            {
                Handler = new EventsHandlerMock(log);
            }
        }

        /// <summary>
        /// 测试用例目的：验证 EventsHandler 的订阅、取消订阅和事件触发功能。
        /// 包括：
        /// - 重复订阅同一委托应只订阅一次
        /// - 取消订阅应正确移除委托
        /// - 事件触发应执行所有订阅的委托
        /// </summary>
        [Test]
        public void SubscribeUnsubscribeTest()
        {
            var l = new StringBuilder();

            var obs = new ObserverMock(l);

            var res = "";

            // 创建测试委托：追加 "A" 到结果字符串
            var del = new Action(() =>
            {
                res += "A";
            });

            // 第一次订阅
            obs.Event += del;
            var l1 = l.ToString();
            // 第二次订阅同一委托（应该只订阅一次，log 不增加）
            obs.Event += del;
            var l2 = l.ToString();

            // 触发事件
            obs.Handler.RaiseEvent();

            // 验证事件触发执行了委托（两次，因为订阅了两次）
            Action del1 = null;
            Action del2 = null;

            // 第一次取消订阅
            obs.Event -= del;
            var l3 = l.ToString();
            del1 = obs.Handler.Delegate;
            // 第二次取消订阅
            obs.Event -= del;
            var l4 = l.ToString();
            del2 = obs.Handler.Delegate;

            // 验证结果
            Assert.AreEqual("AA", res);       // 事件触发执行了两次
            Assert.AreEqual("s", l1);         // 第一次订阅：追加 "s"
            Assert.AreEqual("s", l2);         // 第二次订阅：未追加（已订阅）
            Assert.AreEqual("s", l3);         // 第一次取消：未追加
            Assert.AreEqual("su", l4);        // 第二次取消：追加 "u"
            Assert.IsNotNull(del1);           // 第一次取消后仍有委托
            Assert.IsNull(del2);              // 第二次取消后无委托
        }
    }
}
