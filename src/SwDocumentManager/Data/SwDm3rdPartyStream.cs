// -*- coding: utf-8 -*-
// src/SwDocumentManager/Data/SwDm3rdPartyStream.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 包装 SOLIDWORKS 文档中的第三方数据流（Stream），常用于顺序读写插件自定义数据。
//*********************************************************************

using SolidWorks.Interop.swdocumentmgr;
using System;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using Xarial.XCad.Data.Enums;
using Xarial.XCad.Toolkit.Data;
using Xarial.XCad.Toolkit.Utils;

namespace Xarial.XCad.SolidWorks.Data
{
    /// <summary>
    /// Wraps a third-party stream inside a SOLIDWORKS document.
    /// 包装 SOLIDWORKS 文档中的第三方数据流（Stream），常用于顺序读写插件自定义数据。
    /// </summary>
    internal class SwDm3rdPartyStream : ComStream
    {
        private readonly ISwDMDocument19 m_Doc;
        private readonly string m_Name;
        private readonly bool m_IsActive;

        /// <summary>
        /// Opens the named third-party stream and positions the cursor at the beginning.
        /// 打开指定名称的第三方数据流，并把读写游标定位到流起始位置。
        /// </summary>
        internal SwDm3rdPartyStream(ISwDMDocument19 doc, string name, AccessType_e access) 
            : base(AccessTypeHelper.GetIsWriting(access), false)
        {
            m_Doc = doc;
            m_Name = name;
            m_IsActive = false;

            try
            {
                var stream = m_Doc.Get3rdPartyStorage(name, AccessTypeHelper.GetIsWriting(access)) as IStream;

                if (stream != null)
                {
                    Load(stream);
                    m_IsActive = true;
                }
                else 
                {
                    throw new Exception("Stream doesn't exist");
                }
            }
            catch 
            {
                m_Doc.Release3rdPartyStorage(m_Name);
                throw;
            }

            Seek(0, SeekOrigin.Begin);
        }

        /// <summary>
        /// Releases the stream back to Document Manager after the wrapper is disposed.
        /// 在包装器释放后，把底层流句柄归还给 Document Manager。
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (m_IsActive)
            {
                m_Doc.Release3rdPartyStorage(m_Name);
            }
        }
    }
}
