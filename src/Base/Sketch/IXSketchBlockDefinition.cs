//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Features;
using Xarial.XCad.Geometry.Structures;

namespace Xarial.XCad.Sketch
{
    /// <summary>
    /// Represents the defintion of <see cref="IXSketchBlockInstance"/>
    /// 表示 <see cref="IXSketchBlockInstance"/> 的块定义
    /// </summary>
    public interface IXSketchBlockDefinition : IXFeature
    {
        /// <summary>
        /// Insertion point of the sketch block definition
        /// 草图块定义的插入点
        /// </summary>
        Point InsertionPoint { get; }

        /// <summary>
        /// All instances of this sketch block defintion
        /// 该草图块定义的所有实例
        /// </summary>
        IEnumerable<IXSketchBlockInstance> Instances { get; }

        /// <summary>
        /// Entities of this sketch block definition
        /// 草图块定义包含的实体集合
        /// </summary>
        IXSketchEntityRepository Entities { get; }
    }
}
