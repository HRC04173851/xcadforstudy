//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Xarial.XCad.Base
{
    /// <summary>
    /// Represents the collection of elements
    /// 表示元素集合（基础存储库接口）
    /// </summary>
    public interface IXRepository : IEnumerable
    {
        /// <summary>
        /// Number of elements in the collection
        /// 集合中的元素数量
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Filters the entities with the specified query
        /// 使用指定查询条件筛选实体
        /// </summary>
        /// <param name="reverseOrder">Reverse the order of results 是否反转结果顺序</param>
        /// <param name="filters">Filters 筛选条件</param>
        /// <returns>Filtered entities 筛选后的实体集合</returns>
        IEnumerable Filter(bool reverseOrder, params RepositoryFilterQuery[] filters);//TODO: potentially replace the Filter with the IQueryable
    }

    /// <summary>
    /// Represents the collection of specific elements
    /// 表示特定类型元素的强类型集合
    /// </summary>
    /// <typeparam name="TEnt"></typeparam>
    public interface IXRepository<TEnt> : IXRepository, IEnumerable<TEnt>
        where TEnt : IXTransaction
    {
        /// <summary>
        /// Retrieves the element by name
        /// 按名称获取元素（元素不存在时抛出异常）
        /// </summary>
        /// <param name="name">Name of the element 元素名称</param>
        /// <returns>Pointer to element 指向该元素的指针</returns>
        /// <remarks>This method should through an exception for missing element. Use <see cref="TryGet(string, out TEnt)"/>for a safe way getting the element</remarks>
        TEnt this[string name] { get; }

        /// <summary>
        /// Attempts to get element by name
        /// 尝试按名称获取元素（安全方式，不抛出异常）
        /// </summary>
        /// <param name="name">Name of the element 元素名称</param>
        /// <param name="ent">Resulting element if exists or null otherwise 若存在则返回该元素，否则为 null</param>
        /// <returns>True if element exists 元素存在返回 true</returns>
        bool TryGet(string name, out TEnt ent);

        /// <summary>
        /// Commits entities
        /// 将实体提交（添加）到存储库
        /// </summary>
        /// <param name="ents"></param>
        /// <param name="cancellationToken">Cancellation token 取消令牌</param>
        void AddRange(IEnumerable<TEnt> ents, CancellationToken cancellationToken);

        /// <summary>
        /// Removes specified enitites
        /// 从存储库中移除指定的实体
        /// </summary>
        /// <param name="ents">Entities to remove 要移除的实体</param>
        /// <param name="cancellationToken">Cancellation token 取消令牌</param>
        void RemoveRange(IEnumerable<TEnt> ents, CancellationToken cancellationToken);

        /// <summary>
        /// Pre-creates template object
        /// 预创建模板对象（延迟提交）
        /// </summary>
        /// <typeparam name="T">Specific type of the template object 模板对象的具体类型</typeparam>
        /// <returns>Template object 模板对象</returns>
        /// <remarks>Use <see cref="IXTransaction.Commit(CancellationToken)"/> or <see cref="IXRepository{TEnt}.AddRange(IEnumerable{TEnt}, CancellationToken)"/> to commit templates and create objects</remarks>
        T PreCreate<T>() where T : TEnt;
    }

    /// <summary>
    /// Filter of the repository in the <see cref="IXRepository.Filter(bool, RepositoryFilterQuery[])"/>
    /// 用于 <see cref="IXRepository.Filter(bool, RepositoryFilterQuery[])"/> 的存储库筛选器
    /// </summary>
    public class RepositoryFilterQuery
    {
        /// <summary>
        /// Type of entity or null for all types
        /// 实体类型，null 表示所有类型
        /// </summary>
        public Type Type { get; set; }
    }
}