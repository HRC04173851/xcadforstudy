// -*- coding: utf-8 -*-
// src/Base/Features/IXFeature.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件定义特征的跨CAD平台接口。
// 特征（Feature）是 CAD 软件中构建模型的基本操作单元。
//
// 特征类型：
// - 基础特征：拉伸、旋转、倒角、圆角、孔等
// - 组合特征：放样、扫描、边界曲面等
// - 参考几何：基准面、基准轴、坐标系、原点等
// - 工程特征：螺纹、筋、槽等
// - 自定义特征：宏特性（Macro Feature）等第三方扩展
//
// 特征树（FeatureManager Tree）：
// CAD 模型以树形结构组织特征，父特征通常是子特征的几何基础。
// 例如：拉伸特征创建实体 → 倒角特征修改实体的边 → 圆角特征进一步修改
//
// 特征状态：
// - Default：正常状态，完全参与建模操作
// - Suppressed：压缩状态，在建模计算中忽略但不删除
//
// 特征编辑：
// 通过 IEditor&lt;IXFeature&gt; 接口实现特征的参数化编辑，
// 支持事务性操作：开始编辑 → 修改参数 → 确认/取消
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