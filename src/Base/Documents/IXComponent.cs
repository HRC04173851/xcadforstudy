// -*- coding: utf-8 -*-
// src/Base/Documents/IXComponent.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件定义组件（Component）的跨CAD平台接口。
// Component 是装配体中的基本构建单元，代表对零件或子装配体的引用。
//
// Component 核心概念：
// 1. 引用关系：Component 引用外部文档（零件或装配体）
// 2. 变换矩阵：Component 有位置和旋转变换
// 3. 组件状态：压缩、轻量化、隐藏等
// 4. 层级关系：组件有父子层级关系（Parent/Children）
//
// 组件类型：
// - 零件组件：引用零件文档
// - 装配体组件：引用子装配体文档（形成递归层级）
//
// 组件状态：
// - Default：正常状态
// - Suppressed：压缩状态，不参与装配体计算
// - Hidden：隐藏状态，不可见
// - Lightweight：轻量化状态，延迟加载
//*********************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xarial.XCad.Annotations;
using Xarial.XCad.Base;
using Xarial.XCad.Documents.Enums;
using Xarial.XCad.Features;
using Xarial.XCad.Geometry;
using Xarial.XCad.Geometry.Structures;

namespace Xarial.XCad.Documents
{
    /// <summary>
    /// Represents components in the <see cref="IXAssembly"/>
    /// <para>中文：表示装配体中的组件</para>
    /// </summary>
    public interface IXComponent : IXSelObject, IXObjectContainer, IXTransaction, IHasColor, IDimensionable, IHasName
    {
        /// <summary>
        /// Full name of the component including the hierarchical path
        /// <para>中文：包含层次路径的组件完整名称</para>
        /// </summary>
        string FullName { get; }

        /// <summary>
        /// Reference label of the component
        /// <para>中文：组件的参考标签</para>
        /// </summary>
        string Reference { get; set; }

        /// <summary>
        /// Parent component of this component or null if root
        /// <para>中文：此组件的父组件，若为根组件则返回 null</para>
        /// </summary>
        IXComponent Parent { get; }

        /// <summary>
        /// Returns the referenced configuration of this component
        /// <para>中文：返回此组件所引用的配置</para>
        /// </summary>
        /// <remarks>For unloaded or rapid components this configuration may be uncommitted</remarks>
        IXConfiguration ReferencedConfiguration { get; set; }

        /// <summary>
        /// State of this component
        /// <para>中文：组件的状态（如压缩、轻化、隐藏等）</para>
        /// </summary>
        ComponentState_e State { get; set; }

        /// <summary>
        /// Document of the component
        /// <para>中文：组件所引用的三维文档</para>
        /// </summary>
        /// <remarks>If component is rapid, view only or suppressed document migth not be loaded into the memory. Use <see cref="IXTransaction.IsCommitted"/> to check the state and call <see cref="IXTransaction.Commit(System.Threading.CancellationToken)"/> to load document if needed
        /// When changing the referenced document of the committed element, document can be either replaced (if existing file is provided) or made independent if non-exisitng file is provided. Use an empty <see cref="IXDocument.Path"/> for the <see cref="ComponentState_e.Embedded"/> components
        /// </remarks>
        IXDocument3D ReferencedDocument { get; set; }
        
        /// <summary>
        /// Children components
        /// <para>中文：子组件集合</para>
        /// </summary>
        IXComponentRepository Children { get; }

        /// <summary>
        /// Features of this components
        /// <para>中文：此组件的特征集合</para>
        /// </summary>
        IXFeatureRepository Features { get; }

        /// <summary>
        /// Bodies in this component
        /// <para>中文：此组件中的实体集合</para>
        /// </summary>
        IXBodyRepository Bodies { get; }

        /// <summary>
        /// Transformation of this component in the assembly relative to the global coordinate system
        /// <para>中文：此组件相对于全局坐标系的变换矩阵</para>
        /// </summary>
        TransformMatrix Transformation { get; set; }

        /// <summary>
        /// Enables an editing mode for the component
        /// <para>中文：为组件启用编辑模式（关联编辑）</para>
        /// </summary>
        /// <returns>Component editor</returns>
        IEditor<IXComponent> Edit();
    }

    /// <summary>
    /// Specific component of the <see cref="IXPart"/>
    /// <para>中文：零件文档对应的专用组件接口</para>
    /// </summary>
    public interface IXPartComponent : IXComponent
    {
        /// <inheritdoc/>>
        new IXPart ReferencedDocument { get; set; }

        /// <inheritdoc/>>
        new IXPartConfiguration ReferencedConfiguration { get; set; }
    }

    /// <summary>
    /// Specific component of the <see cref="IXAssembly"/>
    /// <para>中文：装配体文档对应的专用组件接口</para>
    /// </summary>
    public interface IXAssemblyComponent : IXComponent
    {
        /// <inheritdoc/>>
        new IXAssembly ReferencedDocument { get; set; }

