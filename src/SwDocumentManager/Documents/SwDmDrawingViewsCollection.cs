// -*- coding: utf-8 -*-
// src/SwDocumentManager/Documents/SwDmDrawingViewsCollection.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 枚举某个图纸页上的全部工程图视图，将原始视图 COM 对象转换为 xCAD 工程图视图包装器。
//*********************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Documents;
using Xarial.XCad.Base;
using SolidWorks.Interop.swdocumentmgr;
using System.Linq;
using System.Threading;
using Xarial.XCad.Toolkit.Utils;
using Xarial.XCad.Documents.Delegates;

namespace Xarial.XCad.SwDocumentManager.Documents
{
    /// <summary>
    /// Repository contract for drawing views on a sheet.
    /// 图纸页上的工程图视图仓库约定。
    /// </summary>
    public interface ISwDmDrawingViewsCollection : IXDrawingViewRepository 
    {
    }

    /// <summary>
    /// Enumerates drawing views belonging to a specific sheet.
    /// 枚举某个图纸页上的全部工程图视图。
    /// </summary>
    internal class SwDmDrawingViewsCollection : ISwDmDrawingViewsCollection
    {
        #region Not Supported
        public event DrawingViewCreatedDelegate ViewCreated { add => throw new NotSupportedException(); remove => throw new NotSupportedException(); }
        public void AddRange(IEnumerable<IXDrawingView> ents, CancellationToken cancellationToken) => throw new NotSupportedException();
        public void RemoveRange(IEnumerable<IXDrawingView> ents, CancellationToken cancellationToken) => throw new NotSupportedException();
        public T PreCreate<T>() where T : IXDrawingView => throw new NotSupportedException();
        #endregion

        public IXDrawingView this[string name] => RepositoryHelper.Get(this, name);

        public int Count => (((ISwDMSheet4)m_Sheet.Sheet).GetViews() as object[] ?? new object[0]).Length;

        private readonly SwDmSheet m_Sheet;

        private readonly SwDmDrawing m_Drw;

        /// <summary>
        /// Creates a sheet-scoped view repository.
        /// 创建限定在单个图纸页范围内的视图仓库。
        /// </summary>
        internal SwDmDrawingViewsCollection(SwDmSheet sheet, SwDmDrawing drw) 
        {
            m_Sheet = sheet;
            m_Drw = drw;
        }

        /// <summary>
        /// Converts raw view COM objects into xCAD drawing view wrappers.
        /// 将原始视图 COM 对象转换为 xCAD 工程图视图包装器。
        /// </summary>
        public IEnumerator<IXDrawingView> GetEnumerator()
        {
            var views = ((ISwDMSheet4)m_Sheet.Sheet).GetViews() as object[] ?? new object[0];

            return views.Cast<ISwDMView>().Select(v => SwDmObjectFactory.FromDispatch<ISwDmDrawingView>(v, m_Drw)).GetEnumerator();
        }

        public bool TryGet(string name, out IXDrawingView ent)
        {
            ent = this.FirstOrDefault(v => string.Equals(v.Name, name, StringComparison.CurrentCultureIgnoreCase));
            return ent != null;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerable Filter(bool reverseOrder, params RepositoryFilterQuery[] filters) => RepositoryHelper.FilterDefault(this, filters, reverseOrder);
    }
}
