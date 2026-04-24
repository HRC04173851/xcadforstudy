// -*- coding: utf-8 -*- // samples/SwAddIn/BoxMacroFeatureEditor.cs
//*********************************************************************
//xCAD
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 中文：BoxMacroFeatureEditor.cs - 宏特征编辑器示例
// 演示如何创建 SOLIDWORKS 宏特征（Macro Feature），包括：
//   - 属性页（PropertyPage）与特征参数的双向数据转换
//   - 几何体创建与预览
//   - 尺寸标注对齐
//   - 错误处理与数据验证
// This file demonstrates a SOLIDWORKS Macro Feature editor with bidirectional
// conversion between property page parameters and feature data, geometry creation,
// dimension alignment, and error handling.

using SwAddInExample.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Xarial.XCad;
using Xarial.XCad.Documents;
using Xarial.XCad.Features.CustomFeature;
using Xarial.XCad.Features.CustomFeature.Attributes;
using Xarial.XCad.Features.CustomFeature.Delegates;
using Xarial.XCad.Features.CustomFeature.Enums;
using Xarial.XCad.Features.CustomFeature.Services;
using Xarial.XCad.Geometry;
using Xarial.XCad.Geometry.Structures;
using Xarial.XCad.SolidWorks;
using Xarial.XCad.SolidWorks.Documents;
using Xarial.XCad.SolidWorks.Features.CustomFeature;
using Xarial.XCad.SolidWorks.Geometry;
using Xarial.XCad.SolidWorks.UI.PropertyPage;
using Xarial.XCad.UI.PropertyPage.Attributes;
using Xarial.XCad.UI.PropertyPage.Base;
using Xarial.XCad.UI.PropertyPage.Enums;
using Xarial.XCad.UI.PropertyPage.Services;
using Xarial.XCad.UI.PropertyPage.Structures;

namespace SwAddInExample
{
    /// <summary>
    /// 中文：宏特征的属性页（PropertyPage）
    /// 包含确定、取消、钉选按钮，并支持页面锁定功能
    /// The macro feature property page with Okay, Cancel, Pin buttons and page locking support.
    /// </summary>
    /// <remarks>
    /// 中文：PageOptions_e 控制属性页的行为：
    ///   - OkayButton：显示确定按钮
    ///   - CancelButton：显示取消按钮
    ///   - PushpinButton：允许用户钉选页面使其保持打开
    ///   - LockedPage：防止页面在特征数据更改时自动关闭
    /// </remarks>
    [PageOptions(PageOptions_e.OkayButton | PageOptions_e.PushpinButton | PageOptions_e.CancelButton | PageOptions_e.LockedPage)]
    public class BoxPage
    {
        /// <summary>
        /// 中文：获取或设置盒形参数的页面数据
        /// Gets or sets the box parameters page data containing all user-editable values.
        /// </summary>
        public BoxParametersPage Parameters { get; set; }

        /// <summary>
        /// 中文：构造函数，初始化空的参数页面
        /// Constructor initializing an empty parameters page.
        /// </summary>
        /// <param name="dummy">中文：虚拟参数，用于区分不同构造函数（因为 SWIG/C++ 接口需要）Dummy parameter to distinguish constructors for SWIG/C++ interface compatibility.</param>
        public BoxPage(string dummy)
        {
            Parameters = new BoxParametersPage();
        }
    }

