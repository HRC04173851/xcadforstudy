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
    /// Represents an instance of <see cref="IXSketchBlockDefinition"/>
    /// 表示 <see cref="IXSketchBlockDefinition"/> 的块实例
    /// </summary>
    public interface IXSketchBlockInstance : IXSketchEntity, IXFeature, IHasLayer
    {
        /// <summary>
        /// Definition of this sketch block instance
        /// 该草图块实例对应的块定义
        /// </summary>
        IXSketchBlockDefinition Definition { get; }

        /// <summary>
        /// Transformation of this sketch block instance regarding its defintion
        /// 该块实例相对于块定义的变换矩阵
        /// </summary>
        TransformMatrix Transform { get; }

        /// <summary>
        /// Entities of this sketch block definition
        /// 该块实例中的实体集合
        /// </summary>
        IXSketchEntityRepository Entities { get; }
    }

    /// <summary>
    /// Additional methods of <see cref="IXSketchBlockInstance"/>
    /// <see cref="IXSketchBlockInstance"/> 的扩展方法
    /// </summary>
    public static class XSketchBlockInstanceExtension 
    {
        /// <summary>
        /// Returns the total transform of this block, including parent block transforms
        /// 返回该块实例总变换（包含父块层级变换）
        /// </summary>
        /// <param name="skBlockInst"></param>
        /// <returns></returns>
        public static TransformMatrix GetTotalTransform(this IXSketchBlockInstance skBlockInst) 
        {
            var transform = TransformMatrix.Identity;

            while (skBlockInst != null) 
            {
                transform = transform.Multiply(skBlockInst.Transform);
                skBlockInst = skBlockInst.OwnerBlock;
            }

            return transform;
        }
    }
}
