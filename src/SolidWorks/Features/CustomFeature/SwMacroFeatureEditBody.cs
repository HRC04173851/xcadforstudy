// -*- coding: utf-8 -*-
// src/SolidWorks/Features/CustomFeature/SwMacroFeatureEditBody.cs

//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xarial.XCad.Documents;
using Xarial.XCad.Geometry;
using Xarial.XCad.SolidWorks.Documents;
using Xarial.XCad.SolidWorks.Geometry;
using Xarial.XCad.SolidWorks.Utils;

namespace Xarial.XCad.SolidWorks.Features.CustomFeature
{
    /// <summary>
    /// 宏特性编辑体接口。
    /// <para>
    /// 表示在宏特性中被编辑的几何体（使用 <see cref="XCad.Features.CustomFeature.Attributes.ParameterEditBodyAttribute"/> 标记）。
    /// 使用 <see cref="SwMacroFeatureDefinition.CreateEditBody(IBody2, ISwDocument, ISwApplication, bool)"/> 创建实例。
    /// </para>
    /// </summary>
    /// <remarks>
    /// 重建过程中使用的几何体是临时体和永久体的混合体（例如，支持与临时体的布尔运算）。
    /// 为确保在预览模式中的一致访问，添加了 <see cref="ISwMacroFeatureEditBody"/> 接口，
    /// 它同时表示预览模式下的临时体和重建时的永久体。
    /// </remarks>
    internal interface ISwMacroFeatureEditBody : ISwTempBody
    {
        /// <summary>
        /// 预览模式下的几何体副本
        /// </summary>
        IBody2 PreviewBody { get; }

        /// <summary>
        /// 是否处于预览模式
        /// </summary>
        bool IsPreviewMode { get; }
    }

    /// <summary>
    /// 宏特性编辑体工具类
    /// <para>提供编辑体的布尔运算和创建方法</para>
    /// </summary>
    internal static class SwMacroFeatureEditBody
    {
        /// <summary>
        /// 执行加运算（合并）
        /// </summary>
        /// <param name="editBody">编辑体</param>
        /// <param name="other">另一个几何体</param>
        /// <returns>运算结果</returns>
        internal static ISwTempBody PerformAdd(this ISwMacroFeatureEditBody editBody, ISwTempBody other)
            => SwTempBodyHelper.Add(ProvideBooleanOperationBody(editBody), ((SwBody)other).Body, (SwApplication)editBody.OwnerApplication, (SwDocument)editBody.OwnerDocument,
                b => CreateTempBodyBooleanOperationResult(b, (SwDocument)editBody.OwnerDocument, (SwApplication)editBody.OwnerApplication, editBody.IsPreviewMode));

        /// <summary>
        /// 执行减运算（切除）
        /// </summary>
        internal static ISwTempBody[] PerformSubstract(this ISwMacroFeatureEditBody editBody, ISwTempBody other)
            => SwTempBodyHelper.Substract(ProvideBooleanOperationBody(editBody), ((SwBody)other).Body, (SwApplication)editBody.OwnerApplication, (SwDocument)editBody.OwnerDocument,
                b => CreateTempBodyBooleanOperationResult(b, (SwDocument)editBody.OwnerDocument, (SwApplication)editBody.OwnerApplication, editBody.IsPreviewMode));

        /// <summary>
        /// 执行交运算（交集）
        /// </summary>
        internal static ISwTempBody[] PerformCommon(this ISwMacroFeatureEditBody editBody, ISwTempBody other)
            => SwTempBodyHelper.Common(ProvideBooleanOperationBody(editBody), ((SwBody)other).Body, (SwApplication)editBody.OwnerApplication, (SwDocument)editBody.OwnerDocument,
                b => CreateTempBodyBooleanOperationResult(b, (SwDocument)editBody.OwnerDocument, (SwApplication)editBody.OwnerApplication, editBody.IsPreviewMode));

