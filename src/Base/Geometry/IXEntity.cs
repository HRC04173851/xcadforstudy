//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System.Collections.Generic;
using Xarial.XCad.Documents;
using Xarial.XCad.Geometry.Structures;

namespace Xarial.XCad.Geometry
{
    /// <summary>
    /// Represents the base itnerface for gemetrical entities
    /// 表示几何实体的基础接口
    /// </summary>
    public interface IXEntity : IXSelObject
    {
        /// <summary>
        /// Gets the component associated with this entity in the context of the assembly
        /// 在装配体上下文中获取与该实体关联的组件
        /// </summary>
        /// <remarks>Null is returned if entity is not associated with the component (e.g. assembly level feature or entity is in the context of the part)</remarks>
        IXComponent Component { get; }

        /// <summary>
        /// Returns the body which owns this entity
        /// 返回拥有该实体的几何体（Body）
        /// </summary>
        IXBody Body { get; }

        /// <summary>
        /// Returns all adjacent entitites of this entity
        /// 返回与该实体拓扑相邻的所有实体
        /// </summary>
        IXEntityRepository AdjacentEntities { get; }

        /// <summary>
        /// Finds the closes point on the specified face
        /// 计算该实体上距离指定点最近的点
        /// </summary>
        /// <param name="point">Input point</param>
        /// <returns>Closest point</returns>
        Point FindClosestPoint(Point point);
    }

    /// <summary>
    /// Additional methods of <see cref="IXEntity"/>
    /// <see cref="IXEntity"/> 的扩展方法
    /// </summary>
    public static class XEntityExtension 
    {
        /// <summary>
        /// Returns the total transformation of this entity in the current context
        /// 返回实体在当前上下文中的总变换矩阵
        /// </summary>
        /// <param name="ent">Entity to get transformation for</param>
        /// <param name="context">Context document</param>
        /// <returns>Transformation</returns>
        /// <remarks>For the entity in the assembly context the transformation of the component is returned relatively to currently editing target</remarks>
        public static TransformMatrix GetRelativeTransform(this IXEntity ent, IXDocument context) 
        {
            var comp = ent.Component;

            TransformMatrix transform;

            if (comp != null)
            {
                transform = comp.Transformation;
            }
            else 
            {
                transform = TransformMatrix.Identity;
            }

            if (context is IXAssembly) 
            {
                var editComp = ((IXAssembly)context).EditingComponent;

                if (editComp != null)
                {
                    transform = transform.Multiply(editComp.Transformation.Inverse());
                }
            }

            return transform;
        }
    }
}