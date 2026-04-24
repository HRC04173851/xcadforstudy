// -*- coding: utf-8 -*-
// src/SwDocumentManager/Data/SwDmConfigurationCustomPropertiesCollection.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 配置特定（Configuration-specific）的自定义属性仓库实现，包括配置级属性的增删改查操作。
//*********************************************************************

using SolidWorks.Interop.swdocumentmgr;
using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Data;
using Xarial.XCad.SwDocumentManager.Documents;
using System.Linq;

namespace Xarial.XCad.SwDocumentManager.Data
{
    /// <summary>
    /// Repository of configuration-specific custom properties.
    /// 配置特定（Configuration-specific）的自定义属性仓库。
    /// </summary>
    internal class SwDmConfigurationCustomPropertiesCollection : SwDmCustomPropertiesCollection
    {
        public override int Count => m_Conf.Configuration.GetCustomPropertyCount();

        private readonly SwDmConfiguration m_Conf;

        internal SwDmConfigurationCustomPropertiesCollection(SwDmConfiguration conf)
        {
            m_Conf = conf;
        }

        /// <summary>
        /// Enumerates configuration properties except the internal quantity placeholder property.
        /// 枚举指定配置的自定义属性，并排除内部数量占位属性。
        /// </summary>
        public override IEnumerator<IXProperty> GetEnumerator()
        {
            var prpNames = m_Conf.Configuration.GetCustomPropertyNames() as string[] ?? new string[0];
            prpNames = prpNames.Except(new string[] { SwDmConfiguration.QTY_PROPERTY }).ToArray();

            return prpNames.Select(p => CreatePropertyInstance(p, true)).GetEnumerator();
        }

        protected override ISwDmCustomProperty CreatePropertyInstance(string name, bool isCreated)
            => new SwDmConfigurationCustomProperty(m_Conf, name, isCreated);

        protected override bool Exists(string name)
            // 使用 StringComparer.CurrentCultureIgnoreCase 进行不区分大小写的比较
            // == true 确保当 name 不存在于属性列表时返回 false（而不是 null），从而兼容 null-safe 调用
            => (m_Conf.Configuration.GetCustomPropertyNames() as string[])?
            .Contains(name, StringComparer.CurrentCultureIgnoreCase) == true;
    }

    /// <summary>
    /// Custom property implementation backed by a specific configuration.
    /// 由某个具体配置驱动的自定义属性实现。
    /// </summary>
    internal class SwDmConfigurationCustomProperty : SwDmCustomProperty
    {
        private readonly SwDmConfiguration m_Conf;

        internal SwDmConfigurationCustomProperty(SwDmConfiguration conf, string name, bool isCreated) 
            : base(name, isCreated)
        {
            m_Conf = conf;
        }

        /// <summary>
        /// Adds a new property to the active configuration record.
        /// 向当前配置记录添加新的自定义属性。
        /// </summary>
        protected override void AddValue(object value)
        {
            SwDmCustomInfoType type = GetPropertyType(value);

            if (!m_Conf.Configuration.AddCustomProperty(Name, type, value?.ToString()))
            {
                throw new Exception("Failed to add custom property");
            }

            m_Conf.Document.IsDirty = true;
        }

        /// <summary>
        /// Reads the raw property payload from the configuration property table.
        /// 从配置属性表读取原始属性数据。
        /// </summary>
        protected override string ReadRawValue(out SwDmCustomInfoType type, out string linkedTo)
            => ((ISwDMConfiguration5)m_Conf.Configuration).GetCustomPropertyValues(Name, out type, out linkedTo);

        /// <summary>
        /// Updates an existing configuration-specific property.
        /// 更新配置特定的现有自定义属性。
        /// </summary>
        protected override void SetValue(object value)
        {
            m_Conf.Configuration.SetCustomProperty(Name, value?.ToString());
            m_Conf.Document.IsDirty = true;
        }

        /// <summary>
        /// Deletes the configuration-specific property from the document model.
        /// 从文档模型中删除该配置特定属性。
        /// </summary>
        internal override void Delete()
        {
            if (!m_Conf.Configuration.DeleteCustomProperty(Name))
            {
                throw new Exception("Failed to delete property");
            }

            m_Conf.Document.IsDirty = true;
        }
    }
}