        /// <summary>
        /// 创建布尔运算结果的临时体
        /// </summary>
        /// <remarks>
        /// 根据结果几何体是临时体还是永久体，返回不同类型的包装器。
        /// 如果是临时体，直接从 dispatch 创建；否则创建编辑体包装器。
        /// </remarks>
        private static ISwTempBody CreateTempBodyBooleanOperationResult(IBody2 body, SwDocument doc, SwApplication app, bool isPreview)
        {
            if (body.IsTemporaryBody())
            {
                return doc.CreateObjectFromDispatch<ISwTempBody>(body);
            }
            else
            {
                return (ISwTempBody)SwMacroFeatureDefinition.CreateEditBody(body, doc, app, isPreview);
            }
        }

        /// <summary>
        /// 为布尔运算提供正确的几何体
        /// </summary>
        /// <remarks>
        /// 预览模式下使用预览体副本，正式重建时使用永久体。
        /// 这确保了预览体验的流畅性，同时保持最终几何的正确性。
        /// </remarks>
        private static IBody2 ProvideBooleanOperationBody(ISwMacroFeatureEditBody editBody)
        {
            if (editBody.IsPreviewMode)
            {
                return editBody.PreviewBody;
            }
            else
            {
                return editBody.Body;
            }
        }
    }

