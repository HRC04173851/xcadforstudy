// -*- coding: utf-8 -*-
// src/SolidWorks/Features/CustomFeature/SwMacroFeatureEditor.cs

//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using SolidWorks.Interop.swconst;
using System;
using System.Drawing;
using Xarial.XCad.Base;
using Xarial.XCad.Documents;
using Xarial.XCad.Extensions;
using Xarial.XCad.Features.CustomFeature;
using Xarial.XCad.Features.CustomFeature.Delegates;
using Xarial.XCad.Geometry;
using Xarial.XCad.SolidWorks.Documents;
using Xarial.XCad.SolidWorks.Geometry;
using Xarial.XCad.SolidWorks.UI.PropertyPage;
using Xarial.XCad.Toolkit.Utils;
using Xarial.XCad.UI.PropertyPage;
using Xarial.XCad.UI.PropertyPage.Delegates;
using Xarial.XCad.UI.PropertyPage.Enums;
using Xarial.XCad.Utils.CustomFeature;
using Xarial.XCad.Utils.Diagnostics;

namespace Xarial.XCad.SolidWorks.Features.CustomFeature
{
    /// <summary>
    /// 带属性管理器页面的宏特性编辑器。
    /// <para>
    /// 负责管理带参数类型和属性页面的宏特性编辑流程。
    /// 处理预览上下文提供、特性的完成和参数缓存应用。
    /// </para>
    /// </summary>
    /// <typeparam name="TData">参数类型</typeparam>
    /// <typeparam name="TPage">属性页面数据类型</typeparam>
    internal class SwMacroFeatureEditor<TData, TPage> : BaseCustomFeatureEditor<TData, TPage>
        where TData : class
        where TPage : class
    {
        /// <summary>
        /// 当需要提供预览上下文时触发
        /// <para>用于确定预览应该使用的文档上下文（部件或部件组件）</para>
        /// </summary>
        internal event Func<IXDocument, ISwObject> ProvidePreviewContext;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="app">应用程序</param>
        /// <param name="defType">宏特性定义类型</param>
        /// <param name="svcProvider">服务提供者</param>
        /// <param name="page">属性管理器页面</param>
        /// <param name="behavior">编辑器行为选项</param>
        internal SwMacroFeatureEditor(ISwApplication app, Type defType,
            IServiceProvider svcProvider,
            SwPropertyManagerPage<TPage> page, CustomFeatureEditorBehavior_e behavior)
            : base(app, defType, svcProvider, page, behavior)
        {
        }

        /// <summary>
        /// 获取当前预览上下文
        /// </summary>
        protected override IXObject CurrentPreviewContext => ProvidePreviewContext?.Invoke(CurrentDocument);

        /// <summary>
        /// 完成特性编辑
        /// <para>当用户点击"确定"或"应用"按钮时调用</para>
        /// </summary>
        /// <remarks>
        /// 此方法处理参数缓存的应用。
        /// 在编辑过程中，参数被缓存以提高性能；
        /// 当编辑成功完成时，缓存的参数被应用到特性。
        /// </remarks>
        protected override void CompleteFeature(PageCloseReasons_e reason)
        {
            base.CompleteFeature(reason);

            // 仅在用户确认（确定或应用）时应用参数缓存
            if (reason == PageCloseReasons_e.Okay || reason == PageCloseReasons_e.Apply)
            {
                if (m_CurrentFeature.IsCommitted)
                {
                    var curMacroFeat = (SwMacroFeature<TData>)m_CurrentFeature;

                    // 如果使用了参数缓存，应用它
                    if (curMacroFeat.UseCachedParameters)
                    {
                        curMacroFeat.ApplyParametersCache();
                        curMacroFeat.UseCachedParameters = false;
                    }
                }
            }
        }
    }
}