// -*- coding: utf-8 -*-
// src/SolidWorks/Features/SwSketch3D.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 实现 3D 草图特征（Sketch3D）的封装，支持在三维空间中创建和编辑草图。
// SwSketch3D 继承自 SwSketchBase，与 2D 草图不同，3D 草图中的线条可以在
// 三维空间中任意方向绘制，主要用于扫描（Sweep）等需要空间曲线的特征。
//*********************************************************************

using SolidWorks.Interop.sldworks;
using System;
using Xarial.XCad.Features;
using Xarial.XCad.SolidWorks.Documents;

namespace Xarial.XCad.SolidWorks.Features
{
    public interface ISwSketch3D : ISwSketchBase, IXSketch3D
    {
    }

    internal class SwSketch3DEditor : SwSketchEditorBase<SwSketch3D>
    {
        public SwSketch3DEditor(SwSketch3D sketch, ISketch swSketch) : base(sketch, swSketch)
        {
        }

        protected override void StartEdit() => Target.OwnerDocument.Model.SketchManager.Insert3DSketch(true);
        protected override void EndEdit(bool cancel) => Target.OwnerDocument.Model.SketchManager.Insert3DSketch(!cancel);
    }

    internal class SwSketch3D : SwSketchBase, ISwSketch3D
    {
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
            OwnerModelDoc.ClearSelection2(true);
            OwnerModelDoc.Insert3DSketch2(true);
            return OwnerModelDoc.SketchManager.ActiveSketch;
        }
    }
}