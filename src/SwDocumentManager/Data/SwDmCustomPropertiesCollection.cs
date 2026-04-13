//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Xarial.XCad.Base;
using Xarial.XCad.Data;
using Xarial.XCad.Toolkit.Utils;

namespace Xarial.XCad.SwDocumentManager.Data
{
    /// <summary>
    /// Shared custom property repository contract for Document Manager objects.
    /// Document Manager 对象通用的自定义属性仓库约定。
    /// </summary>
    public interface ISwDmCustomPropertiesCollection : IXPropertyRepository 
    {
    }

    /// <summary>
    /// Base repository implementation for document, configuration, and cut-list custom properties.
    /// 文档、配置和切割清单自定义属性仓库的基础实现。
    /// </summary>
    internal abstract class SwDmCustomPropertiesCollection : ISwDmCustomPropertiesCollection
    {
        /// <summary>
        /// Indexes properties by name using xCAD repository helpers.
        /// 使用 xCAD 仓库辅助方法按属性名索引自定义属性。
        /// </summary>
        public IXProperty this[string name] => RepositoryHelper.Get(this, name);

        public void AddRange(IEnumerable<IXProperty> ents, CancellationToken cancellationToken) => RepositoryHelper.AddRange(ents, cancellationToken);

        /// <summary>
        /// Pre-creates a transient property wrapper that can be configured before commit.
        /// 预创建一个临时属性包装器，便于在提交前先配置名称和值。
        /// </summary>
        public T PreCreate<T>() where T : IXProperty => (T)CreatePropertyInstance("", false);

        /// <summary>
        /// Removes property wrappers by forwarding deletion to the concrete property implementation.
        /// 通过具体属性实现执行删除操作，从仓库中移除指定属性。
        /// </summary>
        public void RemoveRange(IEnumerable<IXProperty> ents, CancellationToken cancellationToken)
        {
            foreach (SwDmCustomProperty prp in ents) 
            {
                prp.Delete();
            }
        }

        /// <summary>
        /// Attempts to create a live property wrapper only when the property already exists in the source object.
        /// 仅当源对象中已存在该属性时，才创建对应的实时属性包装器。
        /// </summary>
        public bool TryGet(string name, out IXProperty ent)
        {
            if (Exists(name))
            {
                ent = CreatePropertyInstance(name, true);
                return true;
            }
            else
            {
                ent = null;
                return false;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerable Filter(bool reverseOrder, params RepositoryFilterQuery[] filters) => RepositoryHelper.FilterDefault(this, filters, reverseOrder);

        public abstract int Count { get; }
        public abstract IEnumerator<IXProperty> GetEnumerator();

        protected abstract bool Exists(string name);
        protected abstract ISwDmCustomProperty CreatePropertyInstance(string name, bool isCreated);
    }
}
