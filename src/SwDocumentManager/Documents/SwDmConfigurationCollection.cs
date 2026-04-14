// -*- coding: utf-8 -*-
// src/SwDocumentManager/Documents/SwDmConfigurationCollection.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 零件与装配体共用的配置仓库逻辑实现，提供配置的查询、遍历和按名称解析功能。
//*********************************************************************

using SolidWorks.Interop.swdocumentmgr;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Xarial.XCad.Base;
using Xarial.XCad.Documents;
using Xarial.XCad.Documents.Delegates;
using Xarial.XCad.Exceptions;
using Xarial.XCad.SwDocumentManager.Exceptions;
using Xarial.XCad.Toolkit.Utils;

namespace Xarial.XCad.SwDocumentManager.Documents
{
    /// <summary>
    /// Repository contract for document configurations.
    /// 文档配置仓库约定。
    /// </summary>
    public interface ISwDmConfigurationCollection : IXConfigurationRepository 
    {
        new ISwDmConfiguration this[string name] { get; }
        new ISwDmConfiguration Active { get; }
    }

    /// <summary>
    /// Shared configuration repository logic for parts and assemblies.
    /// 零件与装配体共用的配置仓库逻辑。
    /// </summary>
    internal abstract class SwDmConfigurationCollection : ISwDmConfigurationCollection
    {
        #region Not Supported
        public event ConfigurationActivatedDelegate ConfigurationActivated 
        {
            add => throw new NotSupportedException();
            remove => throw new NotSupportedException();
        }
        public void AddRange(IEnumerable<IXConfiguration> ents, CancellationToken cancellationToken) => throw new NotSupportedException();
        public IXConfiguration PreCreate() => throw new NotSupportedException();
        public T PreCreate<T>() where T : IXConfiguration => throw new NotSupportedException();
        #endregion

        IXConfiguration IXRepository<IXConfiguration>.this[string name] => this[name];
        
        IXConfiguration IXConfigurationRepository.Active
        {
            get => Active;
            set => throw new NotSupportedException();
        }

        public ISwDmConfiguration this[string name] => (ISwDmConfiguration)RepositoryHelper.Get(this, name);

        /// <summary>
        /// Returns the active configuration resolved by the underlying configuration manager.
        /// 返回由底层配置管理器解析出的活动配置。
        /// </summary>
        public ISwDmConfiguration Active 
        {
            get 
            {
                var confName = m_Doc.Document.ConfigurationManager.GetActiveConfigurationName();

                if (string.IsNullOrEmpty(confName)) 
                {
                    throw new InvalidConfigurationsException("Name of the active configuration cannot be extracted");
                }

                return this[confName];
            }
        }

        /// <summary>
        /// Counts available configurations, using the newer API when version-aware errors are exposed.
        /// 统计可用配置数量；若版本支持，则使用可返回详细错误码的新 API。
        /// </summary>
        public int Count
        {
            get
            {
                if (m_Doc.IsVersionNewerOrEqual(SwDmVersion_e.Sw2019))
                {
                    var count = ((ISwDMConfigurationMgr2)m_Doc.Document.ConfigurationManager).GetConfigurationCount2(out SwDMConfigurationError err);
                    
                    if (err != SwDMConfigurationError.SwDMConfigurationError_None) 
                    {
                        throw new InvalidConfigurationsException(err);
                    }

                    return count;
                }
                else
                {
                    return m_Doc.Document.ConfigurationManager.GetConfigurationCount();
                }
            }
        }
         
        private readonly SwDmDocument3D m_Doc;
        private readonly Dictionary<ISwDMConfiguration, ISwDmConfiguration> m_ConfigurationsCache;

        /// <summary>
        /// Initializes the repository and its configuration cache.
        /// 初始化配置仓库以及内部配置缓存。
        /// </summary>
        internal SwDmConfigurationCollection(SwDmDocument3D doc) 
        {
            m_Doc = doc;
            m_ConfigurationsCache = new Dictionary<ISwDMConfiguration, ISwDmConfiguration>();
        }

        public IEnumerator<IXConfiguration> GetEnumerator()
            => GetConfigurationNames().Select(n => this[n]).GetEnumerator();

        /// <summary>
        /// Retrieves all configuration names from the model.
        /// 从模型中提取全部配置名称。
        /// </summary>
        protected string[] GetConfigurationNames()
        {
            string[] confNames;

            if (m_Doc.IsVersionNewerOrEqual(SwDmVersion_e.Sw2019))
            {
                confNames = (string[])((ISwDMConfigurationMgr2)m_Doc.Document.ConfigurationManager).GetConfigurationNames2(out SwDMConfigurationError err);

                if (err != SwDMConfigurationError.SwDMConfigurationError_None)
                {
                    throw new InvalidConfigurationsException(err);
                }
            }
            else
            {
                confNames = (string[])m_Doc.Document.ConfigurationManager.GetConfigurationNames();
            }

            return confNames;
        }

        /// <summary>
        /// Resolves a configuration by name and caches the wrapper instance.
        /// 按名称解析配置并缓存包装器实例。
        /// </summary>
        public bool TryGet(string name, out IXConfiguration ent)
        {
            ISwDMConfiguration conf;

            if (m_Doc.IsVersionNewerOrEqual(SwDmVersion_e.Sw2019))
            {
                conf = ((ISwDMConfigurationMgr2)m_Doc.Document.ConfigurationManager).GetConfigurationByName2(name, out SwDMConfigurationError err);

                if (err == SwDMConfigurationError.SwDMConfigurationError_NameNotFound) 
                {
                    ent = null;
                    return false;
                }

                if (err != SwDMConfigurationError.SwDMConfigurationError_None)
                {
                    throw new InvalidConfigurationsException(err);
                }
            }
            else
            {
                conf = m_Doc.Document.ConfigurationManager.GetConfigurationByName(name);
            }

            if (!m_ConfigurationsCache.TryGetValue(conf, out ISwDmConfiguration curConf))
            {
                curConf = SwDmObjectFactory.FromDispatch<ISwDmConfiguration>(conf, m_Doc);
                m_ConfigurationsCache.Add(conf, curConf);
            }

            ent = curConf;

            return true;
        }

