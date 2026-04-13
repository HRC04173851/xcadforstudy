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
using Xarial.XCad.Documents;
using Xarial.XCad.Base;
using SolidWorks.Interop.swdocumentmgr;
using System.Linq;
using Xarial.XCad.SwDocumentManager.Services;
using System.IO;
using Xarial.XCad.Toolkit.Exceptions;
using Xarial.XCad.Exceptions;
using System.Threading;
using Xarial.XCad.Toolkit.Utils;
using Xarial.XCad.Documents.Delegates;
using Xarial.XCad.SwDocumentManager.Exceptions;

namespace Xarial.XCad.SwDocumentManager.Documents
{
    /// <summary>
    /// Repository of assembly components exposed by Document Manager.
    /// 由 Document Manager 暴露的装配体组件仓库。
    /// </summary>
    public interface ISwDmComponentCollection : IXComponentRepository 
    {
    }

    /// <summary>
    /// Enumerates and caches components for an assembly configuration.
    /// 按装配体配置枚举并缓存组件，用于构建离线组件树。
    /// </summary>
    internal class SwDmComponentCollection : ISwDmComponentCollection
    {
        #region Not Supported
        public void AddRange(IEnumerable<IXComponent> ents, CancellationToken cancellationToken) => throw new NotSupportedException();
        public void RemoveRange(IEnumerable<IXComponent> ents, CancellationToken cancellationToken) => throw new NotSupportedException();
        public T PreCreate<T>() where T : IXComponent => throw new NotSupportedException();
        #endregion

        private readonly ISwDmConfiguration m_Conf;
        private readonly SwDmAssembly m_ParentAssm;

        private IFilePathResolver m_PathResolver;

        private readonly Dictionary<string, SwDmComponent> m_ComponentsCache;

        /// <summary>
        /// Creates a component repository for a specific assembly configuration.
        /// 为指定装配体配置创建组件仓库。
        /// </summary>
        internal SwDmComponentCollection(SwDmAssembly parentAssm, ISwDmConfiguration conf)
        {
            m_ParentAssm = parentAssm;
            m_Conf = conf;

            m_PathResolver = new SwDmFilePathResolver();
            m_ComponentsCache = new Dictionary<string, SwDmComponent>(StringComparer.CurrentCultureIgnoreCase);
        }

        public IXComponent this[string name] => RepositoryHelper.Get(this, name);

        /// <summary>
        /// Counts immediate child components of the current configuration.
        /// 统计当前配置下的一级子组件数量。
        /// </summary>
        public int Count
        {
            get
            {
                ValidateSpeedPak(m_Conf.Configuration);
                return GetComponents(m_Conf.Configuration).Length;
            }
        }

        /// <summary>
        /// Counts all descendant components recursively, including nested subassemblies.
        /// 递归统计全部后代组件数量，包括子装配体中的嵌套组件。
        /// </summary>
        public int TotalCount 
        {
            get 
            {
                ValidateSpeedPak(m_Conf.Configuration);

                var totalCount = 0;

                var cachedCount = new Dictionary<string, int>(StringComparer.CurrentCultureIgnoreCase);

                CountComponents(m_Conf.Configuration, cachedCount, ref totalCount);

                return totalCount;
            }
        }

        /// <summary>
        /// Reads the raw Document Manager component array for a configuration.
        /// 读取指定配置对应的原始 Document Manager 组件数组。
        /// </summary>
        private object[] GetComponents(ISwDMConfiguration conf) 
        {
            ValidateSpeedPak(conf);
            return ((ISwDMConfiguration2)conf).GetComponents() as object[] ?? new object[0];
        }

        /// <summary>
        /// Rejects SpeedPak configurations because Document Manager cannot expose a complete component tree for them.
        /// 拒绝处理 SpeedPak 配置，因为 Document Manager 无法为其提供完整的组件树。
        /// </summary>
        private void ValidateSpeedPak(ISwDMConfiguration conf) 
        {
            if (((ISwDMConfiguration11)conf).IsSpeedPak())
            {
                throw new SpeedPakConfigurationComponentsException();
            }
        }
        
        /// <summary>
        /// Recursively traverses subassemblies to accumulate a full component count.
        /// 递归遍历子装配体，累计完整组件数量。
        /// </summary>
        private void CountComponents(ISwDMConfiguration conf, Dictionary<string, int> cachedCount, ref int totalCount) 
        {
            foreach (ISwDMComponent6 comp in GetComponents(conf))
            {
                totalCount++;

                if (comp.DocumentType == SwDmDocumentType.swDmDocumentAssembly && !comp.IsSuppressed())
                {
                    try
                    {
                        var path = m_PathResolver.ResolvePath(Path.GetDirectoryName(m_ParentAssm.Path), comp.PathName);

                        var confName = comp.ConfigurationName;

                        var cacheKey = $"{path}:{confName}";
                        
                        if (cachedCount.TryGetValue(cacheKey, out int count))
                        {
                            totalCount += count;
                        }
                        else
                        {
                            if (File.Exists(path))
                            {
                                int subTotalCount = 0;

                                var subAssm = m_ParentAssm.OwnerApplication.SwDocMgr
                                    .GetDocument(path, SwDmDocumentType.swDmDocumentAssembly, true, out SwDmDocumentOpenError err);

                                try
                                {
                                    if (subAssm != null)
                                    {
                                        var subConf = subAssm.ConfigurationManager.GetConfigurationByName(confName);

                                        if (subConf != null)
                                        {
                                            CountComponents(subConf, cachedCount, ref subTotalCount);
                                        }
                                    }

                                    totalCount += subTotalCount;
                                    cachedCount.Add(cacheKey, subTotalCount);
                                }
                                finally 
                                {
                                    subAssm?.CloseDoc();
                                }
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            }
        }

        private IEnumerable<ISwDMComponent> IterateDmComponents() 
        {
            if (m_Conf.IsCommitted)
            {
                //if the parent document was closed calling the below method will open the document into the memory
                // 如果父文档已关闭，下面的调用会把文档重新加载到内存中。
                return GetComponents(m_Conf.Configuration).Cast<ISwDMComponent>();
            }
            else
            {
                throw new NonCommittedElementAccessException();
            }
        }

        public IEnumerator<IXComponent> GetEnumerator()
            => IterateDmComponents()
            .Select(CreateComponentInstance)
            .GetEnumerator();

        public IEnumerable Filter(bool reverseOrder, params RepositoryFilterQuery[] filters) => RepositoryHelper.FilterDefault(this, filters, reverseOrder);

        /// <summary>
        /// Creates or reuses a cached strongly typed component wrapper.
        /// 创建或复用已缓存的强类型组件包装器。
        /// </summary>
        protected virtual SwDmComponent CreateComponentInstance(ISwDMComponent dmComp) 
        {
            var compName = ((ISwDMComponent7)dmComp).Name2;

            if (!m_ComponentsCache.TryGetValue(compName, out SwDmComponent comp))
            {
                comp = SwDmObjectFactory.FromDispatch<SwDmComponent>(dmComp, m_ParentAssm);
                m_ComponentsCache.Add(compName, comp);
            }

            return comp;
        }

        public bool TryGet(string name, out IXComponent ent)
        {
            var comp = IterateDmComponents().FirstOrDefault(c => string.Equals(((ISwDMComponent7)c).Name2,
                name, StringComparison.CurrentCultureIgnoreCase));

            if (comp != null)
            {
                ent = CreateComponentInstance(comp);
                return true;
            }
            else
            {
                ent = null;
                return false;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
