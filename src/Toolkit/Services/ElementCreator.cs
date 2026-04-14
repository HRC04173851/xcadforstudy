// -*- coding: utf-8 -*-
// src/Toolkit/Services/ElementCreator.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件实现 xCAD 框架的核心服务类 - ElementCreator。
// ElementCreator 是管理 CAD 对象生命周期的核心组件。
//
// 设计模式：延迟创建（Lazy Creation）
// - 很多 CAD 对象创建成本很高（如打开文档、计算几何等）
// - ElementCreator 延迟实际创建操作，直到真正需要对象时才执行
// - 支持创建模板对象（尚未提交到 CAD 系统）
//
// 核心功能：
// 1. 延迟创建 - Create 方法延迟到首次访问 Element 属性时
// 2. 缓存属性 - CachedProperties 管理模板状态的属性值
// 3. 提交/回滚 - 支持提交创建（Commit）或放弃（无需显式回滚）
// 4. 状态跟踪 - IsCreated/IsCommitted 跟踪对象状态
//
// 使用场景：
// - 文档创建：创建新文档时，先设置属性（路径、模板），实际打开延迟到 Commit
// - 几何创建：创建几何体时，先设置参数，实际计算延迟到首次访问
// - 特征创建：创建特征时，先设置编辑参数，实际修改延迟到确认编辑
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Xarial.XCad.Exceptions;
using Xarial.XCad.Toolkit.Exceptions;

namespace Xarial.XCad.Services
{
    /// <summary>
    /// Manages cached properties of <see cref="Base.IXTransaction"/>
    /// <para>管理 <see cref="Base.IXTransaction"/> 的缓存属性值。</para>
    /// </summary>
    public class CachedProperties 
    {
        private readonly Dictionary<string, object> m_CachedProperties;

        /// <summary>
        /// Gets cached value for property name.
        /// <para>获取指定属性名对应的缓存值。</para>
        /// </summary>
        public T Get<T>([CallerMemberName]string prpName = "")
        {
            object val;

            if (!m_CachedProperties.TryGetValue(prpName, out val))
            {
                val = default(T);
                m_CachedProperties.Add(prpName, val);
            }

            return (T)val;
        }

        /// <summary>
        /// Sets cached value for property name.
        /// <para>设置指定属性名对应的缓存值。</para>
        /// </summary>
        public void Set<T>(T val, [CallerMemberName]string prpName = "")
        {
            m_CachedProperties[prpName] = val;
        }

        /// <summary>
        /// Checks whether property value is present in cache.
        /// <para>检查缓存中是否已存在该属性值。</para>
        /// </summary>
        public bool Has<T>([CallerMemberName] string prpName = "") 
            => m_CachedProperties.ContainsKey(prpName);

        internal CachedProperties() 
        {
            m_CachedProperties = new Dictionary<string, object>();
        }
    }

    /// <summary>
    /// Helper class to manage the lifecycle of <see cref="Base.IXTransaction"/>
    /// <para>用于管理 <see cref="Base.IXTransaction"/> 生命周期的辅助接口。</para>
    /// </summary>
    /// <typeparam name="TElem">Type of the underlying object</typeparam>
    public interface IElementCreatorBase<TElem>
    {
        /// <summary>
        /// True if this element is created or false if it is a template
        /// <para>若元素已创建则为 `true`，否则为模板状态。</para>
        /// </summary>
        bool IsCreated { get; }

        /// <summary>
        /// Provides access to manage cached properties
        /// <para>提供访问缓存属性的能力。</para>
        /// </summary>
        CachedProperties CachedProperties { get; }

        /// <summary>
        /// Pointer to the specific element
        /// <para>指向具体元素实例。</para>
        /// </summary>
        /// <exception cref="NonCommittedElementAccessException"/>
        TElem Element { get; }

        /// <summary>
        /// Sets the element to the specified value and updates the state
        /// <para>将元素设置为指定值并更新创建状态。</para>
        /// </summary>
        /// <param name="elem">Element or null</param>
        void Set(TElem elem);

        /// <summary>
        /// Forcibly inits the element instance
        /// <para>强制初始化元素实例。</para>
        /// </summary>
        /// <param name="elem">Element to set</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <exception cref="ElementAlreadyCommittedException"/>
        /// <remarks> Element must not be null and not commited. This method will also call the post creation handler</remarks>
        /// <remarks><para>元素不能为空且未提交；该方法会调用后置创建处理器。</para></remarks>
        void Init(TElem elem, CancellationToken cancellationToken);
    }