    /// <summary>
    /// 中文：盒形特征的参数数据类
    /// 实现 INotifyPropertyChanged 以支持属性页的数据绑定
    /// Box feature parameters implementing INotifyPropertyChanged for property page data binding.
    /// </summary>
    /// <remarks>
    /// 中文：此类的属性直接绑定到属性页控件：
    ///   - Width/Height/Length：长宽高，使用数值框编辑，单位 mm
    ///   - BaseFace：基准面，用于确定盒子的放置位置和方向
    ///   - TestFaces：测试选择的面集合
    ///   - Size/Volume：只读文本显示，仅用于信息展示
    /// </remarks>
    public class BoxParametersPage : INotifyPropertyChanged
    {
        /// <inheritdoc />
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 中文：面选择过滤器接口实现
        /// 示例：演示如何自定义选择行为
        /// Sample selection filter implementing ISelectionCustomFilter interface.
        /// </summary>
        /// <remarks>
        /// 中文：此过滤器可以访问选择框的当前值和选中对象，
        /// 可用于实现自定义验证、过滤或动态调整选择条件
        /// This filter can access the selection box value and selected object,
        /// used for custom validation, filtering, or dynamic selection criteria adjustment.
        /// </remarks>
        public class SampleSelectionFilter : ISelectionCustomFilter
        {
            /// <summary>
            /// 中文：过滤选择操作
            /// Filters the selection operation when user selects an object.
            /// </summary>
            /// <param name="selBox">中文：选择框控件，提供 GetValue 方法读取当前选择 Selection box control with GetValue method to read current selection.</param>
            /// <param name="selection">中文：用户选择的对象 The object selected by user.</param>
            /// <param name="args">中文：传递给过滤器的参数 Arguments passed to the filter.</param>
            public void Filter(IControl selBox, IXSelObject selection, SelectionCustomFilterArguments args)
            {
                var val = selBox.GetValue();
            }
        }

        /// <summary>
        /// 中文：基准面 - 定义盒子放置的起始面
        /// 该面决定了盒子的位置、方向和尺寸参考
        /// Base face defining the placement start surface for the box.
        /// This face determines box position, orientation, and dimension reference.
        /// </summary>
        /// <remarks>
        /// 中文：基准面的用途：
        ///   1. 确定盒子在模型中的位置（使用面的中心点）
        ///   2. 确定盒子的方向（使用面的法向作为盒子 Z 轴方向）
        ///   3. 确定盒子的参考方向（使用面的参考方向作为盒子 X 轴方向）
        ///   4. 如果未指定基准面，使用默认方向（Z轴向上，X轴向右）
        /// </remarks>
        public IXFace BaseFace { get; set; }

        /// <summary>
        /// 中文：测试用的面选择集合
        /// 使用 SelectionBoxOptions 指定自定义过滤器
        /// Test face selection collection with custom selection filter.
        /// </summary>
        [SelectionBoxOptions(typeof(SampleSelectionFilter))]
        public List<IXFace> TestFaces { get; set; }

        private string m_Size;
        private double m_Volume;

