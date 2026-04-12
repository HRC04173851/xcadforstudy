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
using Xarial.XCad.Base.Enums;
using Xarial.XCad.Documents;
using Xarial.XCad.Geometry;
using Xarial.XCad.Geometry.Structures;
using Xarial.XCad.SwDocumentManager.Documents;

namespace Xarial.XCad.SwDocumentManager.Geometry
{
    /// <summary>
    /// Base body contract for Document Manager geometry wrappers.
    /// Document Manager 几何包装器中的基础实体体约定。
    /// </summary>
    public interface ISwDmBody : IXBody, ISwDmObject
    {
    }

    /// <summary>
    /// Minimal body implementation used when Document Manager can expose a body concept but not full B-Rep details.
    /// 当 Document Manager 只能暴露“实体体”概念、却无法提供完整 B-Rep 拓扑时使用的最小实现。
    /// </summary>
    internal abstract class SwDmBody : SwDmSelObject, ISwDmBody
    {
        #region Not Supported
        public string Name => throw new NotSupportedException();
        public bool Visible { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
        public Color? Color { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
        public IEnumerable<IXFace> Faces => throw new NotSupportedException();
        public IEnumerable<IXEdge> Edges => throw new NotSupportedException();
        public IXComponent Component => throw new NotSupportedException();
        public IXMemoryBody Add(IXMemoryBody other) => throw new NotSupportedException();
        public IXMemoryBody[] Common(IXMemoryBody other) => throw new NotSupportedException();
        public IXMemoryBody[] Substract(IXMemoryBody other) => throw new NotSupportedException();
        public IXMemoryBody Copy() => throw new NotSupportedException();
        public void Transform(TransformMatrix transform) => throw new NotSupportedException();
        public IXMaterial Material { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
        #endregion

        /// <summary>
        /// Associates the body wrapper with the owning part document.
        /// 将实体体包装器关联到所属零件文档。
        /// </summary>
        public SwDmBody(SwDmPart part) : base(null, part.OwnerApplication, part)
        {
        }
    }
}