    /// <inheritdoc/>
    public interface IElementCreator<TElem> : IElementCreatorBase<TElem>
    {
        /// <summary>
        /// Creates element from the template
        /// <para>从模板创建元素实例。</para>
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <exception cref="ElementAlreadyCommittedException"/>
        /// <returns>Instance of the specific element</returns>
        TElem Create(CancellationToken cancellationToken);
    }

    /// <inheritdoc/>
    public interface IAsyncElementCreator<TElem> : IElementCreatorBase<TElem>
    {
        /// <summary>
        /// Async creates element from the template
        /// <para>异步从模板创建元素实例。</para>
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <exception cref="ElementAlreadyCommittedException"/>
        /// <returns>Instance of the specific element</returns>
        Task<TElem> CreateAsync(CancellationToken cancellationToken);
    }

    /// <summary>
    /// Base implementation for synchronous/asynchronous element creators.
    /// <para>同步/异步元素创建器的基础实现。</para>
    /// </summary>
    public abstract class ElementCreatorBase<TElem> : IElementCreatorBase<TElem>
    {
        public bool IsCreated { get; protected set; }

        protected TElem m_Element;

        protected readonly Action<TElem, CancellationToken> m_PostCreator;

        public CachedProperties CachedProperties { get; }

        /// <summary>
        /// Initializes element creator base.
        /// <para>初始化元素创建器基类。</para>
        /// </summary>
        public ElementCreatorBase(Action<TElem, CancellationToken> postCreator, TElem elem, bool created = false)
        {
            m_PostCreator = postCreator;

            IsCreated = created;
            m_Element = elem;

            CachedProperties = new CachedProperties();
        }

        public TElem Element
        {
            get
            {
                if (IsCreated)
                {
                    return m_Element;
                }
                else
                {
                    throw new NonCommittedElementAccessException();
                }
            }
        }

        public void Init(TElem elem, CancellationToken cancellationToken)
        {
            if (!IsCreated)
            {
                if (elem == null)
                {
                    throw new ArgumentNullException(nameof(elem));
                }

                Set(elem);

                m_PostCreator?.Invoke(elem, cancellationToken);
            }
            else
            {
                throw new ElementAlreadyCommittedException();
            }
        }

        public void Set(TElem elem)
        {
            m_Element = elem;
            IsCreated = m_Element != null;
        }
    }

    /// <summary>
    /// Synchronous element creator implementation.
    /// <para>同步元素创建器实现。</para>
    /// </summary>
    public class ElementCreator<TElem> : ElementCreatorBase<TElem>, IElementCreator<TElem>
    {
        private readonly Func<CancellationToken, TElem> m_Creator;
        
        public ElementCreator(Func<CancellationToken, TElem> creator, TElem elem, bool created = false)
            : this(creator, null, elem, created)
        {
        }

        public ElementCreator(Func<CancellationToken, TElem> creator, Action<TElem, CancellationToken> postCreator, TElem elem, bool created = false) 
            : base(postCreator, elem, created)
        {
            m_Creator = creator;
        }

        /// <summary>
        /// Creates element synchronously.
        /// <para>以同步方式创建元素。</para>
        /// </summary>
        public TElem Create(CancellationToken cancellationToken)
        {
            if (!IsCreated)
            {
                m_Element = m_Creator.Invoke(cancellationToken);
                IsCreated = true;
                m_PostCreator?.Invoke(m_Element, cancellationToken);
                return m_Element;
            }
            else
            {
                throw new ElementAlreadyCommittedException();
            }
        }
    }

    /// <summary>
    /// Asynchronous element creator implementation.
    /// <para>异步元素创建器实现。</para>
    /// </summary>
    public class AsyncElementCreator<TElem> : ElementCreatorBase<TElem>, IAsyncElementCreator<TElem>
    {
        private readonly Func<CancellationToken, Task<TElem>> m_AsyncCreator;

        public AsyncElementCreator(Func<CancellationToken, Task<TElem>> asyncCreator, TElem elem, bool created = false)
            : this(asyncCreator, null, elem, created)
        {
        }

        public AsyncElementCreator(Func<CancellationToken, Task<TElem>> asyncCreator, Action<TElem, CancellationToken> postCreator, TElem elem, bool created = false)
            : base(postCreator, elem, created)
        {
            m_AsyncCreator = asyncCreator;
        }

        /// <summary>
        /// Creates element asynchronously.
        /// <para>以异步方式创建元素。</para>
        /// </summary>
        public async Task<TElem> CreateAsync(CancellationToken cancellationToken)
        {
            if (!IsCreated)
            {
                m_Element = await m_AsyncCreator.Invoke(cancellationToken);
                IsCreated = true;
                m_PostCreator?.Invoke(m_Element, cancellationToken);
                return m_Element;
            }
            else
            {
                throw new ElementAlreadyCommittedException();
            }
        }
    }
}