        /// <summary>
        /// 中文：只读文本块，显示盒子尺寸字符串 "Width x Height x Length"
        /// Read-only text block displaying box dimensions string "Width x Height x Length".
        /// </summary>
        /// <remarks>
        /// 中文：SilentControl 特性表示此控件值更改时不触发特征重建
        /// TextBlock 用于显示格式化文本，不允许用户直接编辑
        /// SilentControl attribute means changes to this control don't trigger feature rebuild.
        /// TextBlock is used to display formatted text, not user-editable.
        /// </remarks>
        [TextBlock]
        [SilentControl]
        public string Size
        {
            get => m_Size;
            set
            {
                m_Size = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Size)));
            }
        }

        /// <summary>
        /// 中文：只读文本块，显示计算后的体积（单位：mm³）
        /// Read-only text block displaying calculated volume in mm³.
        /// </summary>
        /// <remarks>
        /// 中文：体积计算公式：Width * Height * Length * 1000³
        /// 使用 TextBlockOptions 指定格式化字符串
        /// Volume formula: Width * Height * Length * 1000³
        /// Uses TextBlockOptions to specify format string.
        /// </remarks>
        [TextBlock]
        [SilentControl]
        [TextBlockOptions(format: "Volume: {0:N2} mm^3")]
        public double Volume
        {
            get => m_Volume;
            set
            {
                m_Volume = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Volume)));
            }
        }

        /// <summary>
        /// 中文：盒子宽度（单位：mm，默认 0.1）
        /// Box width in mm, default 0.1.
        /// </summary>
        /// <remarks>
        /// 中文：NumberBoxOptions 配置：
        ///   - units: 长度单位 (mm)
        ///   - range: 0 到 1000 mm
        ///   - step: 0.01（微调步长），0.1（大步长），0.001（精确步长）
        ///   - Icon 属性指定在控件旁显示的图标资源
        /// </remarks>
        [NumberBoxOptions(units: NumberBoxUnitType_e.Length, 0, 1000, 0.01, false, 0.1, 0.001)]
        [Xarial.XCad.Base.Attributes.Icon(typeof(Resources), nameof(Resources.horizontal))]
        public double Width { get; set; }

        /// <summary>
        /// 中文：盒子高度（单位：mm，默认 0.2）
        /// Box height in mm, default 0.2.
        /// </summary>
        /// <remarks>
        /// 中文：高度与宽度使用相同的数值框选项，但关联不同的图标资源
        /// Height shares same NumberBoxOptions as Width but uses different icon resource.
        /// </remarks>
        [NumberBoxOptions(units: NumberBoxUnitType_e.Length, 0, 1000, 0.01, false, 0.1, 0.001)]
        [Xarial.XCad.Base.Attributes.Icon(typeof(Resources), nameof(Resources.vertical))]
        public double Height { get; set; }

        /// <summary>
        /// 中文：盒子长度（单位：mm，默认 0.3）
        /// Box length in mm, default 0.3.
        /// </summary>
        /// <remarks>
        /// 中文：长度参数带有 ParameterDimension 特性，
        /// 用于在 SOLIDWORKS 中将此参数标记为可驱动尺寸
        /// Length parameter has ParameterDimension attribute to mark it as driven dimension in SOLIDWORKS.
        /// </remarks>
        [NumberBoxOptions(units: NumberBoxUnitType_e.Length, 0, 1000, 0.01, false, 0.1, 0.001)]
        public double Length { get; set; }

        /// <summary>
        /// 中文：重置所有参数为默认值
        /// Resets all parameters to default values.
        /// </summary>
        /// <remarks>
        /// 中文：在特征插入时调用，确保属性页显示干净的状态
        /// 同时触发 PropertyChanged 事件以更新 UI
        /// Called when feature is inserted to ensure property page shows clean state.
        /// Also triggers PropertyChanged events to update UI.
        /// </remarks>
        internal void Reset()
        {
            Width = 0.1;
            Height = 0.2;
            Length = 0.3;
            TestFaces = null;
            BaseFace = null;

            // 中文：手动触发所有属性的 PropertyChanged 事件
            // Manually trigger PropertyChanged events for all properties
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BaseFace)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TestFaces)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Width)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Height)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Length)));
        }

        /// <summary>
        /// 中文：更新 Size 和 Volume 显示值
        /// Updates the Size and Volume display values based on current dimensions.
        /// </summary>
        /// <remarks>
        /// 中文：在预览几何体创建前调用
        /// 计算体积时乘以 1000³，因为尺寸以米为单位存储而显示单位为 mm
        /// Called before preview geometry creation.
        /// Multiplies by 1000³ because dimensions are stored in meters but displayed in mm.
        /// </remarks>
        internal void UpdateSize()
        {
            Size = $"{Width}x{Height}x{Length}";
            Volume = Width * 1000 * Height * 1000 * Length * 1000;
        }
    }

    /// <summary>
    /// 中文：宏特征数据类
    /// 存储在 SOLIDWORKS 模型中的特征参数
    /// Macro feature data class storing feature parameters in SOLIDWORKS model.
    /// </summary>
    /// <remarks>
    /// 中文：此类定义宏特征的持久化数据结构
    /// 所有属性都会自动序列化和反序列化到模型中
    /// This class defines the serialized data structure for the macro feature.
    /// All properties are automatically serialized/deserialized to/from the model.
    /// </remarks>
    public class BoxMacroFeatureData
    {
        /// <summary>
        /// 中文：基准面引用
        /// Base face reference for box placement.
        /// </summary>
        public IXFace BaseFace { get; set; }

        // 中文：测试面集合（被注释掉的 ParameterExclude 表示此属性不作为独立参数）
        // Test faces collection (commented ParameterExclude means this property is not treated as independent parameter)
        //[ParameterExclude]
        public List<IXFace> TestFaces { get; set; }

        /// <summary>
        /// 中文：盒子宽度（默认值 0.1 米）
        /// Box width in meters, default 0.1.
        /// </summary>
        public double Width { get; set; } = 0.1;

        /// <summary>
        /// 中文：盒子高度（默认值 0.2 米）
        /// Box height in meters, default 0.2.
        /// </summary>
        public double Height { get; set; } = 0.2;

        /// <summary>
        /// 中文：盒子长度（默认值 0.3 米，带线性尺寸标注）
        /// Box length in meters, default 0.3, with linear dimension annotation.
        /// </summary>
        /// <remarks>
        /// 中文：ParameterDimension 特性将此属性标记为可标注的尺寸
        /// CustomFeatureDimensionType_e.Linear 指定尺寸类型为线性
        /// 此特性使 SOLIDWORKS 能够在模型中显示此参数的标注
        /// ParameterDimension attribute marks this property as annotatable.
        /// CustomFeatureDimensionType_e.Linear specifies dimension type as linear.
        /// This allows SOLIDWORKS to display dimension annotation in the model.
        /// </remarks>
        [ParameterDimension(CustomFeatureDimensionType_e.Linear)]
        public double Length { get; set; } = 0.3;
    }

    /// <summary>
    /// 中文：盒形宏特征编辑器
    /// 继承自 SwMacroFeatureDefinition，实现自定义宏特征的所有必需方法
    /// Box macro feature editor extending SwMacroFeatureDefinition, implementing all required methods for custom macro features.
    /// </summary>
    /// <typeparam name="BoxMacroFeatureData">中文：特征参数数据类型 The feature parameter data type.</typeparam>
    /// <typeparam name="BoxPage">中文：属性页数据类型 The property page data type.</typeparam>
    /// <remarks>
    /// 中文：SwMacroFeatureDefinition 是 xCAD 框架提供的基类
    /// 封装了宏特征与 SOLIDWORKS 之间的交互逻辑
    /// SwMacroFeatureDefinition is the base class provided by xCAD framework,
    /// encapsulating interaction logic between macro features and SOLIDWORKS.
    /// </remarks>
    [ComVisible(true)]
    public class BoxMacroFeatureEditor : SwMacroFeatureDefinition<BoxMacroFeatureData, BoxPage>
    {
        /// <summary>
        /// 中文：构造函数
        /// 注册特征重建后的回调事件
        /// Constructor registering post-rebuild callback event.
        /// </summary>
        public BoxMacroFeatureEditor()
        {
            this.PostRebuild += OnPostRebuild;
        }

        /// <summary>
        /// 中文：将属性页数据转换为特征参数
        /// 当用户点击"确定"或"应用"时调用
        /// Converts property page data to feature parameters when user clicks OK or Apply.
        /// </summary>
        /// <param name="app">中文：xCAD 应用程序实例 XCAD application instance.</param>
        /// <param name="doc">中文：当前文档 The current document.</param>
        /// <param name="page">中文：属性页数据 The property page data.</param>
        /// <param name="cudData">中文：现有特征数据的引用（可用于比较变更）Reference to existing feature data (can be used to compare changes).</param>
        /// <returns>中文：转换后的特征参数数据 The converted feature parameter data.</returns>
        /// <remarks>
        /// 中文：此方法实现双向数据转换的关键路径之一
        /// 属性页中的用户输入需要转换为可序列化的特征参数
        /// This method implements one of the key paths for bidirectional data conversion.
        /// User input from property page needs to be converted to serializable feature parameters.
        /// </remarks>
        public override BoxMacroFeatureData ConvertPageToParams(IXApplication app, IXDocument doc, BoxPage page, BoxMacroFeatureData cudData)
            => new BoxMacroFeatureData()
            {
                Height = page.Parameters.Height,
                Length = page.Parameters.Length,
                Width = page.Parameters.Width,
                BaseFace = page.Parameters.BaseFace,
                TestFaces = page.Parameters.TestFaces
            };

        /// <summary>
        /// 中文：将特征参数转换为属性页数据
        /// 当用户编辑现有特征或打开属性页时调用
        /// Converts feature parameters to property page data when user edits existing feature or opens property page.
        /// </summary>
        /// <param name="app">中文：xCAD 应用程序实例 XCAD application instance.</param>
        /// <param name="doc">中文：当前文档 The current document.</param>
        /// <param name="par">中文：存储在模型中的特征参数 The feature parameters stored in the model.</param>
        /// <returns>中文：初始化好的属性页数据 The initialized property page data.</returns>
        /// <remarks>
        /// 中文：此方法实现双向数据转换的另一个关键路径
        /// 存储的特征参数需要转换为 UI 可用的属性页数据
        /// This method implements the other key path for bidirectional data conversion.
        /// Stored feature parameters need to be converted to UI-ready property page data.
        /// </remarks>
        public override BoxPage ConvertParamsToPage(IXApplication app, IXDocument doc, BoxMacroFeatureData par)
            => new BoxPage("")
            {
                Parameters = new BoxParametersPage()
                {
                    Height = par.Height,
                    Length = par.Length,
                    Width = par.Width,
                    BaseFace = par.BaseFace,
                    TestFaces = par.TestFaces
                }
            };

        /// <summary>
        /// 中文：创建几何体
        /// 宏特征的核心方法，生成实际的 CAD 几何体
        /// Creates geometry - core method of macro feature generating actual CAD geometry.
        /// </summary>
        /// <param name="app">中文：SOLIDWORKS 应用程序实例 SOLIDWORKS application instance.</param>
        /// <param name="model">中文：当前模型文档 The current model document.</param>
        /// <param name="feat">中文：宏特征实例 The macro feature instance.</param>
        /// <param name="alignDim">中文：输出参数 - 尺寸对齐回调 The output parameter - dimension alignment callback.</param>
        /// <returns>中文：创建的实体数组 The created body array.</returns>
        /// <remarks>
        /// 中文：几何体创建流程：
        ///   1. 从特征参数获取基准面
        ///   2. 计算盒子放置的位置和方向：
        ///      - 使用面的中心点作为盒子原点
        ///      - 使用面的法向作为盒子 Z 轴
        ///      - 使用面的参考方向作为盒子 X 轴
        ///   3. 调用 MemoryGeometryBuilder 创建实体
        ///   4. 设置尺寸对齐回调以支持尺寸拖动
        /// Geometry creation flow:
        ///   1. Get base face from feature parameters
        ///   2. Calculate box placement position and orientation:
        ///      - Use face center point as box origin
        ///      - Use face normal as box Z axis
        ///      - Use face reference direction as box X axis
        ///   3. Call MemoryGeometryBuilder to create solid body
        ///   4. Set dimension alignment callback to support dimension dragging.
        /// </remarks>
        public override ISwBody[] CreateGeometry(ISwApplication app, ISwDocument model,
            ISwMacroFeature<BoxMacroFeatureData> feat, out AlignDimensionDelegate<BoxMacroFeatureData> alignDim)
        {
            var data = feat.Parameters;

            var face = data.BaseFace;

            Xarial.XCad.Geometry.Structures.Point pt;
            Vector dir;
            Vector refDir;

            // 中文：验证基准面状态
            // Validate base face state
            if (face is IFaultObject)
            {
                // 中文：基准面无效（可能是删除的面或无效引用）
                // Throw user-friendly exception
                // Base face is invalid (possibly deleted face or invalid reference)
                // Throw user-friendly exception
                throw new UserException("Base face is a fault entity");
            }
            else if (face is IXPlanarFace)
            {
                // 中文：基准面是平面，获取相对变换矩阵
                // Base face is planar, get relative transform matrix
                var transform = face.GetRelativeTransform(model);

                var plane = ((IXPlanarFace)face).Plane;

                // 中文：获取面的参数域范围，用于计算中心点
                // Get face parameter domain range for center point calculation
                face.GetUVBoundary(out double uMin, out double uMax, out double vMin, out double vMax);

                // 中文：计算面上的中心点，并应用变换矩阵转换到世界坐标系
                // Calculate center point on face and transform to world coordinate system
                pt = face.Definition.CalculateLocation((uMin + uMax) / 2, (vMin + vMax) / 2, out _).Transform(transform);

                // 中文：计算盒子方向向量
                //   - 法向乘以感知方向（sense）决定朝向
                //   - 参考方向用于确定盒子 X 轴
                // Calculate box direction vectors:
                //   - Normal multiplied by sense direction determines facing
                //   - Reference direction determines box X axis
                dir = plane.Normal.Transform(transform) * (face.Sense ? 1 : -1);
                refDir = plane.Reference.Transform(transform);
            }
            else if (face == null)
            {
                // 中文：未指定基准面，使用默认方向（世界坐标系）
                // No base face specified, use default orientation (world coordinate system)
                pt = new Xarial.XCad.Geometry.Structures.Point(0, 0, 0);
                dir = new Vector(0, 0, 1);
                refDir = new Vector(1, 0, 0);
            }
            else
            {
                // 中文：目前只支持平面或空基准面
                // Only planar or null base face is currently supported
                throw new NotSupportedException();
            }

            // 中文：创建实体盒子几何体
            // Create solid box geometry
            var box = (ISwBody)app.MemoryGeometryBuilder.CreateSolidBox(
                pt, dir, refDir,
                data.Width, data.Height, data.Length).Bodies.First();

            // 中文：设置尺寸对齐回调
            // 当用户拖动 Length 尺寸时，自动对齐到盒子边缘
            // Set dimension alignment callback:
            // When user drags Length dimension, automatically align to box edge
            alignDim = new AlignDimensionDelegate<BoxMacroFeatureData>((p, d) =>
            {
                // 中文：只处理 Length 尺寸的对齐
                // Only handle Length dimension alignment
                if (string.Equals(p, nameof(BoxMacroFeatureData.Length)))
                {
                    this.AlignLinearDimension(d, pt, dir);
                }
            });

            return new ISwBody[] { box };
        }

        /// <summary>
        /// 中文：处理编辑过程中的异常
        /// 当几何体创建或其他操作失败时调用
        /// Handles exceptions during editing - called when geometry creation or other operations fail.
        /// </summary>
        /// <param name="feat">中文：出现问题的特征实例 The feature instance with the issue.</param>
        /// <param name="ex">中文：捕获的异常 The caught exception.</param>
        /// <returns>中文：恢复后的特征数据（用于保持特征在有效状态）The recovered feature data (to keep feature in valid state).</returns>
        /// <remarks>
        /// 中文：此方法提供错误恢复机制
        /// 当用户输入导致几何体创建失败时，返回默认值可使特征保持可用状态
        /// This method provides error recovery mechanism.
        /// When user input causes geometry creation failure, returning default values keeps feature usable.
        /// </remarks>
        protected override BoxMacroFeatureData HandleEditingException(IXCustomFeature<BoxMacroFeatureData> feat, Exception ex)
        {
            return new BoxMacroFeatureData();
        }

        /// <summary>
        /// 中文：创建预览几何体
        /// 在属性页中实时显示几何体预览
        /// Creates preview geometry for real-time display in property page.
        /// </summary>
        /// <param name="app">中文：SOLIDWORKS 应用程序实例 SOLIDWORKS application instance.</param>
        /// <param name="model">中文：当前模型文档 The current model document.</param>
        /// <param name="feat">中文：宏特征实例 The macro feature instance.</param>
        /// <param name="page">中文：当前属性页数据 The current property page data.</param>
        /// <param name="shouldHidePreviewEdit">中文：输出参数 - 是否隐藏预览编辑的回调 Output parameter - callback to determine whether to hide preview edit.</param>
        /// <param name="assignPreviewColor">中文：输出参数 - 预览颜色赋值回调 Output parameter - callback to assign preview color.</param>
        /// <returns>中文：预览几何体数组 The preview geometry array.</returns>
        /// <remarks>
        /// 中文：预览几何体显示机制：
        ///   - 在用户编辑参数时实时更新
        ///   - 使用半透明绿色渲染
        ///   - 可选：shouldHidePreviewEdit 控制是否在特定条件下隐藏预览
        /// Preview geometry display mechanism:
        ///   - Updates in real-time when user edits parameters
        ///   - Rendered with semi-transparent green color
        ///   - Optional: shouldHidePreviewEdit controls whether to hide preview under specific conditions
        /// </remarks>
        public override ISwTempBody[] CreatePreviewGeometry(ISwApplication app, ISwDocument model, ISwMacroFeature<BoxMacroFeatureData> feat, BoxPage page,
            out ShouldHidePreviewEditBodyDelegate<BoxMacroFeatureData, BoxPage> shouldHidePreviewEdit, out AssignPreviewBodyColorDelegate assignPreviewColor)
        {
            var date = feat.Parameters;
            shouldHidePreviewEdit = null; // 中文：不隐藏预览 Preview is not hidden
            assignPreviewColor = AssignPreviewBodyColor;
            // 中文：更新属性页中的尺寸显示信息
            // Update dimension display info in property page
            page.Parameters.UpdateSize();
            return CreateGeometry(app, model, feat, out _)?.Cast<ISwTempBody>().ToArray();
        }

        /// <summary>
        /// 中文：分配预览几何体的颜色
        /// 半透明绿色（ARGB: 100, Green）
        /// Assigns color for preview geometry - semi-transparent green (ARGB: 100, Green).
        /// </summary>
        /// <param name="body">中文：需要设置颜色的几何体 The geometry to set color for.</param>
        /// <param name="color">中文：输出的颜色值 The output color value.</param>
        private void AssignPreviewBodyColor(IXBody body, out Color color)
            => color = Color.FromArgb(100, Color.Green);

        /// <summary>
        /// 中文：特征重建后回调
        /// 目前为空实现，可用于特征后处理
        /// Post-rebuild callback - currently empty implementation, can be used for post-feature processing.
        /// </summary>
        /// <param name="app">中文：SOLIDWORKS 应用程序实例 SOLIDWORKS application instance.</param>
        /// <param name="model">中文：当前模型文档 The current model document.</param>
        /// <param name="feature">中文：重建的特征 The rebuilt feature.</param>
        /// <param name="parameters">中文：特征参数 The feature parameters.</param>
        private void OnPostRebuild(ISwApplication app, ISwDocument model, ISwMacroFeature<BoxMacroFeatureData> feature, BoxMacroFeatureData parameters)
        {
        }

        /// <summary>
        /// 中文：特征插入时回调
        /// 初始化属性页数据并清空选择
        /// Callback when feature is being inserted - initializes property page and clears selections.
        /// </summary>
        /// <param name="app">中文：xCAD 应用程序实例 XCAD application instance.</param>
        /// <param name="doc">中文：当前文档 The current document.</param>
        /// <param name="feat">中文：正在插入的特征 The feature being inserted.</param>
        /// <param name="page">中文：属性页数据 The property page data.</param>
        /// <remarks>
        /// 中文：此方法在特征初次插入时调用
        /// 用于设置属性页的初始状态
        /// Called when feature is first inserted.
        /// Used to set initial state of property page.
        /// </remarks>
        public override void OnFeatureInserting(IXApplication app, IXDocument doc, IXCustomFeature<BoxMacroFeatureData> feat, BoxPage page)
        {
            base.OnFeatureInserting(app, doc, feat, page);

            // 中文：重置属性页参数为默认值
            // Reset property page parameters to default values
            page.Parameters.Reset();
            // 中文：清空文档中所有选择（防止残留选择影响用户体验）
            // Clear all selections in document (to prevent residual selections affecting user experience)
            doc.Selections.Clear();
        }
    }
}