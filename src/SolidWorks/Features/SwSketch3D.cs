//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using SolidWorks.Interop.sldworks;
using System;
using Xarial.XCad.Features;
using Xarial.XCad.SolidWorks.Documents;

namespace Xarial.XCad.SolidWorks.Features
{
    /// <summary>
    /// SolidWorks 3D 草图接口。
    /// 3D 草图在三维空间直接绘制，不依赖单一参考平面。
    /// </summary>
    public interface ISwSketch3D : ISwSketchBase, IXSketch3D
    {
    }

    /// <summary>
    /// 3D 草图编辑器：通过 Insert3DSketch API 进入和退出 3D 草图编辑模式。
    /// </summary>
    internal class SwSketch3DEditor : SwSketchEditorBase<SwSketch3D>
    {
        public SwSketch3DEditor(SwSketch3D sketch, ISketch swSketch) : base(sketch, swSketch)
        {
        }

        protected override void StartEdit() => Target.OwnerDocument.Model.SketchManager.Insert3DSketch(true);
        protected override void EndEdit(bool cancel) => Target.OwnerDocument.Model.SketchManager.Insert3DSketch(!cancel);
    }

    /// <summary>
    /// SolidWorks 3D 草图实现类。
    /// </summary>
    internal class SwSketch3D : SwSketchBase, ISwSketch3D
    {
        /// <summary>
        /// SolidWorks 3D 草图特征类型名（3DProfileFeature）。
        /// </summary>
        internal const string TypeName = "3DProfileFeature";

        internal SwSketch3D(IFeature feat, SwDocument doc, SwApplication app, bool created) : base(feat, doc, app, created)
        {
        }

        internal SwSketch3D(ISketch sketch, SwDocument doc, SwApplication app, bool created) : base(sketch, doc, app, created)
        {
        }

        protected internal override IEditor<IXSketchBase> CreateSketchEditor(ISketch sketch) => new SwSketch3DEditor(this, sketch);

        protected override ISketch CreateSketch()
        {
            //TODO: try to use API only selection
            // 中文：创建 3D 草图前先清空当前选择集，避免历史选择影响草图进入状态
            OwnerModelDoc.ClearSelection2(true);
            OwnerModelDoc.Insert3DSketch2(true);
            return OwnerModelDoc.SketchManager.ActiveSketch;
        }
    }
}