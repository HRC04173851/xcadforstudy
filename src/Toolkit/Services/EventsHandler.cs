// -*- coding: utf-8 -*-
// src/Toolkit/Services/EventsHandler.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件实现延迟事件处理器基类 EventsHandler<TDel>。
// 为包装事件实现延迟订阅处理，只有在有外部订阅者时才订阅底层事件。
// 用于优化事件处理性能和资源管理。
//*********************************************************************

using System;

namespace Xarial.XCad.Toolkit.Services
{
    /// <summary>
    /// Utility allowing to implement lazy event handlers for a wrapped events
    /// <para>用于为包装事件实现延迟订阅处理器的工具类。</para>
    /// </summary>
    /// <typeparam name="TDel">Delegate type of the wrapped event<para>被包装事件的委托类型。</para></typeparam>
    /// <remarks>Use this approach when handling of events might result in the performance penalties or other issues
    /// so it is only required to subscribe to events when underlying users are subscribed
    /// <para>当事件处理存在性能开销或副作用时，可仅在外部用户订阅后再订阅底层事件。</para></remarks>
    public abstract class EventsHandler<TDel> : IDisposable
        where TDel : Delegate
    {
        /// <summary>
        /// Combined delegate invocation list.
        /// <para>组合后的委托调用列表。</para>
        /// </summary>
        public TDel Delegate { get; set; }

        private bool m_IsSubscribed;

        /// <summary>
        /// Initializes lazy events handler.
        /// <para>初始化延迟事件处理器。</para>
        /// </summary>
        protected EventsHandler()
        {
            m_IsSubscribed = false;
        }

        /// <summary>
        /// Attaches handler and subscribes wrapped events if needed.
        /// <para>附加处理委托，并在需要时订阅底层事件。</para>
        /// </summary>
        public void Attach(TDel del)
        {
            SubscribeIfNeeded();
            AddToInvocationList(del);
        }

        /// <summary>
        /// Detaches handler and unsubscribes wrapped events when no subscribers remain.
        /// <para>移除处理委托，当无订阅者时取消订阅底层事件。</para>
        /// </summary>
        public void Detach(TDel del)
        {
            RemoveFromInvocationList(del);

            if (Delegate == null)
            {
                UnsubscribeIfNeeded();
            }
        }

        private void SubscribeIfNeeded()
        {
            if (!m_IsSubscribed)
            {
                SubscribeEvents();

                m_IsSubscribed = true;
            }
        }

        private void UnsubscribeIfNeeded()
        {
            if (m_IsSubscribed)
            {
                UnsubscribeEvents();

                m_IsSubscribed = false;
            }
        }

        /// <summary>
        /// Releases underlying event subscriptions.
        /// <para>释放底层事件订阅资源。</para>
        /// </summary>
        public void Dispose()
        {
            UnsubscribeIfNeeded();
        }

        protected abstract void SubscribeEvents();
        protected abstract void UnsubscribeEvents();

        private void AddToInvocationList(TDel del)
        {
            Delegate = System.Delegate.Combine(Delegate, del) as TDel;
        }
        
        private void RemoveFromInvocationList(TDel del)
        {
            Delegate = System.Delegate.Remove(Delegate, del) as TDel;
        }
    }
}
