//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using Xarial.XCad.Geometry;
using Xarial.XCad.SwDocumentManager.Documents;

namespace Xarial.XCad.SwDocumentManager.Geometry
{
    /// <summary>
    /// Solid body contract.
    /// 实体体（Solid Body）约定。
    /// </summary>
    public interface ISwDmSolidBody : ISwDmBody, IXSolidBody 
    {
    }

    /// <summary>
    /// Solid body placeholder used for cut-list quantity expansion.
    /// 用于切割清单数量展开的实体体占位实现。
    /// </summary>
    internal class SwDmSolidBody : SwDmBody, ISwDmSolidBody
    {
        /// <summary>
        /// Creates a solid body wrapper owned by the specified part.
        /// 创建一个由指定零件拥有的实体体包装器。
        /// </summary>
        public SwDmSolidBody(SwDmPart part) : base(part)
        {
        }

        public double Volume => throw new NotSupportedException();
    }
}