        /// <summary>
        /// Marks configurations for deletion, except the active configuration.
        /// 将配置标记为删除，但不允许删除当前活动配置。
        /// </summary>
        public void RemoveRange(IEnumerable<IXConfiguration> ents, CancellationToken cancellationToken) 
        {
            var activeConfName = m_Doc.Document.ConfigurationManager.GetActiveConfigurationName();

            foreach (ISwDmConfiguration conf in ents) 
            {
                if (string.Equals(conf.Name, activeConfName, StringComparison.CurrentCultureIgnoreCase)) 
                {
                    throw new Exception("Cannot delete active configuration");
                }

                ((ISwDMConfiguration7)conf.Configuration).Delete = true;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerable Filter(bool reverseOrder, params RepositoryFilterQuery[] filters) => RepositoryHelper.FilterDefault(this, filters, reverseOrder);
    }

    /// <summary>
    /// Specialized repository contract for assembly configurations.
    /// 装配体配置仓库的专用约定。
    /// </summary>
    public interface ISwDmAssemblyConfigurationCollection : ISwDmConfigurationCollection, IXAssemblyConfigurationRepository 
    {
        new ISwDmAssemblyConfiguration this[string name] { get; }
        new ISwDmAssemblyConfiguration Active { get; set; }
    }

    /// <summary>
    /// Specialized repository contract for part configurations.
    /// 零件配置仓库的专用约定。
    /// </summary>
    public interface ISwDmPartConfigurationCollection : ISwDmConfigurationCollection, IXPartConfigurationRepository
    {
        new ISwDmPartConfiguration this[string name] { get; }
        new ISwDmPartConfiguration Active { get; set; }
    }

    /// <summary>
    /// Assembly configuration repository implementation.
    /// 装配体配置仓库实现。
    /// </summary>
    internal class SwDmAssemblyConfigurationCollection : SwDmConfigurationCollection, ISwDmAssemblyConfigurationCollection
    {
        private readonly SwDmAssembly m_Assm;

        internal SwDmAssemblyConfigurationCollection(SwDmAssembly assm) : base(assm)
        {
            m_Assm = assm;
        }
        
        ISwDmAssemblyConfiguration ISwDmAssemblyConfigurationCollection.this[string name] => (ISwDmAssemblyConfiguration)base[name];

        IXAssemblyConfiguration IXAssemblyConfigurationRepository.Active 
        {
            get => (ISwDmAssemblyConfiguration)base.Active;
            set => (this as IXConfigurationRepository).Active = value;
        }
        
        ISwDmAssemblyConfiguration ISwDmAssemblyConfigurationCollection.Active 
        {
            get => (ISwDmAssemblyConfiguration)base.Active;
            set => (this as IXConfigurationRepository).Active = value;
        }

        public void AddRange(IEnumerable<IXAssemblyConfiguration> ents, CancellationToken cancellationToken)
            => base.AddRange(ents, cancellationToken);

        public void RemoveRange(IEnumerable<IXAssemblyConfiguration> ents, CancellationToken cancellationToken)
            => base.RemoveRange(ents, cancellationToken);

        public bool TryGet(string name, out IXAssemblyConfiguration ent)
        {
            var res = base.TryGet(name, out IXConfiguration conf);
            ent = (IXAssemblyConfiguration)conf;
            return res;
        }

        IXAssemblyConfiguration IXAssemblyConfigurationRepository.PreCreate()
            => new SwDmAssemblyConfiguration(null, m_Assm);
    }

    /// <summary>
    /// Part configuration repository implementation.
    /// 零件配置仓库实现。
    /// </summary>
    internal class SwDmPartConfigurationCollection : SwDmConfigurationCollection, ISwDmPartConfigurationCollection
    {
        private readonly SwDmPart m_Part;

        internal SwDmPartConfigurationCollection(SwDmPart part) : base(part)
        {
            m_Part = part;
        }
        
        ISwDmPartConfiguration ISwDmPartConfigurationCollection.this[string name] => (ISwDmPartConfiguration)base[name];

        IXPartConfiguration IXPartConfigurationRepository.Active
        {
            get => (ISwDmPartConfiguration)base.Active;
            set => (this as IXConfigurationRepository).Active = value;
        }

        ISwDmPartConfiguration ISwDmPartConfigurationCollection.Active
        {
            get => (ISwDmPartConfiguration)base.Active;
            set => (this as IXConfigurationRepository).Active = value;
        }

        public void AddRange(IEnumerable<IXPartConfiguration> ents, CancellationToken cancellationToken)
            => base.AddRange(ents, cancellationToken);

        public void RemoveRange(IEnumerable<IXPartConfiguration> ents, CancellationToken cancellationToken)
            => base.RemoveRange(ents, cancellationToken);

        public bool TryGet(string name, out IXPartConfiguration ent)
        {
            var res = base.TryGet(name, out IXConfiguration conf);
            ent = (IXPartConfiguration)conf;
            return res;
        }

        IXPartConfiguration IXPartConfigurationRepository.PreCreate()
            => new SwDmPartConfiguration(null, m_Part);
    }
}
