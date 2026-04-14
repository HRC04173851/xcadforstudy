// -*- coding: utf-8 -*-
// src/SwDocumentManager/Data/SwDm3rdPartyStorage.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 包装 SOLIDWORKS 文档中的第三方结构化存储（Storage），用于读写自定义二进制数据容器。
//*********************************************************************

using SolidWorks.Interop.swdocumentmgr;
using System;
using Xarial.XCad.Data.Enums;
using Xarial.XCad.Toolkit.Data;
using Xarial.XCad.Toolkit.Utils;

namespace Xarial.XCad.SolidWorks.Data
{
    /// <summary>
    /// Wraps a third-party structured storage inside a SOLIDWORKS document managed by Document Manager.
    /// 包装 SOLIDWORKS 文档中的第三方结构化存储（Storage），用于读写自定义二进制数据容器。
    /// </summary>
    internal class SwDm3rdPartyStorage : ComStorage
    {
        private readonly ISwDMDocument19 m_Doc;
        private readonly string m_Name;

        private readonly bool m_IsActive;

        /// <summary>
        /// Opens the named third-party storage and loads it into the generic COM storage adapter.
        /// 打开指定名称的第三方存储，并将其加载到通用 COM Storage 适配器中。
        /// </summary>
        internal SwDm3rdPartyStorage(ISwDMDocument19 doc, string name, AccessType_e access) 
            : base(AccessTypeHelper.GetIsWriting(access))
        {
            m_Doc = doc;
            m_Name = name;
            m_IsActive = false;

            try
            {
                var storage = m_Doc.Get3rdPartyStorageStore(name, AccessTypeHelper.GetIsWriting(access)) as IComStorage;

                if (storage != null)
                {
                    Load(storage);
                    m_IsActive = true;
                }
                else 
                {
                    throw new Exception("Storage doesn't exist");
                }
            }
            catch 
            {
                m_Doc.Release3rdPartyStorageStore(m_Name);
                throw;
            }
        }

        /// <summary>
        /// Releases the third-party storage handle back to Document Manager.
        /// 将第三方存储句柄释放回 Document Manager，避免文档保持占用状态。
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();

            if (m_IsActive)
            {
                if (!m_Doc.Release3rdPartyStorageStore(m_Name))
                {
                    throw new InvalidOperationException("Failed to release 3rd party storage store");
                }
            }
        }
    }
}
