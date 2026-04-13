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
    /// 宏特征参数编辑器实现（PropertyManagerPage 交互层）。
    /// 负责在参数页关闭时提交或回写缓存参数，并提供预览上下文对象。
    /// </summary>
    internal class SwMacroFeatureEditor<TData, TPage> : BaseCustomFeatureEditor<TData, TPage>
        where TData : class
        where TPage : class
    {
        /// <summary>
        /// 提供预览上下文（通常为当前文档中的特征/组件/实体），用于编辑预览显示。
        /// </summary>
        internal event Func<IXDocument, ISwObject> ProvidePreviewContext;

        internal SwMacroFeatureEditor(ISwApplication app, Type defType,
            IServiceProvider svcProvider,
            SwPropertyManagerPage<TPage> page, CustomFeatureEditorBehavior_e behavior) 
            : base(app, defType, svcProvider, page, behavior)
        {
        }

        protected override IXObject CurrentPreviewContext => ProvidePreviewContext?.Invoke(CurrentDocument);

        protected override void CompleteFeature(PageCloseReasons_e reason)
        {
            base.CompleteFeature(reason);

            if (reason == PageCloseReasons_e.Okay || reason == PageCloseReasons_e.Apply) 
            {
                if (m_CurrentFeature.IsCommitted)
                {
                    var curMacroFeat = (SwMacroFeature<TData>)m_CurrentFeature;

                    if (curMacroFeat.UseCachedParameters)
                    {
                        // 对已提交宏特征：将参数缓存回写到特征定义
                        curMacroFeat.ApplyParametersCache();
                        curMacroFeat.UseCachedParameters = false;
                    }
                }
            }
        }
    }
}