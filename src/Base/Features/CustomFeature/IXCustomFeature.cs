// -*- coding: utf-8 -*-
// src/Base/Features/CustomFeature/IXCustomFeature.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义自定义特征实例的接口，包含特征定义类型、变换矩阵、引用配置等属性。
// 支持带参数数据模型的泛型版本IXCustomFeature&lt;TParams&gt;。
//*********************************************************************

using System;
using Xarial.XCad.Documents;
using Xarial.XCad.Geometry.Structures;

namespace Xarial.XCad.Features.CustomFeature
{
     /// <summary>
     /// Instance of the custom feature
     /// 自定义特征实例
     /// </summary>
    public interface IXCustomFeature : IXFeature
    {
        /// <summary>
        /// Type of the definition of this custom feature
        /// 该自定义特征定义类型
        /// </summary>
        Type DefinitionType { get; set; }

        /// <summary>
        /// Transformation of this feature
        /// 该特征的目标变换矩阵
        /// </summary>
        /// <remarks>This is useful when the feature is inserted in the context of the assembly</remarks>
        TransformMatrix TargetTransformation { get; }

        /// <summary>
        /// Referenced configuration
        /// 引用配置
        /// </summary>
        IXConfiguration Configuration { get; }
    }

    /// <summary>
    /// Instance of the custom feature with parameters
    /// 带参数数据模型的自定义特征实例
    /// </summary>
    /// <typeparam name="TParams">Parameters data model</typeparam>
    public interface IXCustomFeature<TParams> : IXCustomFeature
        where TParams : class
    {
        /// <summary>
        /// Parameters of this feature
        /// 该特征参数
        /// </summary>
        TParams Parameters { get; set; }

        /// <summary>
        /// Gets the transformation matrix of the specified entity of the macro feature
        /// 获取宏特征中指定实体的变换矩阵
        /// </summary>
        /// <param name="entity">Entity to get the transformation from</param>
        /// <returns>Entity transformation matrix</returns>
        /// <remarks>Entity is a selection object which is specified in the <see cref="Parameters"/></remarks>
        TransformMatrix GetEntityTransformation(IXSelObject entity);
    }

    /// <summary>
    /// Additional methods for <see cref="IXCustomFeature"/>
    /// <see cref="IXCustomFeature"/> 扩展方法
    /// </summary>
    public static class XCustomFeatureExtension 
    {
        /// <summary>
        /// Gets the actual transformation of the entity in case of the in-context editing
        /// 在就地编辑场景下获取实体到目标的实际变换
        /// </summary>
        /// <typeparam name="TParams">Parameters</typeparam>
        /// <param name="feat">Custom feature</param>
        /// <param name="entity">Entity</param>
        /// <returns>Total transform</returns>
        /// <remarks>Use this method to transform the coordinates and vectors from the selection entities in the parameters to the custom feature target</remarks>
        public static TransformMatrix GetEntityToTargetTransformation<TParams>(this IXCustomFeature<TParams> feat, IXSelObject entity)
            where TParams : class
        {
            var entTransform = feat.GetEntityTransformation(entity);
            var targetTransform = feat.TargetTransformation.Inverse();

            return entTransform * targetTransform;
        }
    }
}