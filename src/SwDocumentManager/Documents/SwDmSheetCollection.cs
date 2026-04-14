// -*- coding: utf-8 -*-
// src/SwDocumentManager/Documents/SwDmSheetCollection.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 枚举并解析工程图文档中的全部图纸页，提供图纸页的查询、激活状态和自定义枚举器实现。
//*********************************************************************

using SolidWorks.Interop.swdocumentmgr;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xarial.XCad.Documents;
using Xarial.XCad.Documents.Delegates;
using Xarial.XCad.Base;
using System.Threading;
using Xarial.XCad.Toolkit.Utils;

namespace Xarial.XCad.SwDocumentManager.Documents
{
    /// <summary>
    /// Repository contract for drawing sheets.
    /// 工程图图纸页仓库约定。
    /// </summary>
    public interface ISwDmSheetCollection : IXSheetRepository 
    {
    }

    /// <summary>
    /// Enumerates and resolves sheets contained in a drawing document.
    /// 枚举并解析工程图文档中的全部图纸页。
    /// </summary>
    internal class SwDmSheetCollection : ISwDmSheetCollection
    {
        #region Not Supported
        public event SheetActivatedDelegate SheetActivated { add => throw new NotSupportedException(); remove => throw new NotSupportedException(); }
        public event SheetCreatedDelegate SheetCreated { add => throw new NotSupportedException(); remove => throw new NotSupportedException(); }
        public void AddRange(IEnumerable<IXSheet> ents, CancellationToken cancellationToken) => throw new NotSupportedException();
        public void RemoveRange(IEnumerable<IXSheet> ents, CancellationToken cancellationToken) => throw new NotSupportedException();
        public T PreCreate<T>() where T : IXSheet => throw new NotSupportedException();
        #endregion

        private readonly SwDmDrawing m_Drw;

        /// <summary>
        /// Creates a sheet repository for the specified drawing.
        /// 为指定工程图创建图纸页仓库。
        /// </summary>
        internal SwDmSheetCollection(SwDmDrawing drw) 
        {
            m_Drw = drw;
        }

        public IXSheet this[string name]  => RepositoryHelper.Get(this, name);

        /// <summary>
        /// Returns the active sheet recorded in the drawing document.
        /// 返回工程图当前记录的活动图纸页。
        /// </summary>
        public IXSheet Active 
        {
            get 
            {
                var activeSheetName = (m_Drw.Document as ISwDMDocument10).GetActiveSheetName();
                return this[activeSheetName];
            }
            set => throw new NotSupportedException();
        }

        public int Count => (m_Drw.Document as ISwDMDocument10).GetSheetCount();

        public IEnumerator<IXSheet> GetEnumerator() => new SwDmSheetEnumerator(m_Drw);

        public IEnumerable Filter(bool reverseOrder, params RepositoryFilterQuery[] filters) => RepositoryHelper.FilterDefault(this, filters, reverseOrder);

        public bool TryGet(string name, out IXSheet ent)
        {
            ent = this.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.CurrentCultureIgnoreCase));
            return ent != null;
        }

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }

    /// <summary>
    /// Custom enumerator over drawing sheets.
    /// 工程图图纸页的自定义枚举器。
    /// </summary>
    internal class SwDmSheetEnumerator : IEnumerator<IXSheet>
    {
        public IXSheet Current
            => SwDmObjectFactory.FromDispatch<ISwDmSheet>(m_Sheets[m_CurSheetIndex], m_Drw);

        object IEnumerator.Current => Current;

        private int m_CurSheetIndex;

        private readonly SwDmDrawing m_Drw;

        private ISwDMSheet[] m_Sheets;

        /// <summary>
        /// Captures the raw sheet COM objects once at enumerator construction time.
        /// 在枚举器构造时一次性抓取原始图纸页 COM 对象。
        /// </summary>
        internal SwDmSheetEnumerator(SwDmDrawing drw)
        {
            m_Drw = drw;

            m_CurSheetIndex = -1;
            m_Sheets = (((ISwDMDocument10)m_Drw.Document).GetSheets() as object[])?.Cast<ISwDMSheet>().ToArray();
        }

        public bool MoveNext()
        {
            m_CurSheetIndex++;
            return m_CurSheetIndex < m_Sheets.Length;
        }

        public void Reset()
        {
            m_CurSheetIndex = -1;
        }

        public void Dispose()
        {
        }
    }
}