        /// <inheritdoc/>>
        new IXAssemblyConfiguration ReferencedConfiguration { get; set; }
    }

    /// <summary>
    /// Additional methods for <see cref="IXComponent"/>
    /// <para>中文：为 IXComponent 提供扩展方法的静态类</para>
    /// </summary>
    public static class XComponentExtension 
    {
        /// <summary>
        /// Iterates all bodies from the components
        /// <para>中文：遍历组件中的所有实体</para>
        /// </summary>
        /// <param name="comp">Component</param>
        /// <param name="includeHidden">True to include all bodies, false to only include visible</param>
        /// <returns>Bodies</returns>
        public static IEnumerable<IXBody> IterateBodies(this IXComponent comp, bool includeHidden = false)
            => IterateBodies(comp,
                c => includeHidden || !c.State.HasFlag(ComponentState_e.Hidden),
                b => includeHidden || b.Visible);
        
        /// <summary>
        /// Iterates all bodies from the component with the specified filter
        /// <para>中文：使用指定过滤条件遍历组件中的所有实体</para>
        /// </summary>
        /// <param name="comp">Component to get bodies from</param>
        /// <param name="compFilter">Filter for components</param>
        /// <param name="bodyFilter">Filter for bodies</param>
        /// <returns>Bodies enumeration</returns>
        public static IEnumerable<IXBody> IterateBodies(this IXComponent comp, Predicate<IXComponent> compFilter, Predicate<IXBody> bodyFilter)
        {
            IEnumerable<IXComponent> SelectComponents(IXComponent parent)
            {
                var state = parent.State;

                if (!state.HasFlag(ComponentState_e.Suppressed) && !state.HasFlag(ComponentState_e.SuppressedIdMismatch))
                {
                    if (compFilter.Invoke(parent))
                    {
                        yield return parent;

                        if (state.HasFlag(ComponentState_e.Lightweight))
                        {
                            if (parent.ReferencedDocument is IXAssembly)
                            {
                                parent.State = (ComponentState_e)(state - ComponentState_e.Lightweight);
                            }
                        }

                        foreach (var child in parent.Children.SelectMany(c => SelectComponents(c)))
                        {
                            yield return child;
                        }
                    }
                }
            }

            IXBody[] GetComponentBodies(IXComponent srcComp)
                => srcComp.Bodies.Where(b => bodyFilter.Invoke(b)).ToArray();

            foreach (var body in SelectComponents(comp)
                    .SelectMany(GetComponentBodies))
            {
                yield return body;
            }
        }

        /// <summary>
        /// Makes this component independent
        /// <para>中文：使此组件成为独立副本</para>
        /// </summary>
        /// <param name="comp">Component</param>
        /// <param name="newPath">New file path or an empty string for the embedded component</param>
        /// <exception cref="NotSupportedException"></exception>
        public static void MakeIndependent(this IXComponent comp, string newPath) 
        {
            if (!comp.IsCommitted) 
            {
                throw new NotSupportedException("Component is not committed");
            }

            if (comp.State.HasFlag(ComponentState_e.Embedded))
            {
                if (!string.IsNullOrEmpty(newPath))
                {
                    throw new NotSupportedException("Use empty path to make embedded component independent");
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(newPath))
                {
                    if (File.Exists(newPath))
                    {
                        throw new NotSupportedException($"File exists, use {nameof(ReplaceDocument)} function instead");
                    }
                }
                else
                {
                    throw new NotSupportedException("Empty path is only supported for embedded components");
                }
            }

            var newDoc = comp.OwnerApplication.Documents.PreCreate<IXDocument3D>();
            newDoc.Path = newPath;

            comp.ReferencedDocument = newDoc;
        }

        /// <summary>
        /// Replaces the reference document of this component
        /// <para>中文：替换此组件所引用的文档</para>
        /// </summary>
        /// <param name="comp">Component</param>
        /// <param name="newPath">Path to replace</param>
        /// <exception cref="NotSupportedException"></exception>
        public static void ReplaceDocument(this IXComponent comp, string newPath)
        {
            if (!comp.IsCommitted)
            {
                throw new NotSupportedException("Component is not committed");
            }

            if (!comp.State.HasFlag(ComponentState_e.Embedded))
            {
                if (!string.IsNullOrEmpty(newPath))
                {
                    if (!File.Exists(newPath))
                    {
                        throw new NotSupportedException($"File does not exists, use {nameof(MakeIndependent)} function instead");
                    }
                }
                else
                {
                    throw new NotSupportedException("Replacement path is not specified");
                }
            }
            else 
            {
                throw new NotSupportedException("Referenced document cannot be replaced for the embedded component");
            }

            var newDoc = comp.OwnerApplication.Documents.PreCreate<IXDocument3D>();
            newDoc.Path = newPath;

            comp.ReferencedDocument = newDoc;
        }
    }
}