    /// <summary>
    /// 惰性预览体
    /// <para>延迟创建几何体副本，仅在预览模式下需要时创建</para>
    /// </summary>
    internal class LazyMacroFeaturePreviewBody : Lazy<IBody2>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="body">原始几何体</param>
        /// <param name="isPreview">是否预览模式</param>
        /// <param name="app">应用程序</param>
        internal LazyMacroFeaturePreviewBody(IBody2 body, bool isPreview, SwApplication app)
            : base(() =>
            {
                if (isPreview)
                {
                    // 预览模式下创建几何体副本
                    return body.CreateCopy(app);
                }
                else
                {
                    throw new Exception("Macro feature body is not in a preview state");
                }
            })
        {

        }
    }

    /// <summary>
    /// 平面片体宏特性编辑体
    /// <para>用于钣金展开等平面片体特征的编辑</para>
    /// </summary>
    internal class SwPlanarSheetMacroFeatureEditBody : SwPlanarSheetBody, ISwMacroFeatureEditBody, ISwTempPlanarSheetBody
    {
        IXMemoryBody IXMemoryBody.Add(IXMemoryBody other) => Add((ISwTempBody)other);
        IXMemoryBody[] IXMemoryBody.Substract(IXMemoryBody other) => Substract((ISwTempBody)other);
        IXMemoryBody[] IXMemoryBody.Common(IXMemoryBody other) => Common((ISwTempBody)other);

        public bool IsPreviewMode { get; }

        public IBody2 PreviewBody => m_PreviewBodyLazy.Value;

        private Lazy<IBody2> m_PreviewBodyLazy;

        internal SwPlanarSheetMacroFeatureEditBody(IBody2 body, SwDocument doc, SwApplication app, bool isPreview) : base(body, doc, app)
        {
            IsPreviewMode = isPreview;
            m_PreviewBodyLazy = new LazyMacroFeaturePreviewBody(body, isPreview, app);
        }

        public ISwTempBody Add(ISwTempBody other) => this.PerformAdd(other);
        public ISwTempBody[] Substract(ISwTempBody other) => this.PerformSubstract(other);
        public ISwTempBody[] Common(ISwTempBody other) => this.PerformCommon(other);

        public void Preview(IXObject context, Color color)
        {
        }

        public void Dispose()
        {
        }
    }

    internal class SwSheetMacroFeatureEditBody : SwSheetBody, ISwMacroFeatureEditBody, ISwTempSheetBody
    {
        IXMemoryBody IXMemoryBody.Add(IXMemoryBody other) => Add((ISwTempBody)other);
        IXMemoryBody[] IXMemoryBody.Substract(IXMemoryBody other) => Substract((ISwTempBody)other);
        IXMemoryBody[] IXMemoryBody.Common(IXMemoryBody other) => Common((ISwTempBody)other);

        public bool IsPreviewMode { get; }

        public IBody2 PreviewBody => m_PreviewBodyLazy.Value;

        private Lazy<IBody2> m_PreviewBodyLazy;

        internal SwSheetMacroFeatureEditBody(IBody2 body, SwDocument doc, SwApplication app, bool isPreview) : base(body, doc, app)
        {
            IsPreviewMode = isPreview;
            m_PreviewBodyLazy = new LazyMacroFeaturePreviewBody(body, isPreview, app);
        }

        public ISwTempBody Add(ISwTempBody other) => this.PerformAdd(other);
        public ISwTempBody[] Substract(ISwTempBody other) => this.PerformSubstract(other);
        public ISwTempBody[] Common(ISwTempBody other) => this.PerformCommon(other);

        public void Preview(IXObject context, Color color)
        {
        }

        public void Dispose()
        {
        }
    }

    internal class SwSolidMacroFeatureEditBody : SwSolidBody, ISwMacroFeatureEditBody, ISwTempSolidBody
    {
        IXMemoryBody IXMemoryBody.Add(IXMemoryBody other) => Add((ISwTempBody)other);
        IXMemoryBody[] IXMemoryBody.Substract(IXMemoryBody other) => Substract((ISwTempBody)other);
        IXMemoryBody[] IXMemoryBody.Common(IXMemoryBody other) => Common((ISwTempBody)other);

        public bool IsPreviewMode { get; }

        public IBody2 PreviewBody => m_PreviewBodyLazy.Value;

        private Lazy<IBody2> m_PreviewBodyLazy;

        internal SwSolidMacroFeatureEditBody(IBody2 body, SwDocument doc, SwApplication app, bool isPreview) : base(body, doc, app)
        {
            IsPreviewMode = isPreview;
            m_PreviewBodyLazy = new LazyMacroFeaturePreviewBody(body, isPreview, app);
        }

        public ISwTempBody Add(ISwTempBody other) => this.PerformAdd(other);
        public ISwTempBody[] Substract(ISwTempBody other) => this.PerformSubstract(other);
        public ISwTempBody[] Common(ISwTempBody other) => this.PerformCommon(other);

        public void Preview(IXObject context, Color color)
        {
        }

        public void Dispose()
        {
        }
    }

    internal class SwWireMacroFeatureEditBody : SwWireBody, ISwMacroFeatureEditBody, ISwTempWireBody
    {
        IXMemoryBody IXMemoryBody.Add(IXMemoryBody other) => Add((ISwTempBody)other);
        IXMemoryBody[] IXMemoryBody.Substract(IXMemoryBody other) => Substract((ISwTempBody)other);
        IXMemoryBody[] IXMemoryBody.Common(IXMemoryBody other) => Common((ISwTempBody)other);

        public bool IsPreviewMode { get; }

        public IBody2 PreviewBody => m_PreviewBodyLazy.Value;

        private Lazy<IBody2> m_PreviewBodyLazy;

        internal SwWireMacroFeatureEditBody(IBody2 body, SwDocument doc, SwApplication app, bool isPreview) : base(body, doc, app)
        {
            IsPreviewMode = isPreview;
            m_PreviewBodyLazy = new LazyMacroFeaturePreviewBody(body, isPreview, app);
        }

        public ISwTempBody Add(ISwTempBody other) => this.PerformAdd(other);
        public ISwTempBody[] Substract(ISwTempBody other) => this.PerformSubstract(other);
        public ISwTempBody[] Common(ISwTempBody other) => this.PerformCommon(other);

        public void Preview(IXObject context, Color color)
        {
        }

        public void Dispose()
        {
        }
    }
}
