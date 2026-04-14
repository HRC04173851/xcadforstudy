// -*- coding: utf-8 -*-
// src/Toolkit/CustomFeature/BaseCustomFeatureEditor.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件实现自定义特征编辑器基类 BaseCustomFeatureEditor<TData, TPage>。
// 负责参数与属性页数据转换、预览几何体显示与编辑生命周期管理。
// 支持编辑启动、参数变更、预览更新和编辑完成等完整工作流程。
//*********************************************************************

using System;
using System.Linq;
using Xarial.XCad.Base;
using Xarial.XCad.Documents;
using Xarial.XCad.Extensions;
using Xarial.XCad.Features.CustomFeature;
using Xarial.XCad.Features.CustomFeature.Delegates;
using Xarial.XCad.Geometry;
using Xarial.XCad.Services;
using Xarial.XCad.Toolkit.CustomFeature;
using Xarial.XCad.UI.PropertyPage;
using Xarial.XCad.UI.PropertyPage.Enums;
using Xarial.XCad.UI.PropertyPage.Structures;
using Xarial.XCad.Utils.Diagnostics;
using Xarial.XCad.Utils.Reflection;
using Xarial.XCad.Toolkit;
using Xarial.XCad.UI.PropertyPage.Delegates;
using System.Collections.Generic;
using Xarial.XCad.Exceptions;
using Xarial.XCad.Toolkit.Utils;
using Xarial.XCad.Features;
using System.Drawing;

namespace Xarial.XCad.Utils.CustomFeature
{
    /// <summary>
    /// Delegate for notifying that custom feature editor state has changed.
    /// <para>用于通知自定义特征编辑状态变化的委托。</para>
    /// </summary>
    /// <typeparam name="TData">Type of custom feature parameters.<para>自定义特征参数类型。</para></typeparam>
    /// <typeparam name="TPage">Type of property manager page data.<para>属性页数据类型。</para></typeparam>
    /// <param name="app">Application object.<para>应用程序对象。</para></param>
    /// <param name="doc">Active CAD document.<para>当前CAD文档对象。</para></param>
    /// <param name="feat">Custom feature instance.<para>自定义特征实例。</para></param>
    /// <param name="page">Property page data model.<para>属性页数据模型。</para></param>
    public delegate void CustomFeatureStateChangedDelegate<TData, TPage>(IXApplication app, IXDocument doc, IXCustomFeature<TData> feat, TPage page)
        where TData : class
        where TPage : class;

    /// <summary>
    /// Delegate for notifying page parameter changes during editing.
    /// <para>用于通知编辑过程中属性页参数变化的委托。</para>
    /// </summary>
    public delegate void CustomFeaturePageParametersChangedDelegate<TData, TPage>(IXApplication app, IXDocument doc, IXCustomFeature<TData> feat, TPage page)
        where TData : class
        where TPage : class;

    /// <summary>
    /// Delegate raised before inserting a new custom feature.
    /// <para>在插入新自定义特征前触发的委托。</para>
    /// </summary>
    public delegate void CustomFeatureInsertedDelegate<TData, TPage>(IXApplication app, IXDocument doc, IXCustomFeature<TData> feat, TPage page)
        where TData : class
        where TPage : class;

    /// <summary>
    /// Delegate raised when editing is completing or completed.
    /// <para>在编辑即将完成或已完成时触发的委托。</para>
    /// </summary>
    public delegate void CustomFeatureEditingCompletedDelegate<TData, TPage>(IXApplication app, IXDocument doc, IXCustomFeature<TData> feat, TPage page, PageCloseReasons_e reason)
        where TData : class
        where TPage : class;

    /// <summary>
    /// Delegate that determines whether preview geometry needs update.
    /// <para>用于判断是否需要更新预览几何体的委托。</para>
    /// </summary>
    public delegate bool ShouldUpdatePreviewDelegate<TData, TPage>(TData oldData, TData newData, TPage page, bool dataChanged)
        where TData : class
        where TPage : class;

