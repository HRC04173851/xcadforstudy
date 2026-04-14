// -*- coding: utf-8 -*-
// src/Base/Features/XFeatureRepositoryExtension.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// IXFeatureRepository接口的扩展方法类。
// 提供创建自定义特征、预创建草图、预创建哑实体等辅助功能。
//*********************************************************************

using Xarial.XCad.Base;
using Xarial.XCad.Features.CustomFeature;

namespace Xarial.XCad.Features
{
    /// <summary>
    /// Additional methods for <see cref="IXFeatureRepository"/>
    /// <see cref="IXFeatureRepository"/> 的扩展方法
    /// </summary>
    public static class XFeatureRepositoryExtension
    {
        /// <summary>
        /// Creates custom feature with specified parameters
        /// 使用指定参数创建自定义特征
        /// </summary>
        /// <typeparam name="TDef">Definition</typeparam>
        /// <typeparam name="TParams">Paramteres</typeparam>
        /// <param name="feats">Feature repository</param>
        /// <param name="param">Parameters</param>
        /// <returns>Instance of the custom feature</returns>
        public static IXCustomFeature<TParams> CreateCustomFeature<TDef, TParams>(this IXFeatureRepository feats, TParams param)
            where TParams : class
            where TDef : IXCustomFeatureDefinition<TParams>
        {
            var custFeat = feats.PreCreateCustomFeature<TParams>();
            custFeat.DefinitionType = typeof(TDef);
            custFeat.Parameters = param;
            feats.Add(custFeat);

            return custFeat;
        }

        /// <summary>
        /// Creates parameterlsess custom feature
        /// 创建无参数自定义特征
        /// </summary>
        /// <typeparam name="TDef">Defintion of the custom feature</typeparam>
        /// <param name="feats">Feature repository</param>
        /// <returns>Instance of the custom feature</returns>
        public static IXCustomFeature CreateCustomFeature<TDef>(this IXFeatureRepository feats)
            where TDef : IXCustomFeatureDefinition
        {
            var custFeat = feats.PreCreateCustomFeature();
            custFeat.DefinitionType = typeof(TDef);
            feats.Add(custFeat);

            return custFeat;
        }

        /// <summary>
        /// Starts the insertion of the custom feature with page editor
        /// 启动带页面编辑器的自定义特征插入流程
        /// </summary>
        /// <typeparam name="TDef">Defintion</typeparam>
        /// <typeparam name="TParams">Parameters</typeparam>
        /// <typeparam name="TPage">Page</typeparam>
        /// <param name="feats">Feature repository</param>
        public static void CreateCustomFeature<TDef, TParams, TPage>(this IXFeatureRepository feats)
            where TParams : class, new()
            where TPage : class
            where TDef : class, IXCustomFeatureDefinition<TParams, TPage>, new()
            => feats.CreateCustomFeature<TDef, TParams, TPage>(new TParams());

        /// <summary>
        /// Creates a template for 2D sketch
        /// 预创建二维草图模板
        /// </summary>
        /// <returns>2D sketch template</returns>
        public static IXSketch2D PreCreate2DSketch(this IXFeatureRepository feats) => feats.PreCreate<IXSketch2D>();

        /// <summary>
        /// Creates a template for 3D sketch
        /// 预创建三维草图模板
        /// </summary>
        /// <returns>2D sketch template</returns>
        public static IXSketch3D PreCreate3DSketch(this IXFeatureRepository feats) => feats.PreCreate<IXSketch3D>();

        /// <summary>
        /// Pre-creates custom feature
        /// 预创建自定义特征
        /// </summary>
        /// <returns>Instance of custom feature</returns>
        public static IXCustomFeature PreCreateCustomFeature(this IXFeatureRepository feats) => feats.PreCreate<IXCustomFeature>();

        /// <summary>
        /// Pre-creates dumb body feature
        /// 预创建哑实体特征
        /// </summary>
        /// <returns>Instance of dumb body feature</returns>
        public static IXDumbBody PreCreateDumbBody(this IXFeatureRepository feats) => feats.PreCreate<IXDumbBody>();

        /// <summary>
        /// Pre-creates custom feature with specific parameters
        /// </summary>
        /// <typeparam name="TParams">Type of parameters managed by this custom feature</typeparam>
        /// <returns>Instance of custom feature</returns>
        public static IXCustomFeature<TParams> PreCreateCustomFeature<TParams>(this IXFeatureRepository feats)
            where TParams : class
            => feats.PreCreate<IXCustomFeature<TParams>>();
    }
}