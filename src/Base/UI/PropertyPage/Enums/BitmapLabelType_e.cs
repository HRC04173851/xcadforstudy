// -*- coding: utf-8 -*-
// src/Base/UI/PropertyPage/Enums/BitmapLabelType_e.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义标准控件位图标签类型，用于选择边、面、顶点等几何元素的标识符号
//*********************************************************************

namespace Xarial.XCad.UI.PropertyPage.Enums
{
    /// <summary>
    /// Standard control bitmap label types
    /// 标准控件位图标签类型
    /// </summary>
    public enum BitmapLabelType_e
    {
        LinearDistance = 1,
        AngularDistance,
        SelectEdgeFaceVertex,
        SelectFaceSurface,
        SelectVertex,
        SelectFace,
        SelectEdge,
        SelectFaceEdge,
        SelectComponent,
        Diameter,
        Radius,
        LinearDistance1,
        LinearDistance2,
        Thickness1,
        Thickness2,
        LinearPattern,
        CircularPattern,
        Width,
        Depth,
        KFactor,
        BendAllowance,
        BendDeduction,
        RipGap,
        SelectProfile,
        SelectBoundary
    }
}