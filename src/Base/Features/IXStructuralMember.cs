// -*- coding: utf-8 -*-
// src/Base/Features/IXStructuralMember.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义焊件结构构件（Structural Member）特征的接口。
// 包含截面草图和构件分组集合，用于创建和管理焊接结构件。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xarial.XCad.Base;
using Xarial.XCad.Geometry;
using Xarial.XCad.Geometry.Structures;
using Xarial.XCad.Sketch;

namespace Xarial.XCad.Features
{
    public interface IXSructuralMemberGroupRepository : IXRepository<IXStructuralMemberGroup>
    {
    }

    public interface IXStructuralMemberGroup : IXTransaction
    {
        string Name { get; }
        IXSructuralMemberPieceRepository Pieces { get; }
    }

    public interface IXSructuralMemberPieceRepository : IXRepository<IXStructuralMemberPiece>
    {
    }

    public interface IXStructuralMemberPiece : IXTransaction
    {
        IXSolidBody Body { get; }
        IXSketchSegment[] Segments { get; }
        Plane ProfilePlane { get; }
    }

    /// <summary>
    /// Represents the weldment structural member feature
    /// 表示焊件结构构件（Structural Member）特征
    /// </summary>
    public interface IXStructuralMember : IXFeature
    {
        /// <summary>
        /// Profile sketch of the structural member
        /// 构件截面草图
        /// </summary>
        IXSketch2D Profile { get; }

        /// <summary>
        /// Groups of structural member pieces
        /// 结构构件分组集合
        /// </summary>
        IXSructuralMemberGroupRepository Groups { get; }
    }
}
