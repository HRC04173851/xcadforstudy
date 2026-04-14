// -*- coding: utf-8 -*-
// src/Base/UI/PropertyPage/Enums/BitmapButtonLabelType_e.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义位图按钮的标准标签和图标类型，如直径、距离、角度等几何参数标识
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.UI.PropertyPage.Enums
{
    /// <summary>
    /// Standard bitmap button label/icon identifiers
    /// 位图按钮标准标签/图标标识
    /// </summary>
    public enum BitmapButtonLabelType_e
    {
        AlongZ = 1,
        Angle = 2,
        AutoBalCircular = 3,
        AutoBalLeft = 4,
        AutoBalRight = 5,
        AutoBalSquare = 6,
        AutoBalTop = 7,
        Diameter = 8,
        Distance1 = 9,
        Distance2 = 10,
        Draft = 11,
        DveButCmarkBolt = 12,
        DveButCmarkLinear = 13,
        DveButCmarkSingle = 14,
        LeaderAngAbove = 15,
        LeaderAngBeside = 16,
        LeaderHorAbove = 17,
        LeaderHorBeside = 18,
        LeaderLeft = 19,
        LeaderNo = 20,
        LeaderRight = 21,
        LeaderYes = 22,
        Parallel = 23,
        Perpendicular = 24,
        ReverseDirection = 25,
        RevisionCircle = 26,
        RevisionHexagon = 27,
        RevisionSquare = 28,
        RevisionTriangle = 29,
        StackLeft = 30,
        StackRight = 31,
        StackUp = 32,
        Stack = 33,
        FavoriteAdd = 34,
        favoriteDelete = 35,
        FavoriteSave = 36,
        FavoriteLoad = 37,
        DimensionSetDefaultAttributes = 38
    }
}
