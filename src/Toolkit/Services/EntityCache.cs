// -*- coding: utf-8 -*-
// src/Toolkit/Services/EntityCache.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件实现实体缓存服务 EntityCache<TEnt>。
// 管理从未提交对象创建出的实体缓存，支持添加、移除和提交。
// 用于延迟创建实体的生命周期管理和批量提交。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xarial.XCad.Base;

namespace Xarial.XCad.Toolkit.Services
{
    /// <summary>
    /// This service allows to manage the entities which are created from the uncommitted object
    /// <para>该服务用于管理从未提交对象创建出的实体缓存。</para>
    /// </summary>
    /// <typeparam name="TEnt">Type of entity<para>实体类型。</para></typeparam>
    public class EntityCache<TEnt>
            where TEnt : IXTransaction
    {
        private readonly List<TEnt> m_Cache;

        private readonly Func<TEnt, string> m_NameProvider;
        protected readonly IXTransaction m_Owner;
        protected readonly IXRepository<TEnt> m_Repo;

        /// <summary>
        /// Initializes entity cache.
        /// <para>初始化实体缓存。</para>
        /// </summary>
        public EntityCache(IXTransaction owner, IXRepository<TEnt> repo, Func<TEnt, string> nameProvider)
        {
            m_Owner = owner;
            m_Repo = repo;
            m_NameProvider = nameProvider;

            m_Cache = new List<TEnt>();
        }

        /// <summary>
        /// Number of cached entities.
        /// <para>缓存实体数量。</para>
        /// </summary>
        public int Count => m_Cache.Count;

        public void AddRange(IEnumerable<TEnt> ents, CancellationToken cancellationToken)
            => m_Cache.AddRange(ents);

        public IEnumerator<TEnt> GetEnumerator() => IterateEntities(m_Cache).GetEnumerator();

        public void RemoveRange(IEnumerable<TEnt> ents, CancellationToken cancellationToken)
        {
            foreach (var ent in ents)
            {
                if (!m_Cache.Remove(ent))
                {
                    throw new Exception($"Failed to remove '{m_NameProvider.Invoke(ent)}' from cache");
                }
            }
        }

        public bool TryGet(string name, out TEnt ent)
        {
            ent = m_Cache.FirstOrDefault(c => string.Equals(m_NameProvider.Invoke(c), name, StringComparison.CurrentCultureIgnoreCase));

            return ent != null;
        }

        /// <summary>
        /// Commits cached entities to repository and clears cache.
        /// <para>将缓存实体提交到仓储并清空缓存。</para>
        /// </summary>
        public void Commit(CancellationToken cancellationToken)
        {
            try
            {
                CommitEntitiesFromCache(m_Cache, cancellationToken);
            }
            finally
            {
                m_Cache.Clear();
            }
        }

        protected virtual IEnumerable<TEnt> IterateEntities(IReadOnlyList<TEnt> ents) => ents;
        
        protected virtual void CommitEntitiesFromCache(IReadOnlyList<TEnt> ents, CancellationToken cancellationToken) 
        {
            if (ents.Any())
            {
                if (m_Owner.IsCommitted)
                {
                    m_Repo.AddRange(ents, cancellationToken);
                }
                else
                {
                    throw new Exception("Commit is only possible when the owner entity is committed");
                }
            }
        }
    }
}
