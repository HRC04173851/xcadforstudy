//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using Xarial.XCad.Exceptions;

namespace Xarial.XCad.Base
{
    /// <summary>
    /// Provides additional methods for <see cref="IXRepository{TEnt}"/>
    /// 为 <see cref="IXRepository{TEnt}"/> 提供扩展方法
    /// </summary>
    public static class XRepositoryExtension
    {
        /// <summary>
        /// Adds entities to the repository without the cancellation token
        /// 不使用取消令牌将实体添加到存储库
        /// </summary>
        /// <typeparam name="TEnt">Type of entity 实体类型</typeparam>
        /// <param name="repo">Target repository 目标存储库</param>
        /// <param name="ents">Entities 实体集合</param>
        public static void AddRange<TEnt>(this IXRepository<TEnt> repo, IEnumerable<TEnt> ents)
            where TEnt : IXTransaction
            => repo.AddRange(ents, CancellationToken.None);

        /// <summary>
        /// Removes entities from the repository without the cancellation token
        /// 不使用取消令牌从存储库移除实体
        /// </summary>
        /// <typeparam name="TEnt">Type of entity 实体类型</typeparam>
        /// <param name="repo">Target repository 目标存储库</param>
        /// <param name="ents">Entities 实体集合</param>
        public static void RemoveRange<TEnt>(this IXRepository<TEnt> repo, IEnumerable<TEnt> ents)
            where TEnt : IXTransaction
            => repo.RemoveRange(ents, CancellationToken.None);

        /// <inheritdoc/>
        public static void Add<TEnt>(this IXRepository<TEnt> repo, params TEnt[] ents)
            where TEnt : IXTransaction
            => repo.AddRange(ents, CancellationToken.None);

        /// <summary>
        /// Adds object one-by-one or as an array
        /// 一个一个或批量添加实体（带取消令牌）
        /// </summary>
        /// <typeparam name="TEnt">Type of entity 实体类型</typeparam>
        /// <param name="repo">Target repository 目标存储库</param>
        /// <param name="cancellationToken">Cancellation token 取消令牌</param>
        /// <param name="ents">Entities to add 要添加的实体</param>
        public static void Add<TEnt>(this IXRepository<TEnt> repo, CancellationToken cancellationToken, params TEnt[] ents)
            where TEnt : IXTransaction
            => repo.AddRange(ents, cancellationToken);

        /// <inheritdoc/>
        public static void Remove<TEnt>(this IXRepository<TEnt> repo, params TEnt[] ents)
            where TEnt : IXTransaction
            => repo.RemoveRange(ents, CancellationToken.None);

        /// <summary>
        /// Removes object one-by-one or as an array
        /// 一个一个或批量移除实体（带取消令牌）
        /// </summary>
        /// <typeparam name="TEnt">Type of entity 实体类型</typeparam>
        /// <param name="repo">Target repository 目标存储库</param>
        /// <param name="cancellationToken">Cancellation token 取消令牌</param>
        /// <param name="ents">Entities to remove 要移除的实体</param>
        public static void Remove<TEnt>(this IXRepository<TEnt> repo, CancellationToken cancellationToken, params TEnt[] ents)
            where TEnt : IXTransaction
            => repo.RemoveRange(ents, cancellationToken);

        /// <summary>
        /// Pre-creates default template
        /// 预创建默认模板对象
        /// </summary>
        /// <typeparam name="TEnt"></typeparam>
        /// <param name="repo">Repository 存储库</param>
        /// <returns>Entity template 实体模板</returns>
        public static TEnt PreCreate<TEnt>(this IXRepository<TEnt> repo) where TEnt : IXTransaction
            => repo.PreCreate<TEnt>();

        /// <summary>
        /// Filters entities by type
        /// 按类型筛选实体
        /// </summary>
        /// <typeparam name="TSpecificEnt">Entity type 实体类型</typeparam>
        /// <param name="repo">Repository 存储库</param>
        /// <param name="reverseOrder">True to reverse order 为 true 则反转顺序</param>
        /// <returns>Filtered entities 筛选后的实体</returns>
        public static IEnumerable<TSpecificEnt> Filter<TSpecificEnt>(this IXRepository repo, bool reverseOrder = false)
            => repo.Filter(reverseOrder, new RepositoryFilterQuery()
            {
                Type = typeof(TSpecificEnt)
            }).Cast<TSpecificEnt>();

        /// <summary>
        /// Checks if the specified entity exists
        /// 检查指定实体是否存在
        /// </summary>
        /// <typeparam name="TEnt">Type of entity 实体类型</typeparam>
        /// <param name="repo">Target repository 目标存储库</param>
        /// <param name="name">Name of the entity 实体名称</param>
        /// <returns>True if entity exists, False if not 实体存在返回 true，否则返回 false</returns>
        public static bool Exists<TEnt>(this IXRepository<TEnt> repo, string name) where TEnt : IXTransaction
            => repo.TryGet(name, out _);
    }
}