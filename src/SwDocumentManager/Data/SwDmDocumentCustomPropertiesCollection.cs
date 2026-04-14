// -*- coding: utf-8 -*-
// src/SwDocumentManager/Data/SwDmDocumentCustomPropertiesCollection.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 直接存储在文档级别上的自定义属性仓库实现，支持文件级属性的增删改查操作。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Data;
using Xarial.XCad.SwDocumentManager.Documents;
using System.Linq;
using SolidWorks.Interop.swdocumentmgr;

namespace Xarial.XCad.SwDocumentManager.Data
{
    /// <summary>
    /// Repository of file-level custom properties stored directly on the document.
    /// 直接存储在文档级别上的自定义属性仓库。
    /// </summary>
    internal class SwDmDocumentCustomPropertiesCollection : SwDmCustomPropertiesCollection
    {
        public override int Count => m_Doc.Document.GetCustomPropertyCount();

        private readonly ISwDmDocument m_Doc;

        internal SwDmDocumentCustomPropertiesCollection(ISwDmDocument doc) 
        {
            m_Doc = doc;
        }

        /// <summary>
        /// Enumerates document properties except the internal quantity helper property.
        /// 枚举文档自定义属性，并排除内部使用的数量辅助属性。
        /// </summary>
        public override IEnumerator<IXProperty> GetEnumerator()
        {
            var prpNames = m_Doc.Document.GetCustomPropertyNames() as string[] ?? new string[0];
            prpNames = prpNames.Except(new string[] { SwDmConfiguration.QTY_PROPERTY }).ToArray();
            return prpNames.Select(p => CreatePropertyInstance(p, true)).GetEnumerator();
        }

        protected override ISwDmCustomProperty CreatePropertyInstance(string name, bool isCreated)
            => new SwDmDocumentCustomProperty(m_Doc, name, isCreated);

        protected override bool Exists(string name) 
            => (m_Doc.Document.GetCustomPropertyNames() as string[])?
            .Contains(name, StringComparer.CurrentCultureIgnoreCase) == true;
    }

    /// <summary>
    /// Concrete custom property bound to the document-level property table.
    /// 绑定到文档级属性表的具体自定义属性实现。
    /// </summary>
    internal class SwDmDocumentCustomProperty : SwDmCustomProperty
    {
        private readonly ISwDmDocument m_Doc;
        
        public SwDmDocumentCustomProperty(ISwDmDocument doc, string name, bool isCreated) : base(name, isCreated)
        {
            m_Doc = doc;
        }

        /// <summary>
        /// Adds a new document custom property and marks the file as modified.
        /// 向文档添加新的自定义属性，并把文件标记为已修改。
        /// </summary>
        protected override void AddValue(object value)
        {
            SwDmCustomInfoType type = GetPropertyType(value);

            if (!m_Doc.Document.AddCustomProperty(Name, type, value?.ToString()))
            {
                throw new Exception("Failed to add custom property");
            }

            m_Doc.IsDirty = true;
        }

        /// <summary>
        /// Reads the raw property value and linked expression from the document property manager.
        /// 从文档属性管理器读取原始属性值及其链接表达式。
        /// </summary>
        protected override string ReadRawValue(out SwDmCustomInfoType type, out string linkedTo)
            => ((ISwDMDocument5)m_Doc.Document).GetCustomPropertyValues(Name, out type, out linkedTo);

        /// <summary>
        /// Updates the stored string representation of the document custom property.
        /// 更新文档自定义属性保存的字符串值。
        /// </summary>
        protected override void SetValue(object value)
        {
            m_Doc.Document.SetCustomProperty(Name, value?.ToString());
            m_Doc.IsDirty = true;
        }

        /// <summary>
        /// Deletes the document custom property and marks the document dirty.
        /// 删除文档级自定义属性，并把文档标记为脏状态。
        /// </summary>
        internal override void Delete() 
        {
            if (!m_Doc.Document.DeleteCustomProperty(Name)) 
            {
                throw new Exception("Failed to delete property");
            }

            m_Doc.IsDirty = true;
        }
    }
}
