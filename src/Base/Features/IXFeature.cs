//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using Xarial.XCad.Annotations;
using Xarial.XCad.Base;
using Xarial.XCad.Documents;
using Xarial.XCad.Geometry;

namespace Xarial.XCad.Features
{
    /// <summary>
    /// Specifies the state of the feature
    /// <para>中文：指定特征的状态</para>
    /// </summary>
    [Flags]
    public enum FeatureState_e 
    {
        /// <summary>
        /// Default state
        /// <para>中文：默认状态</para>
        /// </summary>
        Default = 0,

        /// <summary>
        /// Feature is suppressed
        /// <para>中文：特征已被压缩</para>
        /// </summary>
        Suppressed = 1
    }

    /// <summary>
    /// Represents all features in the Feature Manager Design Tree
    /// <para>中文：表示特征管理器设计树中的所有特征</para>
    /// </summary>
    public interface IXFeature : IXSelObject, IXEntity, IHasColor, IDimensionable, IXTransaction, IHasName
    {
        /// <summary>
        /// Identifies if this feature is standard (soldered) or a user created
        /// <para>中文：标识此特征是标准（内置）特征还是用户创建的特征</para>
        /// </summary>
        bool IsUserFeature { get; }

        /// <summary>
        /// State of this feature in the feature tree
        /// <para>中文：特征树中此特征的状态</para>
        /// </summary>
        FeatureState_e State { get; set; }

        /// <summary>
        /// Enables feature editing mode
        /// <para>中文：启用特征编辑模式</para>
        /// </summary>
        /// <returns>Feature edtior</returns>
        IEditor<IXFeature> Edit();
    }

    /// <summary>
    /// Additional method of the <see cref="IXFeature"/>
    /// <para>中文：<see cref="IXFeature"/> 的附加扩展方法</para>
    /// </summary>
    public static class XFeatureExtension 
    {
        /// <summary>
        /// Iterates all bodies produced by this feature
        /// <para>中文：遍历此特征生成的所有实体</para>
        /// </summary>
        /// <param name="feat">Feature to iterate bodies</param>
        /// <returns>Bodies of the feture</returns>
        public static IEnumerable<IXBody> IterateBodies(this IXFeature feat)
        {
            var processedBodies = new List<IXBody>();

            foreach (var face in feat.AdjacentEntities.Filter<IXFace>())
            {
                var body = face.Body;

                if (!processedBodies.Any(b => b.Equals(body)))
                {
                    processedBodies.Add(body);
                    yield return body;
                }
            }
        }
    }
}