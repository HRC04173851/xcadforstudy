//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Xarial.XCad.Base.Enums;
using Xarial.XCad.SwDocumentManager.Documents;

namespace Xarial.XCad.SwDocumentManager
{
    /// <summary>
    /// Selectable xCAD object contract for Document Manager wrappers.
    /// Document Manager 包装器中的可选择对象约定；虽然接口存在，但大多数选择行为在离线 API 中并不支持。
    /// </summary>
    public interface ISwDmSelObject : ISwDmObject, IXSelObject 
    {
    
    }

    /// <summary>
    /// Base class for selectable entities such as components, sheets, and cut-list items.
    /// 组件、图纸页、切割清单项等可选择实体的基础类。
    /// </summary>
    internal class SwDmSelObject : SwDmObject, ISwDmSelObject
    {
        #region Not Supported
        
        public virtual void Commit(CancellationToken cancellationToken) => throw new NotSupportedException();
        public void Delete() => throw new NotSupportedException();
        public void Select(bool append) => throw new NotSupportedException();

        #endregion

        /// <summary>
        /// Initializes a selectable wrapper with its ownership context.
        /// 使用所属应用和文档上下文初始化一个可选择对象包装器。
        /// </summary>
        public SwDmSelObject(object disp, SwDmApplication ownerApp, SwDmDocument ownerDoc) : base(disp, ownerApp, ownerDoc)
        {
        }

        public bool IsSelected => throw new NotSupportedException();
        public virtual bool IsCommitted => true;
    }
}