    /// <summary>
    /// Delegate for handling parameter read/convert exceptions during editing.
    /// <para>用于处理编辑过程中参数读取或转换异常的委托。</para>
    /// </summary>
    public delegate TData HandleEditingExceptionDelegate<TData>(IXCustomFeature<TData> feat, Exception ex)
        where TData : class;

    /// <summary>
    /// Additional behaviors defined in the macro feature editor
    /// <para>宏特征编辑器中定义的附加行为选项。</para>
    /// </summary>
    [Flags]
    public enum CustomFeatureEditorBehavior_e 
    {
        /// <summary>
        /// Default behavior
        /// <para>默认行为。</para>
        /// </summary>
        Default = 0,

        /// <summary>
        /// If editor page has a pushpin button this option and it is applied,
        /// this option will close and reopen page instead of keeping the page open
        /// <para>当编辑页面启用图钉（Pushpin）并执行应用（Apply）时，页面会关闭并重新打开，而不是持续保持打开状态。</para>
        /// </summary>
        /// <remarks>Some of the feature migth not be able to be created while page is open thus making pushpin not usable<para>某些特征在页面保持打开时可能无法创建，因此图钉模式不可用。</para></remarks>
        ReopenOnApply = 1
    }

    /// <summary>
    /// Base editor workflow for custom features including parameter-page conversion and preview lifecycle.
    /// <para>自定义特征编辑器基类，负责参数与属性页数据转换、预览几何体显示与编辑生命周期管理。</para>
    /// </summary>
    /// <typeparam name="TData">Type of feature parameter model.<para>特征参数模型类型。</para></typeparam>
    /// <typeparam name="TPage">Type of property page model.<para>属性页模型类型。</para></typeparam>
    public abstract class BaseCustomFeatureEditor<TData, TPage> 
        where TData : class
        where TPage : class
    {
        /// <summary>
        /// Occurs when editing starts and the PropertyManager page is shown.
        /// <para>在编辑开始并显示属性管理器页面时触发。</para>
        /// </summary>
        public event CustomFeatureStateChangedDelegate<TData, TPage> EditingStarted;
        /// <summary>
        /// Occurs just before page closing logic completes.
        /// <para>在页面关闭流程完成前触发。</para>
        /// </summary>
        public event CustomFeatureEditingCompletedDelegate<TData, TPage> EditingCompleting;
        /// <summary>
        /// Occurs after feature completion logic executes.
        /// <para>在特征完成处理逻辑执行后触发。</para>
        /// </summary>
        public event CustomFeatureEditingCompletedDelegate<TData, TPage> EditingCompleted;
        /// <summary>
        /// Occurs before a new feature is committed/inserted into model.
        /// <para>在新特征提交插入模型前触发。</para>
        /// </summary>
        public event CustomFeatureInsertedDelegate<TData, TPage> FeatureInserting;
        /// <summary>
        /// Occurs when preview has been updated.
        /// <para>在预览几何体更新后触发。</para>
        /// </summary>
        public event CustomFeaturePageParametersChangedDelegate<TData, TPage> PreviewUpdated;
        /// <summary>
        /// Callback to decide whether preview should be recalculated.
        /// <para>用于决定是否重算预览几何体的回调。</para>
        /// </summary>
        public event ShouldUpdatePreviewDelegate<TData, TPage> ShouldUpdatePreview;
        /// <summary>
        /// Callback to recover from editing exceptions and provide fallback parameters.
        /// <para>用于从编辑异常中恢复并提供回退参数的回调。</para>
        /// </summary>
        public event HandleEditingExceptionDelegate<TData> HandleEditingException;

        /// <summary>
        /// Current xCAD application instance.
        /// <para>当前 xCAD 应用程序实例。</para>
        /// </summary>
        protected readonly IXApplication m_App;
        /// <summary>
        /// Service provider for resolving toolkit services.
        /// <para>用于解析 Toolkit 服务的服务提供器。</para>
        /// </summary>
        protected readonly IServiceProvider m_SvcProvider;
        /// <summary>
        /// Logger used for diagnostics and exception reporting.
        /// <para>用于诊断与异常记录的日志器。</para>
        /// </summary>
        protected readonly IXLogger m_Logger;

        /// <summary>
        /// Equality comparer for CAD bodies.
        /// <para>用于比较 CAD 实体（Body）的相等比较器。</para>
        /// </summary>
        private readonly XObjectEqualityComparer<IXBody> m_BodiesComparer;
        /// <summary>
        /// Parser for custom feature parameters and selections.
        /// <para>用于解析自定义特征参数与选择集的解析器。</para>
        /// </summary>
        private readonly CustomFeatureParametersParser m_ParamsParser;
        /// <summary>
        /// Runtime type of custom feature definition.
        /// <para>自定义特征定义的运行时类型。</para>
        /// </summary>
        private readonly Type m_DefType;

        /// <summary>
        /// PropertyManager page wrapper used for editing UI.
        /// <para>用于编辑界面的 PropertyManager 页面包装对象。</para>
        /// </summary>
        private readonly IXPropertyPage<TPage> m_PmPage;
        /// <summary>
        /// Lazy-loaded custom feature definition instance.
        /// <para>延迟加载的自定义特征定义实例。</para>
        /// </summary>
        private readonly Lazy<IXCustomFeatureDefinition<TData, TPage>> m_DefinitionLazy;

        /// <summary>
        /// Current page data bound to PropertyManager UI.
        /// <para>当前绑定到 PropertyManager 界面的页面数据。</para>
        /// </summary>
        private TPage m_CurPageData;
        /// <summary>
        /// Edit bodies currently hidden while showing preview.
        /// <para>显示预览时被隐藏的编辑实体集合。</para>
        /// </summary>
        private IXBody[] m_HiddenEditBodies;
        /// <summary>
        /// Current custom feature under edit/insert workflow.
        /// <para>当前处于编辑/插入流程中的自定义特征实例。</para>
        /// </summary>
        protected IXCustomFeature<TData> m_CurrentFeature;
        /// <summary>
        /// Last error captured during update or closing flow.
        /// <para>在更新或关闭流程中捕获的最近一次错误。</para>
        /// </summary>
        private Exception m_LastError;
        /// <summary>
        /// Cached preview memory bodies displayed in viewport.
        /// <para>在视图中显示的预览内存实体缓存。</para>
        /// </summary>
        private IXMemoryBody[] m_PreviewBodies;

        /// <summary>
        /// Current document being edited.
        /// <para>当前正在编辑的文档对象。</para>
        /// </summary>
        protected IXDocument CurrentDocument { get; private set; }
        
        /// <summary>
        /// Indicates whether PropertyManager page is active.
        /// <para>指示 PropertyManager 页面是否处于活动状态。</para>
        /// </summary>
        private bool m_IsPageActive;

        /// <summary>
        /// Behavior flags controlling editor apply/close logic.
        /// <para>控制编辑器应用/关闭逻辑的行为标志。</para>
        /// </summary>
        private readonly CustomFeatureEditorBehavior_e m_Behavior;

        /// <summary>
        /// Initializes a new custom feature editor instance.
        /// <para>初始化自定义特征编辑器实例。</para>
        /// </summary>
        /// <param name="app">Application object.<para>应用程序对象。</para></param>
        /// <param name="featDefType">Custom feature definition type.<para>自定义特征定义类型。</para></param>
        /// <param name="svcProvider">Service provider.<para>服务提供器。</para></param>
        /// <param name="page">Property manager page wrapper.<para>属性管理器页面包装对象。</para></param>
        /// <param name="behavior">Editor behavior flags.<para>编辑器行为标志。</para></param>
        public BaseCustomFeatureEditor(IXApplication app,
            Type featDefType,
            IServiceProvider svcProvider, IXPropertyPage<TPage> page, CustomFeatureEditorBehavior_e behavior)
        {
            m_App = app;
            m_SvcProvider = svcProvider;
            m_Logger = svcProvider.GetService<IXLogger>();
            m_DefType = featDefType;
            m_BodiesComparer = new XObjectEqualityComparer<IXBody>();
            m_ParamsParser = new CustomFeatureParametersParser();

            m_Behavior = behavior;

            m_DefinitionLazy = new Lazy<IXCustomFeatureDefinition<TData, TPage>>(
                () => (IXCustomFeatureDefinition<TData, TPage>)CustomFeatureDefinitionInstanceCache.GetInstance(m_DefType));

            m_PmPage = page;

            m_PmPage.Closing += OnPageClosing;
            m_PmPage.DataChanged += OnDataChanged;
            m_PmPage.Closed += OnPageClosed;
        }
        
        /// <summary>
        /// Gets the resolved custom feature definition instance.
        /// <para>获取已解析的自定义特征定义实例。</para>
        /// </summary>
        private IXCustomFeatureDefinition<TData, TPage> Definition => m_DefinitionLazy.Value;

        /// <summary>
        /// Active feature editor transaction wrapper.
        /// <para>当前特征编辑事务包装对象。</para>
        /// </summary>
        private IEditor<IXFeature> m_CurEditor;

        /// <summary>
        /// Opens an existing custom feature for editing.
        /// <para>打开并编辑现有自定义特征。</para>
        /// </summary>
        /// <param name="model">Target document.<para>目标文档。</para></param>
        /// <param name="feature">Feature instance to edit.<para>待编辑的特征实例。</para></param>
        public void Edit(IXDocument model, IXCustomFeature<TData> feature)
        {
            if (feature == null) 
            {
                throw new ArgumentNullException(nameof(feature));
            }

            m_IsPageActive = true;

            CurrentDocument = model;
            m_CurrentFeature = feature;
            m_CurEditor = m_CurrentFeature.Edit();

            try
            {
                TData featData;

                try
                {
                    featData = m_CurrentFeature.Parameters;
                }
                catch (Exception ex)
                {
                    featData = HandleEditingException.Invoke(m_CurrentFeature, ex);
                    m_CurrentFeature.Parameters = featData;
                }

                m_CurPageData = Definition.ConvertParamsToPage(m_App, model, featData);

                EditingStarted?.Invoke(m_App, model, feature, m_CurPageData);

                m_PmPage.Show(m_CurPageData);

                UpdatePreview();
            }
            catch(Exception ex)
            {
                m_Logger.Log(ex);
                m_CurEditor.Cancel = true;
                m_CurEditor.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Pre-creates and starts editing a new custom feature instance.
        /// <para>预创建新的自定义特征实例并开始编辑。</para>
        /// </summary>
        /// <param name="doc">Target document.<para>目标文档。</para></param>
        /// <param name="data">Initial feature parameters.<para>初始特征参数。</para></param>
        public void Insert(IXDocument doc, TData data)
        {
            m_IsPageActive = true;
            
            CurrentDocument = doc;

            m_CurrentFeature = CurrentDocument.Features.PreCreateCustomFeature<TData>();
            m_CurrentFeature.DefinitionType = m_DefType;
            m_CurrentFeature.Parameters = data;

            m_CurPageData = Definition.ConvertParamsToPage(m_App, doc, data);

            EditingStarted?.Invoke(m_App, doc, m_CurrentFeature, m_CurPageData);

            m_PmPage.Show(m_CurPageData);

            UpdatePreview();
        }

        /// <summary>
        /// Current object context used to display preview geometry.
        /// <para>用于显示预览几何体的当前对象上下文。</para>
        /// </summary>
        protected virtual IXObject CurrentPreviewContext => CurrentDocument;

        /// <summary>
        /// Displays preview memory bodies in current preview context.
        /// <para>在当前预览上下文中显示预览内存实体（Memory Body）。</para>
        /// </summary>
        private void DisplayPreview(IXMemoryBody[] bodies, AssignPreviewBodyColorDelegate assignPreviewBodyColorDelegateFunc)
        {
            if (bodies?.Any() == true)
            {
                var previewContext = CurrentPreviewContext;

                if (previewContext == null)
                {
                    throw new Exception("Preview context is not specified");
                }

                foreach (var body in bodies)
                {
                    assignPreviewBodyColorDelegateFunc.Invoke(body, out Color color);

                    body.Preview(previewContext, color);
                }
            }
        }

        /// <summary>
        /// Hides and disposes currently shown preview bodies.
        /// <para>隐藏并释放当前显示的预览实体。</para>
        /// </summary>
        private void HidePreview(IXMemoryBody[] bodies)
        {
            if (bodies != null)
            {
                for (int i = 0; i < bodies.Length; i++)
                {
                    try
                    {
                        bodies[i].Visible = false;
                        bodies[i].Dispose();
                    }
                    catch (Exception ex)
                    {
                        m_Logger.Log(ex);
                    }

                    bodies[i] = null;
                }
            }
        }

        /// <summary>
        /// Hides source edit bodies while preview is shown to avoid duplicate geometry display.
        /// <para>显示预览几何体时隐藏编辑实体，避免模型与预览重复显示。</para>
        /// </summary>
        private void HideEditBodies(ShouldHidePreviewEditBodyDelegate<TData, TPage> shouldHidePreviewEditBodyFunc)
        {
            IXBody[] editBodies;

            m_ParamsParser.Parse(m_CurrentFeature.Parameters, out _, out _, out _, out _, out editBodies);

            var bodiesToShow = m_HiddenEditBodies.ValueOrEmpty().Except(editBodies.ValueOrEmpty(), m_BodiesComparer);

            foreach (var body in bodiesToShow)
            {
                body.Visible = true;
            }

            var doNotHideBodies = new List<IXBody>();

            var bodiesToHide = editBodies.ValueOrEmpty().Except(m_HiddenEditBodies.ValueOrEmpty(), m_BodiesComparer);

            foreach (var body in bodiesToHide)
            {
                var hide = body.Visible;

                if (hide && shouldHidePreviewEditBodyFunc != null) 
                {
                    hide &= shouldHidePreviewEditBodyFunc.Invoke(body, m_CurrentFeature.Parameters, m_CurPageData);
                }

                if (hide)
                {   
                    body.Visible = false;
                }
                else 
                {
                    doNotHideBodies.Add(body);
                }
            }

            if (editBodies != null)
            {
                m_HiddenEditBodies = editBodies.Except(doNotHideBodies).ToArray();
            }
            else 
            {
                m_HiddenEditBodies = null;
            }
        }

        /// <summary>
        /// Clears current preview bodies cache and visual state.
        /// <para>清理当前预览实体缓存及其可视状态。</para>
        /// </summary>
        private void HidePreviewBodies()
        {
            if (m_PreviewBodies != null)
            {
                HidePreview(m_PreviewBodies);
            }

            m_PreviewBodies = null;
        }

        /// <summary>
        /// Handles PropertyManager page data change event.
        /// <para>处理属性管理器页面数据变化事件。</para>
        /// </summary>
        private void OnDataChanged()
        {
            if (m_IsPageActive)
            {
                var oldParams = m_CurrentFeature.Parameters;
                var newParams = Definition.ConvertPageToParams(m_App, CurrentDocument, m_CurPageData, oldParams);

                var dataChanged = AreParametersChanged(oldParams, newParams);

                var needUpdatePreview = ShouldUpdatePreview.Invoke(oldParams, newParams, m_CurPageData, dataChanged);

                m_CurrentFeature.Parameters = newParams;

                if (needUpdatePreview)
                {
                    UpdatePreview();

                    PreviewUpdated?.Invoke(m_App, CurrentDocument, m_CurrentFeature, m_CurPageData);
                }
            }
        }

        /// <summary>
        /// Compares parsed parameter payload to determine whether feature definition data changed.
        /// <para>比较解析后的参数载荷，判断特征定义数据是否发生变化。</para>
        /// </summary>
        private bool AreParametersChanged(TData oldParams, TData newParams) 
        {
            bool AreArraysEqual<T>(T[] oldArr, T[] newArr, Func<T, T, bool> comparer)
            {
                if (oldArr == null && newArr == null)
                {
                    return true;
                }
                else if (oldArr == null || newArr == null)
                {
                    return false;
                }
                else 
                {
                    for (int i = 0; i < oldArr.Length; i++) 
                    {
                        if (!comparer.Invoke(oldArr[i], newArr[i])) 
                        {
                            return false;
                        }
                    }
                }

                return true;
            }

            m_ParamsParser.Parse(oldParams, out CustomFeatureAttribute[] oldAtts, out IXSelObject[] oldSels, out _, out double[] oldDimVals, out IXBody[] oldEditBodies);
            m_ParamsParser.Parse(newParams, out CustomFeatureAttribute[] newAtts, out IXSelObject[] newSels, out _, out double[] newDimVals, out IXBody[] newEditBodies);

            return !(AreArraysEqual(oldAtts, newAtts, (o, n) => string.Equals(o.Name, n.Name) && object.Equals(o.Value, n.Value) && Type.Equals(o.Type, n.Type))
                    && AreArraysEqual(oldSels, newSels, (o, n) => o.Equals(n))
                    && AreArraysEqual(oldDimVals, newDimVals, (o, n) => double.Equals(o, n))
                    && AreArraysEqual(oldEditBodies, newEditBodies, (o, n) => o.Equals(n)));
        }

        /// <summary>
        /// Handles PropertyManager page closed event and finalizes editor state.
        /// <para>处理属性管理器页面关闭事件，并收尾编辑器状态。</para>
        /// </summary>
        private void OnPageClosed(PageCloseReasons_e reason)
        {
            if (m_IsApplying)
            {
                reason = PageCloseReasons_e.Apply;
            }

            var cachedParams = m_CurrentFeature.Parameters;

            m_IsPageActive = false;

            CompleteFeature(reason);

            TData reusableParams;

            if (m_IsApplying)
            {
                reusableParams = Definition.ConvertPageToParams(
                    m_App, CurrentDocument, m_CurPageData, cachedParams);
            }
            else 
            {
                reusableParams = default;
            }

            m_CurEditor?.Dispose();

            m_CurPageData = null;
            m_HiddenEditBodies = null;
            m_CurrentFeature = null;
            m_LastError = null;
            m_PreviewBodies = null;
            m_CurEditor = null;

            if (!m_IsApplying)
            {
                CurrentDocument = null;
            }
            else
            {
                m_IsApplying = false;
                Insert(CurrentDocument, reusableParams);
                m_PmPage.IsPinned = true;
            }
        }

        /// <summary>
        /// Restores visibility of edit bodies hidden during preview.
        /// <para>恢复在预览阶段被隐藏的编辑实体可见性。</para>
        /// </summary>
        private void ShowEditBodies()
        {
            foreach (var body in m_HiddenEditBodies.ValueOrEmpty())
            {
                body.Visible = true;
            }

            m_HiddenEditBodies = null;
        }

        /// <summary>
        /// Handles page closing event and apply/cancel behavior.
        /// <para>处理页面关闭事件以及应用/取消行为。</para>
        /// </summary>
        private void OnPageClosing(PageCloseReasons_e reason, PageClosingArg arg)
        {
            if (!m_IsApplying)
            {
                if (EditingCompleting != null)
                {
                    try
                    {
                        EditingCompleting.Invoke(m_App, CurrentDocument, m_CurrentFeature, m_CurPageData, reason);
                    }
                    catch (Exception ex)
                    {
                        m_Logger.Log(ex);
                        m_LastError = ex;
                    }
                }

                if (m_LastError != null)
                {
                    arg.ErrorMessage = m_LastError is IUserException ? m_LastError.Message : "Unknown error. Please see log for more details";
                    arg.Cancel = true;
                }
                else
                {
                    if (reason == PageCloseReasons_e.Apply)
                    {
                        if (m_Behavior.HasFlag(CustomFeatureEditorBehavior_e.ReopenOnApply))
                        {
                            m_IsApplying = true;
                            m_PmPage.Close(true);
                        }
                        else
                        {
                            CompleteFeature(reason);

                            m_CurrentFeature.Parameters = Definition.ConvertPageToParams(m_App, CurrentDocument, m_CurPageData, m_CurrentFeature.Parameters);

                            //page stays open
                            UpdatePreview();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Indicates apply workflow mode when page is reopened on apply.
        /// <para>指示“应用后重开页面”场景下是否处于 Apply 流程模式。</para>
        /// </summary>
        private bool m_IsApplying;

        /// <summary>
        /// Assigns default preview color (semi-transparent yellow).
        /// <para>指定默认预览颜色（半透明黄色）。</para>
        /// </summary>
        private void DefaultAssignPreviewBodyColor(IXBody body, out Color color)
            => color = Color.FromArgb(100, Color.Yellow);

        /// <summary>
        /// Rebuilds preview geometry, updates body visibility and handles preview errors.
        /// <para>重建预览几何体、更新实体可见性，并处理预览异常。</para>
        /// </summary>
        private void UpdatePreview()
        {
            if (m_IsPageActive)
            {
                using (CurrentDocument.ModelViews.Active.Freeze(true))
                {
                    try
                    {
                        m_LastError = null;

                        HidePreviewBodies();

                        m_PreviewBodies = Definition.CreatePreviewGeometry(m_App, CurrentDocument,
                            m_CurrentFeature, m_CurPageData, out var shouldHidePreviewEdit,
                            out var assignPreviewColor);

                        if (assignPreviewColor == null)
                        {
                            assignPreviewColor = DefaultAssignPreviewBodyColor;
                        }

                        HideEditBodies(shouldHidePreviewEdit);

                        if (m_PreviewBodies?.Any() == true)
                        {
                            DisplayPreview(m_PreviewBodies, assignPreviewColor);
                        }
                    }
                    catch (Exception ex)
                    {
                        HidePreviewBodies();
                        ShowEditBodies();
                        m_Logger.Log(ex);
                        m_LastError = ex;
                    }
                }
            }
        }

        /// <summary>
        /// Completes feature editing transaction according to close reason.
        /// <para>根据关闭原因完成特征编辑事务。</para>
        /// </summary>
        /// <param name="reason">Page close reason.<para>页面关闭原因。</para></param>
        protected virtual void CompleteFeature(PageCloseReasons_e reason)
        {
            EditingCompleted?.Invoke(m_App, CurrentDocument, m_CurrentFeature, m_CurPageData, reason);

            ShowEditBodies();

            HidePreviewBodies();

            m_PreviewBodies = null;

            if (reason == PageCloseReasons_e.Okay || reason == PageCloseReasons_e.Apply)
            {
                if (!m_CurrentFeature.IsCommitted)
                {
                    FeatureInserting?.Invoke(m_App, CurrentDocument, m_CurrentFeature, m_CurPageData);
                }
            }
            else
            {
                if (m_CurrentFeature.IsCommitted)
                {
                    m_CurEditor.Cancel = true;
                }
            }
        }
    }
}