// -*- coding: utf-8 -*-
// src/Toolkit/Utils/RepositoryHelper.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件实现 IXRepository 的通用辅助方法集合 RepositoryHelper。
// 提供实体的预创建、移除、按名称查找和过滤等功能。
// 用于标准化仓储操作的常见模式和最佳实践。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using Xarial.XCad.Base;
using Xarial.XCad.Exceptions;

namespace Xarial.XCad.Toolkit.Utils
{
    /// <summary>
    /// Helper functions of <see cref="IXRepository"/>
    /// <para><see cref="IXRepository"/> 的通用辅助方法集合。</para>
    /// </summary>
    public static class RepositoryHelper
    {
        /// <summary>
        /// Helper tool to automatically create specific entities
        /// <para>用于自动预创建特定实体类型的辅助方法。</para>
        /// </summary>
        /// <typeparam name="TEnt">Generic entity<para>通用实体类型。</para></typeparam>
        /// <typeparam name="TSpecEnt">Specific entity<para>目标特定实体类型。</para></typeparam>
        /// <param name="repo">Repository<para>目标仓储。</para></param>
        /// <param name="factories">Factories of the specific objects<para>可用实体工厂表达式集合。</para></param>
        /// <returns>Specific entity<para>返回创建出的特定实体。</para></returns>
        /// <exception cref="EntityNotSupportedException"/>
        public static TSpecEnt PreCreate<TEnt, TSpecEnt>(IXRepository<TEnt> repo, params Expression<Func<TEnt>>[] factories)
            where TEnt : IXTransaction
            where TSpecEnt : TEnt
        {
            var supportedTypes = new List<Type>();

            foreach (var factory in factories)
            {
                var type = factory.Body.Type;

                if (typeof(TSpecEnt).IsAssignableFrom(type))
                {
                    return (TSpecEnt)factory.Compile().Invoke();
                }

                supportedTypes.Add(type);
            }

            throw new EntityNotSupportedException(typeof(TSpecEnt), supportedTypes);
        }

        /// <summary>
        /// Removes the entities
        /// <para>移除实体集合。</para>
        /// </summary>
        /// <typeparam name="TEnt">Type of entity</typeparam>
        /// <param name="repo">Repository</param>
        /// <param name="ents">Entities</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <exception cref="Exception">Thrown if entity is not selectable</exception>
        public static void RemoveAll<TEnt>(IXRepository<TEnt> repo, IEnumerable<TEnt> ents, CancellationToken cancellationToken)
            where TEnt : IXTransaction
        {
            foreach (var ent in ents.ToArray()) 
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (ent is IXSelObject)
                {
                    ((IXSelObject)ent).Delete();
                }
                else 
                {
                    throw new Exception($"Only '{nameof(IXSelObject)}' entities can be removed");
                }
            }
        }

        /// <summary>
        /// Tries to find the <see cref="IHasName"/> entity in the repository
        /// <para>尝试按名称在仓储中查找 <see cref="IHasName"/> 实体。</para>
        /// </summary>
        /// <typeparam name="TEnt">Entity type</typeparam>
        /// <param name="repo">Repository</param>
        /// <param name="name">Name of the entity</param>
        /// <param name="ent">Entity</param>
        /// <returns>True if found</returns>
        public static bool TryFindByName<TEnt>(IXRepository<TEnt> repo, string name, out TEnt ent)
            where TEnt : IXTransaction
        {
            ent = (TEnt)repo.OfType<IHasName>()
                    .FirstOrDefault(e => string.Equals(e.Name, name, StringComparison.CurrentCultureIgnoreCase));

            return ent != null;
        }

        /// <summary>
        /// Gets the entity by name
        /// <para>按名称获取实体。</para>
        /// </summary>
        /// <typeparam name="TEnt">Type of entity</typeparam>
        /// <param name="repo">Target repository</param>
        /// <param name="name">Name of the entity</param>
        /// <returns>Pointer to named entity</returns>
        /// <exception cref="EntityNotFoundException"/>
        public static TEnt Get<TEnt>(IXRepository<TEnt> repo, string name)
            where TEnt : IXTransaction
        {
            if (repo.TryGet(name, out TEnt ent))
            {
                return ent;
            }
            else
            {
                throw new EntityNotFoundException(name);
            }
        }

        /// <summary>
        /// Performs the default commiting of entities into repository one-by-one
        /// <para>按默认方式逐个提交实体到仓储。</para>
        /// </summary>
        /// <typeparam name="TEnt">Entity type</typeparam>
        /// <param name="repo">Repository</param>
        /// <param name="ents">Entities</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <exception cref="OperationCanceledException"/>
        public static void AddRange<TEnt>(IEnumerable<TEnt> ents, CancellationToken cancellationToken)
            where TEnt : IXTransaction
        {
            if (ents == null) 
            {
                throw new ArgumentNullException(nameof(ents));
            }

            foreach (var ent in ents) 
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    ent.Commit(cancellationToken);
                }
                else 
                {
                    throw new OperationCanceledException();
                }
            }
        }

        /// <summary>
        /// Performs the default filtering of the entities
        /// <para>按默认规则过滤实体集合。</para>
        /// </summary>
        /// <typeparam name="TEnt"></typeparam>
        /// <param name="repoEnts">Repository entities to filter</param>
        /// <param name="filters">Filters</param>
        /// <param name="reverseOrder">True to reverse the order</param>
        /// <returns>Filtered entities</returns>
        /// <exception cref="EntityNotFoundException"></exception>
        public static IEnumerable<TEnt> FilterDefault<TEnt>(IEnumerable<TEnt> repoEnts, RepositoryFilterQuery[] filters, bool reverseOrder)
            where TEnt : IXTransaction
        {
            var filteredEnts = new List<TEnt>();

            foreach (var ent in repoEnts)
            {
                if (MatchesFilters(ent, filters))
                {
                    if (reverseOrder)
                    {
                        filteredEnts.Insert(0, ent);
                    }
                    else
                    {
                        yield return ent;
                    }
                }
            }

            foreach (var ent in filteredEnts)
            {
                yield return ent;
            }
        }

        /// <summary>
        /// Checks if the specified entity matches the filter
        /// <para>检查指定实体是否匹配过滤条件。</para>
        /// </summary>
        /// <typeparam name="TEnt">Entity type</typeparam>
        /// <param name="ent">Entity to match</param>
        /// <param name="filters">Filters</param>
        /// <returns>True if entity matches the filter</returns>
        public static bool MatchesFilters<TEnt>(TEnt ent, params RepositoryFilterQuery[] filters)
        {
            if (filters?.Any() == true)
            {
                foreach (var filter in filters)
                {
                    if (filter.Type == null || filter.Type.IsAssignableFrom(ent.GetType()))
                    {
                        return true;
                    }
                }

                return false;
            }
            else 
            {
                return true;
            }   
        }
    }
}
