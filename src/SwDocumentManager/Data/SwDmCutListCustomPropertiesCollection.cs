// -*- coding: utf-8 -*-
// src/SwDocumentManager/Data/SwDmCutListCustomPropertiesCollection.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 焊件或钣金实体对应的切割清单自定义属性仓库实现，支持按配置范围约束的属性操作。
//*********************************************************************

using SolidWorks.Interop.swdocumentmgr;
using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Data;
using Xarial.XCad.SwDocumentManager.Documents;
using System.Linq;
using Xarial.XCad.SwDocumentManager.Features;
using Xarial.XCad.SwDocumentManager.Exceptions;

namespace Xarial.XCad.SwDocumentManager.Data
{
    /// <summary>
    /// Repository of cut-list custom properties for weldment or sheet-metal bodies.
    /// 焊件或钣金实体对应的切割清单自定义属性仓库。
    /// </summary>
    internal class SwDmCutListCustomPropertiesCollection : SwDmCustomPropertiesCollection
    {
        public override int Count => (m_CutList.CutListItem.GetCustomPropertyNames() as string[])?.Length ?? 0;

        private readonly ISwDmCutListItem m_CutList;
        private readonly SwDmDocument3D m_Doc;
        private readonly ISwDmPartConfiguration m_Conf;

        internal SwDmCutListCustomPropertiesCollection(ISwDmCutListItem cutList, SwDmDocument3D doc, ISwDmPartConfiguration conf)
        {
            m_CutList = cutList;
            m_Doc = doc;
            m_Conf = conf;
        }

        /// <summary>
        /// Enumerates cut-list properties while skipping internal helper entries.
        /// 枚举切割清单属性，同时跳过内部辅助属性项。
        /// </summary>
        public override IEnumerator<IXProperty> GetEnumerator()
        {
            var prpNames = m_CutList.CutListItem.GetCustomPropertyNames() as string[] ?? new string[0];
            prpNames = prpNames.Except(new string[] { SwDmConfiguration.QTY_PROPERTY }).ToArray();
            return prpNames.Select(p => CreatePropertyInstance(p, true)).GetEnumerator();
        }

        protected override ISwDmCustomProperty CreatePropertyInstance(string name, bool isCreated)
            => new SwDmCutListCustomProperty(m_CutList, m_Doc, m_Conf, name, isCreated);

        protected override bool Exists(string name)
            => (m_CutList.CutListItem.GetCustomPropertyNames() as string[])?
            .Contains(name, StringComparer.CurrentCultureIgnoreCase) == true;
    }

    /// <summary>
    /// Custom property bound to a cut-list item and optionally scoped by configuration.
    /// 绑定到切割清单项目的自定义属性实现，并可按配置范围进行约束。
    /// </summary>
    internal class SwDmCutListCustomProperty : SwDmCustomProperty
    {
        private readonly ISwDmCutListItem m_CutList;
        private readonly SwDmDocument3D m_Doc;
        private readonly ISwDmConfiguration m_Conf;

        public SwDmCutListCustomProperty(ISwDmCutListItem cutList, SwDmDocument3D doc, ISwDmConfiguration conf, string name, bool isCreated) 
            : base(name, isCreated)
        {
            m_CutList = cutList;
            m_Doc = doc;
            m_Conf = conf;
        }

        /// <summary>
        /// Adds a cut-list property only when the target configuration is writable through Document Manager.
        /// 仅当目标配置可由 Document Manager 写入时，才允许新增切割清单属性。
        /// </summary>
        protected override void AddValue(object value)
        {
            if (m_Conf == null || m_Conf.Configuration == m_Doc.Configurations.Active.Configuration)
            {
                SwDmCustomInfoType type = GetPropertyType(value);

                if (!m_CutList.CutListItem.AddCustomProperty(Name, type, value?.ToString()))
                {
                    throw new Exception("Failed to add custom property");
                }
            }
            else
            {
                throw new ConfigurationSpecificCutListPropertiesWriteNotSupportedException();
            }

            m_Doc.IsDirty = true;
        }

        /// <summary>
        /// Reads the cut-list property raw value and the expression it is linked to.
        /// 读取切割清单属性的原始值，以及它所链接到的表达式文本。
        /// </summary>
        protected override string ReadRawValue(out SwDmCustomInfoType type, out string linkedTo)
            => m_CutList.CutListItem.GetCustomPropertyValue2(Name, out type, out linkedTo);

        /// <summary>
        /// Updates the cut-list property when the active configuration rules allow writing.
        /// 当活动配置规则允许写入时，更新切割清单属性。
        /// </summary>
        protected override void SetValue(object value)
        {
            if (m_Conf == null || m_Conf.Configuration == m_Doc.Configurations.Active.Configuration)
            {
                m_CutList.CutListItem.SetCustomProperty(Name, value?.ToString());
            }
            else 
            {
                throw new ConfigurationSpecificCutListPropertiesWriteNotSupportedException();
            }

            m_Doc.IsDirty = true;
        }

        /// <summary>
        /// Deletes the cut-list property while respecting configuration limitations of Document Manager.
        /// 在遵守 Document Manager 配置限制的前提下删除切割清单属性。
        /// </summary>
        internal override void Delete()
        {
            if (m_Conf == null || m_Conf.Configuration == m_Doc.Configurations.Active.Configuration)
            {
                if (!m_CutList.CutListItem.DeleteCustomProperty(Name))
                {
                    throw new Exception("Failed to delete property");
                }
            }
            else
            {
                throw new ConfigurationSpecificCutListPropertiesWriteNotSupportedException();
            }

            m_Doc.IsDirty = true;
        }
    }
}
