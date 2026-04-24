// -*- coding: utf-8 -*- // samples/SwAddIn/PmpData.cs
//*********************************************************************
//xCAD
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
//
// PmpData.cs - 属性管理器页面（PMP）数据模型综合示例
// =====================================================
// 本文件展示了 xCAD 框架中属性管理器页面（PropertyManager Page）的各种控件用法，
// 包括：选择框、下拉框、选项框、复选框、列表框、位图按钮、文本块、WPF 自定义控件等。
// 同时演示了依赖处理器、元数据绑定、自定义项目提供者等高级功能。
//
// 关键设计模式：
// - ViewModel 模式：数据模型实现 INotifyPropertyChanged 支持双向绑定
// - 自定义项目提供者：动态生成下拉框/列表框的选项数据
// - 依赖处理器：实现控件间的联动（如启用/禁用、显示/隐藏）
// - 元数据绑定：通过 [Metadata] 和 [DependentOnMetadata] 实现复杂依赖关系
//
// 中文注释说明：中文注释采用流畅的自然语言解释，而非逐字机械翻译。
// 涉及 SolidWorks/CAD 几何术语时，使用标准中文术语（如"面"、"边线"、"实体"）。
//*********************************************************************

using System.Collections.Generic;
using System.Runtime.InteropServices;
using Xarial.XCad.Features.CustomFeature.Attributes;
using Xarial.XCad.Features.CustomFeature.Enums;
using Xarial.XCad.UI.PropertyPage.Attributes;
using Xarial.XCad.Base.Enums;
using Xarial.XCad.SolidWorks.UI.PropertyPage;
using Xarial.XCad.SolidWorks;
using Xarial.XCad.SolidWorks.Geometry;
using System;
using Xarial.XCad;
using Xarial.XCad.SolidWorks.UI.PropertyPage.Services;
using Xarial.XCad.SolidWorks.Documents;
using System.Collections.ObjectModel;
using Xarial.XCad.UI.PropertyPage.Base;
using Xarial.XCad.Base.Attributes;
using SwAddInExample.Properties;
using System.Linq;
using System.ComponentModel;
using Xarial.XCad.UI.PropertyPage.Services;
using Xarial.XCad.UI.PropertyPage.Enums;
using Xarial.XCad.UI.PropertyPage.Structures;
using Xarial.XCad.Enums;
using Xarial.XCad.Features;
using Xarial.XCad.Geometry;
using Xarial.XCad.SolidWorks.UI.PropertyPage.Attributes;
using SolidWorks.Interop.swconst;

namespace SwAddInExample
{
    /// <summary>
    /// 简单枚举，用于属性管理器页面中选项框（OptionBox）和下拉框（ComboBox）控件的静态选项。
    /// </summary>
    /// <remarks>
    /// 选项框控件将枚举值以单选按钮组的形式呈现，用户只能选择其中一个选项。
    /// 中文：用于属性管理器页面中选项框和下拉框控件的简单枚举
    /// </remarks>
    public enum Opts
    {
        /// <summary>选项 1</summary>
        Opt1,
        /// <summary>选项 2</summary>
        Opt2,
        /// <summary>选项 3</summary>
        Opt3
    }

    /// <summary>
    /// 第二个枚举，用于演示多个独立的选项框分组。
    /// </summary>
    /// <remarks>
    /// 当页面中有多个 OptionBox 控件时，每个控件可以绑定到不同的枚举类型，
    /// 从而实现互相独立的选项组。
    /// 中文：用于演示多个独立选项框分组的第二个枚举
    /// </remarks>
    public enum Opts1
    {
        /// <summary>选项 4</summary>
        Opt4,
        /// <summary>选项 5</summary>
        Opt5,
        /// <summary>选项 6</summary>
        Opt6
    }

    /// <summary>
    /// 位标志枚举（Bit-Flag Enum），每个成员代表二的幂次位，可通过按位 OR 运算组合使用。
    /// </summary>
    /// <remarks>
    /// 位标志枚举适用于复选框列表（CheckBoxList），用户可以同时选择多个选项，
    /// 每个选中项对应一个位标志。组合值（如 Opt1_2）表示同时选中多个标志的情况。
    /// 中文：位标志枚举；每个成员代表二的幂次位，可通过按位 OR 组合使用
    /// </remarks>
    [Flags]
    public enum OptsFlag
    {
        /// <summary>选项 1（位值：1）</summary>
        Opt1 = 1,
        /// <summary>选项 2（位值：2）</summary>
        Opt2 = 2,
        /// <summary>选项 3（位值：4）</summary>
        Opt3 = 4,
        /// <summary>选项 4（位值：8）</summary>
        Opt4 = 8
    }

    /// <summary>
    /// 带标题和描述信息的位标志枚举，用于在复选框列表控件中显示丰富的选项名称和说明。
    /// </summary>
    /// <remarks>
    /// 通过 [Title] 和 [Description] 特性为每个枚举值设置友好的显示文本，
    /// 这些信息会在 CheckBoxList 控件中作为选项标签和工具提示展示。
    /// 中文：带有 Title/Description 特性的位标志枚举，用于在复选框列表控件中显示丰富的名称和描述
    /// </remarks>
    [Flags]
    public enum OptsFlag2
    {
        /// <summary>无任何选项选中</summary>
        None = 0,

        /// <summary>选项 1（带自定义标题）</summary>
        [Title("Option #1")]
        [Description("First Option")]
        Opt1 = 1,
        /// <summary>选项 2（使用枚举名称作为默认标题）</summary>
        Opt2 = 2,

        // Combined value representing both Opt1 and Opt2 selected simultaneously
        // 中文：表示同时选中 Opt1 和 Opt2 的组合值
        /// <summary>选项 1 和 2 的组合值</summary>
        [Title("Opt1 + Opt2")]
        Opt1_2 = Opt1 | Opt2,

        /// <summary>选项 3</summary>
        Opt3 = 4,
        /// <summary>选项 4</summary>
        Opt4 = 8
    }

    /// <summary>
    /// WPF 自定义控件的数据上下文（视图模型）。
    /// </summary>
    /// <remarks>
    /// 实现 INotifyPropertyChanged 以支持 WPF 双向数据绑定（Two-Way Binding）。
    /// 当属性值变化时，ValueChanged 事件会通知属性管理器页面更新数据模型。
    /// 这是 MVVM 模式在 xCAD PMP 中的典型应用。
    /// 中文：WPF 自定义控件的数据上下文（视图模型）；实现 INotifyPropertyChanged 以支持双向数据绑定
    /// </remarks>
    public class CustomControlDataContext : INotifyPropertyChanged
    {
        /// <summary>
        /// 当自定义控件的值（Value）发生变化时触发，通知属性管理器页面数据已更新。
        /// </summary>
        /// <remarks>
        /// 此事件用于在 WPF 自定义控件和 PMP 数据模型之间建立桥接，
        /// 使自定义控件的值变化能够反映到属性管理器页面。
        /// 中文：当 Value 属性更改时触发，通知属性管理器页面自定义控件的值已更新
        /// </remarks>
        public event Action<CustomControlDataContext, OptsFlag> ValueChanged;

        /// <summary>
        /// 标准 INotifyPropertyChanged 事件，用于 WPF 数据绑定。
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private OptsFlag m_Value;

        /// <summary>
        /// 自定义控件的当前值，通过 IXCustomControl.Value 与属性管理器页面进行双向绑定。
        /// </summary>
        /// <value>
        /// 一个 OptsFlag 位标志值，表示用户在自定义控件中的选择状态。
        /// </value>
        /// <remarks>
        /// 设置此属性时会触发 ValueChanged 和 PropertyChanged 两个事件，
        /// 确保无论是 xCAD 框架还是 WPF 绑定都能感知到变化。
        /// 中文：自定义控件的当前值，通过 IXCustomControl.Value 与属性管理器页面进行双向绑定
        /// </remarks>
        public OptsFlag Value
        {
            get => m_Value;
            set
            {
                m_Value = value;
                // Notify the PMP that the value has changed so it can update its data model
                // 中文：通知属性管理器页面值已更改，以便其更新数据模型
                ValueChanged?.Invoke(this, value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
            }
        }

        /// <summary>
        /// 构造函数：预填充示例项目，供 WPF 控件的列表展示使用。
        /// </summary>
        /// <remarks>
        /// 初始化时添加两个示例 Item，用于演示 WPF 列表控件的数据绑定。
        /// 中文：构造函数：预填充示例项目，供 WPF 控件的列表展示使用
        /// </remarks>
        public CustomControlDataContext()
        {
            Items = new ObservableCollection<Item>();
            Items.Add(new Item() { Name = "ABC", Value = "XYZ" });
            Items.Add(new Item() { Name = "ABC1", Value = "XYZ1" });
        }

        /// <summary>
        /// 在 WPF 控件列表中显示的可观察项目集合。
        /// </summary>
        /// <remarks>
        /// ObservableCollection 在项目添加、删除或刷新时会自动通知 WPF 绑定系统，
        /// 无需手动调用 PropertyChanged。
        /// 中文：在 WPF 控件列表中显示的可观察项目集合
        /// </remarks>
        public ObservableCollection<Item> Items { get; }
    }

    /// <summary>
    /// WPF 自定义控件项目列表使用的简单键值对数据模型。
    /// </summary>
    /// <remarks>
    /// 此类用于 WPF 的 ItemsControl（如 ListBox、ComboBox）的数据绑定场景，
    /// 提供 Name-Value 键值对结构。
    /// 中文：WPF 自定义控件项目列表使用的简单键值对数据模型
    /// </remarks>
    public class Item
    {
        /// <summary>项目名称</summary>
        public string Name { get; set; }
        /// <summary>项目值</summary>
        public string Value { get; set; }
    }

    /// <summary>
    /// 自定义项目提供者（CustomItemsProvider）使用的示例数据模型。
    /// </summary>
    /// <remarks>
    /// 此类为 ComboBox、OptionBox、CheckBoxList 等控件提供数据源。
    /// 支持通过 [Title] 和 [Description] 特性设置自定义显示文本。
    /// 中文：自定义项目提供者使用的示例数据模型，用于下拉框、选项框和复选框列表控件
    /// </remarks>
    public class MyItem
    {
        /// <summary>
        /// 自定义项子类：名称为"C"，ID 为 3。
        /// </summary>
        /// <remarks>
        /// 嵌套类用于演示如何创建带有特殊属性的自定义选项。
        /// 通过 [Title] 和 [Description] 特性可覆盖默认的显示名称。
        /// </remarks>
        [Title("Custom Item C")]
        [Description("Item C [ID = 3]")]
        private class MyCustomItem : MyItem
        {
            internal MyCustomItem()
            {
                Name = "C";
                Id = 3;
            }
        }

        /// <summary>
        /// 预定义的所有 MyItem 实例数组。
        /// </summary>
        public static MyItem[] All { get; } = new MyItem[]
        {
            new MyItem()
            {
                Name = "A",
                Id = 1
            },
            new MyItem()
            {
                Name = "B",
                Id = 2
            },
            new MyCustomItem()
        };

        /// <summary>项目名称</summary>
        public string Name { get; set; }
        /// <summary>项目唯一标识符</summary>
        public int Id { get; set; }

        /// <summary>
        /// 返回项目的显示名称（Name 属性）。
        /// </summary>
        public override string ToString() => Name;

        /// <summary>
        /// 基于 Id 判断两个 MyItem 实例是否相等。
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj is MyItem)
            {
                return (obj as MyItem).Id == Id;
            }

            return false;
        }

        /// <summary>
        /// 返回固定的哈希码（0），因为 Id 已用于相等性判断。
        /// </summary>
        public override int GetHashCode()
        {
            return 0;
        }
    }

    /// <summary>
    /// 替代的 MyItem 实现，使用自定义的 DisplayName 结构。
    /// </summary>
    /// <remarks>
    /// 此类演示了如何为下拉框等控件提供自定义的显示名称。
    /// DisplayName 属性返回带方括号修饰的字符串格式。
    /// </remarks>
    public class MyItem1
    {
        /// <summary>
        /// 内部类，用于包装显示名称。
        /// </summary>
        public class MyItem1Name
        {
            /// <summary>显示名称</summary>
            public string Name { get; }

            /// <summary>
            /// 构造函数。
            /// </summary>
            /// <param name="name">显示名称</param>
            public MyItem1Name(string name)
            {
                Name = name;
            }
        }

        /// <summary>原始值</summary>
        public string Value { get; }

        /// <summary>显示名称（带方括号装饰）</summary>
        public MyItem1Name DisplayName { get; }

        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="val">原始值</param>
        /// <remarks>
        /// 构造函数自动创建 DisplayName，格式为"[" + val + "]"。
        /// </remarks>
        public MyItem1(string val)
        {
            Value = val;
            DisplayName = new MyItem1Name("[" + val + "]");
        }

        /// <summary>
        /// 返回原始值作为显示文本。
        /// </summary>
        public override string ToString() => Value;
    }

    /// <summary>
    /// 自定义项目提供者：为使用 [ComboBox/OptionBox/CheckBoxList(typeof(MyCustomItemsProvider))]
    /// 装饰的控件提供 MyItem[] 数据源。
    /// </summary>
    /// <remarks>
    /// SwCustomItemsProvider 是 xCAD 框架提供的基础类，
    /// 用于动态生成控件的项目列表。此处返回预定义的 MyItem.All 数组。
    /// 中文：自定义项目提供者：为使用相应特性装饰的控件提供 MyItem[] 数据
    /// </remarks>
    public class MyCustomItemsProvider : SwCustomItemsProvider<MyItem>
    {
        /// <summary>
        /// 提供项目列表。
        /// </summary>
        /// <param name="app">SolidWorks 应用程序实例</param>
        /// <param name="dependencies">依赖的控件数组（当前未使用）</param>
        /// <returns>MyItem 实例数组</returns>
        public override IEnumerable<MyItem> ProvideItems(ISwApplication app, IControl[] dependencies)
            => MyItem.All;
    }

    /// <summary>
    /// 自定义项目提供者，根据关联控件的当前值动态返回字符串列表。
    /// </summary>
    /// <remarks>
    /// 此提供者演示了如何根据依赖控件的值动态生成选项列表。
    /// 在本例中，根据 ComboBox 中选中的 MyItem 的 Name 属性，
    /// 动态生成 "1_Name"、"2_Name" 等格式的选项。
    /// 中文：自定义项目提供者，根据关联控件的当前值动态返回字符串列表
    /// </remarks>
    public class MyCustomItems1Provider : SwCustomItemsProvider<string>
    {
        /// <summary>
        /// 根据依赖控件的值动态提供项目列表。
        /// </summary>
        /// <param name="app">SolidWorks 应用程序实例</param>
        /// <param name="dependencies">依赖控件数组，第一个元素通常是 ComboBox</param>
        /// <returns>根据依赖值生成的字符串数组；如果无有效依赖则返回 null</returns>
        /// <remarks>
        /// 此方法从第一个依赖控件获取 MyItem，然后基于其 Name 属性
        /// 生成格式化的选项列表。
        /// 中文：获取第一个依赖控件（如下拉框）的值，以生成与上下文相关的项目列表
        /// </remarks>
        public override IEnumerable<string> ProvideItems(ISwApplication app, IControl[] dependencies)
        {
            // Get the value of the first dependency control (e.g., a ComboBox) to generate context-sensitive items
            // 中文：获取第一个依赖控件（如下拉框）的值，以生成与上下文相关的项目列表
            var item = dependencies.First()?.GetValue() as MyItem;

            if (item != null)
            {
                return new string[]
                {
                    "1_" + item.Name,
                    "2_" + item.Name,
                    "3_" + item.Name,
                    "4_" + item.Name
                };
            }
            else
            {
                return null;
            }
        }
    }


    /// <summary>
    /// 自定义选择过滤器：仅允许选择平面（Planar Face）。
    /// </summary>
    /// <remarks>
    /// 实现 ISelectionCustomFilter 接口，用于高级选择框场景。
    /// 此过滤器通过检查所选面的几何表面是否为平面来判断是否接受该选择。
    /// 如果选择被拒绝，会在 args.Reason 中设置拒绝原因，用户将看到工具提示。
    /// 中文：自定义选择过滤器，仅允许在选择框中选择平面
    /// </remarks>
    public class PlanarFaceFilter : ISelectionCustomFilter
    {
        /// <summary>
        /// 过滤回调：对每个候选选择项调用。
        /// </summary>
        /// <param name="selBox">所属的选择框控件</param>
        /// <param name="selection">候选的 SolidWorks 选择对象</param>
        /// <param name="args">输出参数：Filter（是否接受）、ItemText（显示文本）、Reason（拒绝原因）</param>
        /// <remarks>
        /// 设置 args.Filter=true 表示接受选择，false 表示拒绝。
        /// 接受时可通过 args.ItemText 自定义选择框中的显示文本。
        /// 拒绝时应在 args.Reason 中提供人类可读的解释。
        /// 中文：过滤回调：对每个候选选择项调用；设置 args.Filter=true 表示接受，false 表示拒绝
        /// </remarks>
        public void Filter(IControl selBox, IXSelObject selection, SelectionCustomFilterArguments args)
        {
            args.Filter = (selection as ISwFace).Face.IGetSurface().IsPlane(); //validating the selection and only allowing planar face
            // 中文：验证所选面是否为平面，仅允许平面通过过滤器

            if (args.Filter)
            {
                // Display "Planar Face" as the item label in the selection box when accepted
                // 中文：选择通过时，在选择框中将该项标记为"Planar Face"
                args.ItemText = "Planar Face";
            }
            else
            {
                // Provide a rejection reason shown as a tooltip when the selection is invalid
                // 中文：提供拒绝原因，选择无效时以提示信息的形式显示给用户
                args.Reason = "Only planar faces can be selected";
            }
        }
    }

    /// <summary>
    /// 依赖处理器：根据布尔类型依赖控件的值来显示或隐藏目标控件。
    /// </summary>
    /// <remarks>
    /// 实现 IDependencyHandler 接口，用于控件联动。
    /// 此处理器监听依赖控件的值变化，动态更新源控件的 Visible 属性。
    /// 当依赖控件为 true 时，源控件显示；为 false 时，源控件隐藏。
    /// 中文：依赖处理器，根据布尔类型依赖控件的值来显示或隐藏目标控件
    /// </remarks>
    public class VisibilityHandler : IDependencyHandler
    {
        /// <summary>
        /// 状态更新回调：每当依赖控件的值更改时调用。
        /// </summary>
        /// <param name="app">SolidWorks 应用程序实例</param>
        /// <param name="source">需要更新状态的源控件（受依赖控制的控件）</param>
        /// <param name="dependencies">依赖控件数组</param>
        /// <remarks>
        /// 此方法根据第一个依赖控件的布尔值切换源控件的显示/隐藏状态。
        /// 中文：UpdateState：每当依赖控件的值更改时调用
        /// </remarks>
        public void UpdateState(IXApplication app, IControl source, IControl[] dependencies)
        {
            // Toggle the visibility of the source control based on the first dependency's boolean value
            // 中文：根据第一个依赖控件的布尔值切换源控件的显示/隐藏状态
            source.Visible = (bool)dependencies.First().GetValue();
        }
    }

    /// <summary>
    /// 依赖处理器：根据是否设置了 Opt2 标志位来启用或禁用目标控件。
    /// </summary>
    /// <remarks>
    /// 此类演示了如何基于位标志枚举的特定标志位来控制控件的启用状态。
    /// 当自定义控件的 Value 包含 Opt2 标志时，源控件被启用；否则被禁用。
    /// 中文：依赖处理器，根据是否设置了 Opt2 标志位来启用或禁用目标控件
    /// </remarks>
    public class CustomControlDependantHandler : IDependencyHandler
    {
        /// <summary>
        /// 状态更新回调：仅当自定义控件设置了 Opt2 标志时才启用源控件。
        /// </summary>
        /// <param name="app">SolidWorks 应用程序实例</param>
        /// <param name="source">需要更新启用状态的源控件</param>
        /// <param name="dependencies">依赖控件数组，第一个元素为自定义控件</param>
        /// <remarks>
        /// 使用 HasFlag 方法检查 OptsFlag 枚举值中是否包含 Opt2 位。
        /// 中文：UpdateState：仅当自定义控件设置了 Opt2 标志时才启用源控件
        /// </remarks>
        public void UpdateState(IXApplication app, IControl source, IControl[] dependencies)
        {
            var val = (OptsFlag)dependencies.First().GetValue();

            // Enable the control only if the Opt2 bit is present in the dependency value
            // 中文：仅当依赖值中包含 Opt2 位时才启用该控件
            source.Enabled = val.HasFlag(OptsFlag.Opt2);
        }
    }

    /// <summary>
    /// 主属性管理器页面（PMP）数据模型。
    /// </summary>
    /// <remarks>
    /// 继承 SwPropertyManagerPageHandler 以处理 SolidWorks 属性管理器页面的生命周期事件。
    /// 本类演示了 xCAD 框架支持的几乎所有控件类型：
    /// - 选择框（SelectionBox）：用于选择 SolidWorks 对象（零部件、面、边线、实体等）
    /// - 下拉框（ComboBox）：静态整数值、自定义数据源、动态依赖选项
    /// - 选项框（OptionBox）：枚举类型选项，以单选按钮组形式呈现
    /// - 复选框（CheckBox）：位图切换按钮、bitmap button 等
    /// - 列表框（ListBox）：静态字符串数组、数值数组、自定义对象列表
    /// - 复选框列表（CheckBoxList）：支持多选的位标志枚举
    /// - 文本块（TextBlock）：带格式的只读文本显示区域
    /// - 自定义控件（CustomControl）：嵌入的 WPF/WinForms 控件
    /// - 动态控件（DynamicControls）：运行时动态添加的控件
    ///
    /// 依赖关系通过 [DependentOn] 和 [DependentOnMetadata] 特性实现，
    /// 支持控件间的联动（如显示/隐藏、启用/禁用）。
    /// 元数据绑定通过 [Metadata] 和 [AttachMetadata] 特性实现复杂的数据关联。
    ///
    /// 中文：插件示例的主属性管理器页面（PMP）数据模型
    /// </remarks>
    [ComVisible(true)]
    [Help("https://xcad.net/")]
    //[PageOptions(PageOptions_e.OkayButton | PageOptions_e.CancelButton | PageOptions_e.HandleKeystrokes)]
    public class PmpData : SwPropertyManagerPageHandler, INotifyPropertyChanged
    {
        /// <summary>
        /// 属性变更事件，用于通知 UI 更新。
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 任意 SolidWorks 对象的选择框；当前过滤为注释表格（AnnotationTables）类型。
        /// </summary>
        /// <remarks>
        /// 此选择框使用 SwSelectionBoxOptions 指定了只允许选择注释表格对象。
        /// SelectionBox 控件支持单选或多选模式，取决于目标属性的类型（单一对象或集合）。
        /// 中文：任意 SolidWorks 对象的选择框；当前过滤为注释表格类型
        /// </remarks>
        //[SelectionBoxOptions(Filters = new Type[] { typeof(IXFace) })]
        [SwSelectionBoxOptions(Filters = new swSelectType_e[] { swSelectType_e.swSelANNOTATIONTABLES })]
        public ISwSelObject UnknownObject { get; set; }

        /// <summary>
        /// 直接嵌入属性管理器页面的 WPF 自定义控件。
        /// </summary>
        /// <value>
        /// 一个 OptsFlag 位标志值，表示自定义控件的当前状态。
        /// </value>
        /// <remarks>
        /// [CustomControl] 特性指定了 WPF 用户控件的类型。
        /// [ControlOptions(height: 300)] 设置控件高度为 300 像素。
        /// [ControlTag] 将此控件绑定到 CustomControl 属性，实现双向数据绑定。
        /// 中文：直接嵌入属性管理器页面的 WPF 自定义控件；其值为 OptsFlag 位字段
        /// </remarks>
        [CustomControl(typeof(WpfUserControl))]
        //[CustomControl(typeof(WinUserControl))]
        [ControlOptions(height: 300)]
        [ControlTag(nameof(CustomControl))]
        public OptsFlag CustomControl { get; set; }

        /// <summary>
        /// 通用对象选择框，其启用状态取决于 CustomControl 是否包含 Opt2 标志。
        /// </summary>
        /// <remarks>
        /// [DependentOn] 特性将此控件绑定到 CustomControlDependantHandler 依赖处理器。
        /// 当 CustomControl 包含 Opt2 标志时，此选择框启用；否则禁用。
        /// [Description] 设置鼠标悬停时显示的工具提示文本。
        /// 中文：选择框，其启用状态取决于 CustomControl 是否包含 Opt2 标志
        /// </remarks>
        [DependentOn(typeof(CustomControlDependantHandler), nameof(CustomControl))]
        [Description("Any object selection")]
        public ISwSelObject AnyObject { get; set; }

        /// <summary>
        /// 仅用于选择平面（Planar Face）的选择框。
        /// </summary>
        /// <remarks>
        /// 同时使用标准类型过滤器（swSelFACES）和自定义 PlanarFaceFilter 过滤器。
        /// PlanarFaceFilter 通过 ISwFace.Face.IGetSurface().IsPlane() 验证面是否为平面。
        /// [AttachMetadata] 将 Components 和 CircEdge 的数据附加到此控件的选择结果上。
        /// 中文：仅用于选择平面的选择框；同时使用标准类型过滤（面）和自定义 PlanarFaceFilter
        /// </remarks>
        [SwSelectionBoxOptions(CustomFilter = typeof(PlanarFaceFilter), Filters = new swSelectType_e[] { swSelectType_e.swSelFACES })] //setting the standard filter to faces and custom filter to only filter planar faces
        // 中文：将标准过滤器设置为面类型，并将自定义过滤器设置为仅允许平面
        [AttachMetadata(nameof(ComponentsMetadata))]
        [AttachMetadata(nameof(CircEdgeMetadata))]
        public ISwFace PlanarFace { get; set; }

        /// <summary>
        /// 装配体零部件的多选列表。
        /// </summary>
        /// <remarks>
        /// 此属性绑定到组件选择框，支持选择多个零部件。
        /// 当列表发生变化时，需要触发 PropertyChanged 事件以刷新 UI。
        /// 中文：装配体零部件的多选列表
        /// </remarks>
        public List<ISwComponent> Components { get; set; }

        /// <summary>
        /// 实体（Solid Body）的选择框，属性管理器页面打开时自动获取输入焦点。
        /// </summary>
        /// <remarks>
        /// [SelectionBoxOptions(Focused = true)] 使此选择框在 PMP 打开时自动获得焦点，
        /// 方便用户立即进行选择操作。
        /// 中文：实体的选择框；属性管理器页面打开时自动获取输入焦点
        /// </remarks>
        [SelectionBoxOptions(Focused = true)]
        public ISwBody Body { get; set; }

        /// <summary>
        /// Components 的元数据版本，用于依赖处理器。
        /// </summary>
        /// <remarks>
        /// [Metadata] 特性将此属性标记为元数据，可被其他控件通过 [DependentOnMetadata] 引用。
        /// 此属性返回 Components 列表，供 IsCheckedDepHandler 等元数据依赖处理器使用。
        /// </remarks>
        [Metadata(nameof(ComponentsMetadata))]
        public List<ISwComponent> ComponentsMetadata => Components;

        /// <summary>
        /// CircEdge 的元数据版本。
        /// </summary>
        [Metadata(nameof(CircEdgeMetadata))]
        public ISwCircularEdge CircEdgeMetadata => CircEdge;

        /// <summary>
        /// 圆形边线（Circular Edge）选择框。
        /// </summary>
        public ISwCircularEdge CircEdge { get; set; }

        private string m_TextBlockText = "Hello World";

        /// <summary>
        /// 带格式的只读文本块，显示在属性管理器页面中。
        /// </summary>
        /// <remarks>
        /// [TextBlock] 特性标识此属性为文本块控件（非输入控件）。
        /// [TextBlockOptions] 设置文本对齐方式为居中，字体样式为粗体和斜体。
        /// [ControlOptions] 设置背景色为黄色，文字颜色为绿色。
        /// 中文：带格式的只读文本块
        /// </remarks>
        [TextBlock]
        [TextBlockOptions(TextAlignment_e.Center, FontStyle_e.Bold | FontStyle_e.Italic)]
        [ControlOptions(backgroundColor: System.Drawing.KnownColor.Yellow, textColor: System.Drawing.KnownColor.Green)]
        public string TextBlockText => m_TextBlockText;

        /// <summary>
        /// 位图切换按钮（Bitmap Toggle Button），使用 96x96 像素的垂直和水平图标。
        /// </summary>
        /// <remarks>
        /// [BitmapToggleButton] 特性定义了一个开关按钮，有两种状态（选中/未选中），
        /// 每种状态对应一个位图资源。此按钮默认处于选中状态（true）。
        /// 中文：位图切换按钮，使用 96x96 像素的垂直和水平图标
        /// </remarks>
        [BitmapToggleButton(typeof(Resources), nameof(Resources.vertical), nameof(Resources.horizontal), 96, 96)]
        [Description("Dynamic icon1")]
        public bool CheckBox1 { get; set; } = true;

        /// <summary>
        /// 位图切换按钮，应用灰度和透明效果，尺寸为 24x24 像素。
        /// </summary>
        /// <remarks>
        /// [BitmapEffect_e.Grayscale | BitmapEffect_e.Transparent] 为按钮图标应用
        /// 灰度和透明效果。
        /// 中文：位图切换按钮，应用灰度和透明效果
        /// </remarks>
        [BitmapToggleButton(typeof(Resources), nameof(Resources.vertical), BitmapEffect_e.Grayscale | BitmapEffect_e.Transparent, 24, 24)]
        [Description("Dynamic icon2")]
        public bool CheckBox2 { get; set; } = false;

        /// <summary>
        /// 位图按钮（Bitmap Button），尺寸 48x48 像素。
        /// </summary>
        /// <remarks>
        /// [BitmapButton] 特性定义了一个普通按钮控件，显示位图图标。
        /// 点击此按钮会触发 Action 委托（由 Button 属性指定）。
        /// 中文：位图按钮，尺寸 48x48 像素
        /// </remarks>
        [BitmapButton(typeof(Resources), nameof(Resources.horizontal), 48, 48)]
        public bool CheckBox { get; set; }

        /// <summary>
        /// 位图按钮，点击时执行 ReduceComponents 操作以移除最后一个选中的零部件。
        /// </summary>
        /// <remarks>
        /// Button 属性在构造函数中被赋值为 ReduceComponents 方法。
        /// 此按钮用于演示如何从装配体中动态移除零部件。
        /// 中文：位图按钮，点击时执行减少零部件操作
        /// </remarks>
        [BitmapButton(typeof(Resources), nameof(Resources.xarial))]
        public Action Button { get; }

        /// <summary>
        /// 动作按钮，点击时更新文本块内容（追加 GUID）。
        /// </summary>
        /// <remarks>
        /// [Title] 和 [Description] 特性设置按钮的显示标题和工具提示。
        /// Button1 在构造函数中被赋值为 lambda 表达式，每次点击时
        /// 在 m_TextBlockText 后追加新的 GUID，并通知 PMP 刷新显示。
        /// 中文：动作按钮，点击时更新文本块内容
        /// </remarks>
        [Title("Action Button")]
        [Description("Sample button")]
        public Action Button1 { get; }

        /// <summary>
        /// 动态控件字典，用于运行时动态创建和管理控件。
        /// </summary>
        /// <remarks>
        /// [DynamicControls] 特性使属性管理器页面能够根据字典内容
        /// 动态添加控件。键为控件标识符，值为控件初始数据。
        /// 中文：动态控件字典，用于运行时动态创建控件
        /// </remarks>
        [DynamicControls("_Test_")]
        public Dictionary<string, object> DynamicControls { get; }

        //public List<string> List { get; set; }

        /// <summary>
        /// 静态整数下拉框，提供 1-5 的选项。
        /// </summary>
        /// <remarks>
        /// [ComboBox(1, 2, 3, 4, 5)] 直接指定整数选项列表。
        /// [Label] 在下拉框左侧显示标签文本。
        /// [ControlLeftAlign_e.Indent] 使标签缩进显示。
        /// 中文：静态整数下拉框，提供 1-5 的选项
        /// </remarks>
        [ComboBox(1, 2, 3, 4, 5)]
        [Label("Static Combo Box:", ControlLeftAlign_e.Indent)]
        public int StaticComboBox { get; set; }

        /// <summary>
        /// MyItem1 类型的对象数组，作为下拉框和列表框的数据源。
        /// </summary>
        /// <remarks>
        /// [Metadata("_SRC_")] 将此数组标记为元数据，可通过 "_SRC_" 标识符被其他控件引用。
        /// 中文：MyItem1 对象数组，作为数据源
        /// </remarks>
        [Metadata("_SRC_")]
        public MyItem1[] Source { get; } = new MyItem1[] { new MyItem1("X"), new MyItem1("Y"), new MyItem1("Z") };

        /// <summary>
        /// 下拉框，使用 Source 元数据作为数据源。
        /// </summary>
        /// <remarks>
        /// [ComboBox(ItemsSource = "_SRC_")] 通过元数据标识符引用 Source 数组作为数据源。
        /// 控件会自动调用 ToString() 方法获取显示文本。
        /// 中文：使用元数据作为数据源的下拉框
        /// </remarks>
        [ComboBox(ItemsSource = "_SRC_")]
        public MyItem1 ItemsSourceComboBox { get; set; }

        /// <summary>
        /// 列表框，使用 Source 元数据，显示 DisplayName.Name 属性。
        /// </summary>
        /// <remarks>
        /// [ListBox(ItemsSource = "_SRC_", DisplayMemberPath = "DisplayName.Name")] 指定
        /// 数据源和显示成员路径。列表框支持单选模式。
        /// [Label] 设置标签文本，[ControlOptions] 设置缩进对齐。
        /// 中文：列表框，显示 DisplayName.Name 属性
        /// </remarks>
        [ListBox(ItemsSource = "_SRC_", DisplayMemberPath = "DisplayName.Name")]
        [Label("List Box1:", ControlLeftAlign_e.LeftEdge, FontStyle_e.Bold)]
        [ControlOptions(align: ControlLeftAlign_e.Indent)]
        public MyItem1 ListBox1 { get; set; }

        /// <summary>
        /// 字符串列表框，提供静态选项 "A1", "A2", "A3"。
        /// </summary>
        [ListBox("A1", "A2", "A3")]
        public string ListBox2 { get; set; }

        /// <summary>
        /// 整数列表框，提供静态选项 1, 2, 3, 4。
        /// </summary>
        /// <remarks>
        /// 支持多选模式，返回 List&lt;int&gt; 类型。
        /// 中文：整数列表框，提供静态选项
        /// </remarks>
        [ListBox(1, 2, 3, 4)]
        public List<int> ListBox3 { get; set; }

        //[ListBox]
        /// <summary>
        /// 选项框（OptionBox），绑定到 Opts 枚举类型。
        /// </summary>
        /// <remarks>
        /// [OptionBox] 将枚举类型以单选按钮组形式显示。
        /// [Label] 设置标签文本，[FontStyle_e.Underline] 添加下划线样式。
        /// 中文：选项框，绑定到 Opts 枚举类型
        /// </remarks>
        [OptionBox]
        [Label("Sample Option Box 4:", fontStyle: FontStyle_e.Underline)]
        public Opts OptionBox4 { get; set; }

        /// <summary>
        /// 选项框，绑定到 Opts1 枚举类型（第二个独立选项组）。
        /// </summary>
        /// <remarks>
        /// Opts1 是一个独立的枚举，不同于 Opts，因此此选项框与 OptionBox4 互不影响。
        /// 中文：选项框，绑定到 Opts1 枚举类型
        /// </remarks>
        [OptionBox]
        [Label("Sample Option Box 5:")]
        public Opts1 OptionBox5 { get; set; }

        /// <summary>
        /// 选项框，提供静态整数选项 1, 2, 3, 4。
        /// </summary>
        /// <remarks>
        /// 默认值为 3（在构造函数中设置）。
        /// 中文：选项框，提供静态整数选项
        /// </remarks>
        [OptionBox(1, 2, 3, 4)]
        public int OptionBox6 { get; set; }

        /// <summary>
        /// 选项框，使用自定义项目提供者（MyCustomItemsProvider）动态生成选项。
        /// </summary>
        /// <remarks>
        /// [OptionBox(typeof(MyCustomItemsProvider))] 指定使用自定义提供者生成选项列表。
        /// 这允许在运行时从数据库、API 等来源获取选项数据。
        /// 中文：选项框，使用自定义项目提供者
        /// </remarks>
        [OptionBox(typeof(MyCustomItemsProvider))]
        public MyItem OptionBox7 { get; set; }

        /// <summary>
        /// 列表框，绑定到位标志枚举类型，支持多选。
        /// </summary>
        /// <remarks>
        /// 初始值为 Opt1 | Opt3（选中第一和第三个选项）。
        /// [ListBox] 用于位标志枚举时呈现为多选列表框。
        /// 中文：列表框，绑定到位标志枚举类型
        /// </remarks>
        [ListBox]
        public OptsFlag ListBox5 { get; set; } = OptsFlag.Opt1 | OptsFlag.Opt3;

        /// <summary>
        /// 复选框列表，使用 OptsFlag2 枚举（带 Title/Description）。
        /// </summary>
        /// <remarks>
        /// [CheckBoxList] 用于位标志枚举，每个标志对应一个复选框。
        /// [CheckBoxListOptions] 应用默认选项列表样式。
        /// [ControlOptions(align: ControlLeftAlign_e.Indent)] 设置缩进对齐。
        /// 中文：复选框列表，使用带描述的枚举类型
        /// </remarks>
        [CheckBoxList]
        [CheckBoxListOptions]
        [ControlOptions(align: ControlLeftAlign_e.Indent)]
        public OptsFlag2 FlagEnumCheckBoxes { get; set; }

        /// <summary>
        /// 整数复选框列表，提供静态选项 1, 2, 3, 4。
        /// </summary>
        /// <remarks>
        /// 支持多选，返回 List&lt;int&gt; 类型。
        /// 中文：整数复选框列表，提供静态选项
        /// </remarks>
        [CheckBoxList(1, 2, 3, 4)]
        public List<int> CheckBoxList2 { get; set; }

        /// <summary>
        /// 复选框列表，使用自定义项目提供者（MyCustomItemsProvider）。
        /// </summary>
        /// <remarks>
        /// 每个 MyItem 实例对应一个复选框，支持多选。
        /// 初始预选第一项（在构造函数中设置）。
        /// 中文：复选框列表，使用自定义项目提供者
        /// </remarks>
        [CheckBoxList(typeof(MyCustomItemsProvider))]
        public List<MyItem> CheckBoxList3 { get; set; }

        /// <summary>
        /// 布尔复选框，用于控制其他控件的可见性。
        /// </summary>
        /// <remarks>
        /// [ControlTag(nameof(Visible))] 将此控件标记为 "Visible"。
        /// VisibilityHandler 依赖处理器监听此控件的变化，
        /// 并据此显示或隐藏 Number 数值控件。
        /// 中文：布尔复选框，控制其他控件的可见性
        /// </remarks>
        [ControlTag(nameof(Visible))]
        public bool Visible { get; set; }

        /// <summary>
        /// 数值控件（双精度浮点数），其可见性受 Visible 复选框控制。
        /// </summary>
        /// <remarks>
        /// [DependentOn(typeof(VisibilityHandler), nameof(Visible))] 将此控件绑定到
        /// VisibilityHandler 依赖处理器。当 Visible 为 true 时显示，为 false 时隐藏。
        /// [Label] 设置控件的显示标签。
        /// 中文：数值控件，其可见性受 Visible 复选框控制
        /// </remarks>
        [DependentOn(typeof(VisibilityHandler), nameof(Visible))]
        [Label("Numeric Control")]
        public double Number { get; set; }

        /// <summary>
        /// 坐标系选择框。
        /// </summary>
        /// <remarks>
        /// 用于选择 SolidWorks 坐标系特征。
        /// 中文：坐标系选择框
        /// </remarks>
        public IXCoordinateSystem CoordSystem { get; set; }

        /// <summary>
        /// 从零部件选择列表中移除最后一项，并通知属性管理器页面刷新控件显示。
        /// </summary>
        /// <remarks>
        /// 此方法作为 Button 位图按钮的点击操作。
        /// 它从 Components 列表中移除最后一个元素，然后触发 PropertyChanged 事件
        /// 以通知属性管理器页面刷新 UI 显示。
        /// 中文：从零部件选择列表中移除最后一项，并通知属性管理器页面刷新控件显示
        /// </remarks>
        private void ReduceComponents()
        {
            if (Components?.Any() == true)
            {
                Components.RemoveAt(Components.Count - 1);
                // Raise PropertyChanged to tell the PMP to refresh the Components selection box
                // 中文：触发 PropertyChanged 通知属性管理器页面刷新零部件选择框
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Components)));
            }
        }

        /// <summary>
        /// 构造函数：绑定按钮操作并为所有属性管理器页面控件设置初始默认值。
        /// </summary>
        /// <remarks>
        /// 初始化过程包括：
        /// 1. 将 ReduceComponents 方法分配给 Button 位图按钮
        /// 2. 用示例条目初始化 DynamicControls 字典
        /// 3. 为 Button1 设置 lambda 表达式（追加 GUID 到文本块）
        /// 4. 设置 WPF 自定义控件的默认值（Opt3 | Opt4）
        /// 5. 设置标志枚举复选框列表的默认值（Opt3 | Opt4）
        /// 6. 预选自定义项目复选框列表的第一项
        /// 7. 设置静态整数选项框的默认值（3）
        ///
        /// 中文：构造函数：绑定按钮操作并为所有属性管理器页面控件设置初始默认值
        /// </remarks>
        public PmpData()
        {
            // Assign the ReduceComponents method as the action for the bitmap button
            // 中文：将 ReduceComponents 方法分配为位图按钮（Button）的点击操作
            Button = ReduceComponents;
            // Initialize the dynamic controls dictionary with a sample "A" entry
            // 中文：用示例条目"A"初始化动态控件字典
            DynamicControls = new Dictionary<string, object>()
            {
                { "A", "Hello" }
            };

            // Button1 updates the TextBlock text with a new GUID suffix each time it is clicked
            // 中文：Button1 每次点击时在文本块内容后追加新的 GUID，演示动态更新文本块
            Button1 = () =>
            {
                m_TextBlockText = "Hello World - " + Guid.NewGuid().ToString();
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TextBlockText)));
            };
            // Set the default selection state for the WPF custom control (Opt3 + Opt4 flags active)
            // 中文：设置 WPF 自定义控件的默认选中状态（Opt3 + Opt4 标志位激活）
            CustomControl = OptsFlag.Opt3 | OptsFlag.Opt4;

            // Set default checked state for the flag-enum checkbox list (Opt3 + Opt4)
            // 中文：设置标志枚举复选框列表的默认勾选状态（Opt3 + Opt4）
            FlagEnumCheckBoxes = OptsFlag2.Opt3 | OptsFlag2.Opt4;

            // Pre-select the first item in the custom items checkbox list
            // 中文：在自定义项目复选框列表中预先选中第一项
            CheckBoxList3 = new List<MyItem>()
            {
                MyItem.All[0]
            };

            // Set the default value for the static integer option box to 3
            // 中文：将静态整数选项框的默认值设置为 3
            OptionBox6 = 3;
        }
    }

    /// <summary>
    /// 宏特征（自定义特征）的属性管理器页面数据模型。
    /// </summary>
    /// <remarks>
    /// 此 PMP 用于宏特征（Macro Feature）的参数编辑，演示了：
    /// - 尺寸参数（[ParameterDimension]）如何在模型上显示为尺寸标注
    /// - [ExcludeControl] 如何将参数从 PMP 控件隐藏，仅在模型上显示
    /// - [ParameterExclude] 如何排除特定参数，使其不存储为特征参数
    /// - 选择框如何作为宏特征的输入几何体
    ///
    /// 宏特征是一种可重用的自定义特征，可以带参数和选择输入，
    /// 在模型树上显示为特征节点。
    ///
    /// 中文：宏特征（自定义特征）的属性管理器页面数据模型
    /// </remarks>
    [ComVisible(true)]
    public class PmpMacroFeatData : SwPropertyManagerPageHandler
    {
        /// <summary>
        /// 文本输入参数，存储为宏特征参数。
        /// </summary>
        /// <remarks>
        /// 此参数会在 PMP 中显示为文本输入框，用户可以输入任意文本内容。
        /// 输入值会作为宏特征的定义数据保存。
        /// 中文：存储为宏特征参数的文本输入
        /// </remarks>
        public string Text { get; set; }

        /// <summary>
        /// 线性尺寸参数，显示为模型上的尺寸标注句柄。
        /// </summary>
        /// <remarks>
        /// [ParameterDimension(CustomFeatureDimensionType_e.Linear)] 指定此参数为线性尺寸。
        /// [ExcludeControl] 表示此参数不在 PMP 中显示为数值控件，
        /// 而是在 SolidWorks 模型上以拖拽的尺寸标注形式显示。
        /// 用户可以直接在模型上拖动尺寸来修改参数值。
        /// 中文：线性尺寸参数；在模型上以尺寸标注形式显示，不在 PMP 中显示为数值控件
        /// </remarks>
        [ParameterDimension(CustomFeatureDimensionType_e.Linear)]
        [ExcludeControl]
        public double Number { get; set; } = 0.1;

        /// <summary>
        /// 枚举选项参数，存储为宏特征参数。
        /// </summary>
        /// <remarks>
        /// 此属性绑定到 OptionBox 控件，以单选按钮组形式呈现。
        /// 用户选择的枚举值会作为宏特征参数保存。
        /// 中文：存储为宏特征参数的枚举选项
        /// </remarks>
        public Opts Options { get; set; }

        /// <summary>
        /// 圆形边线（Circular Edge）的选择框，作为宏特征的输入几何体。
        /// </summary>
        /// <remarks>
        /// 用户需要选择一个圆形边线作为宏特征的输入。
        /// 圆形边线常用于放样、扫掠等特征的几何参考。
        /// 中文：圆形边线的选择框；用作此宏特征的输入几何体
        /// </remarks>
        public ISwCircularEdge Selection { get; set; }

        /// <summary>
        /// 自定义项目下拉框，此值不作为宏特征参数保存。
        /// </summary>
        /// <remarks>
        /// [ParameterExclude] 特性表示此参数仅用于 UI 交互，
        /// 不存储为宏特征的定义数据。这对于临时选择或辅助输入很有用。
        /// [ComboBox(typeof(MyCustomItemsProvider))] 使用自定义项目提供者
        /// 动态生成下拉框选项。
        /// 中文：自定义项目下拉框；[ParameterExclude] 表示此值不作为宏特征参数保存
        /// </remarks>
        [ParameterExclude]
        [ComboBox(typeof(MyCustomItemsProvider))]
        public MyItem Option2 { get; set; }

        /// <summary>
        /// 角度尺寸参数（弧度制），显示为模型上的角度标注。
        /// </summary>
        /// <remarks>
        /// [ParameterDimension(CustomFeatureDimensionType_e.Angular)] 指定为角度尺寸。
        /// [ExcludeControl] 使其在模型上以角度标注形式显示。
        /// 默认值为 π/9（20 度）。
        /// 中文：角度尺寸参数（弧度制）；在模型上以角度标注形式显示
        /// </remarks>
        [ParameterDimension(CustomFeatureDimensionType_e.Angular)]
        [ExcludeControl]
        public double Angle { get; set; } = Math.PI / 9;

        /// <summary>
        /// 构造函数：将下拉框的默认选项设置为最后一个可用项目。
        /// </summary>
        /// <remarks>
        /// 初始化时选择 MyItem.All 数组中的最后一个元素（即自定义项 "C"）。
        /// 中文：构造函数：将下拉框的默认选项设置为最后一个可用项目
        /// </remarks>
        public PmpMacroFeatData()
        {
            Option2 = MyItem.All.Last();
        }
    }

    /// <summary>
    /// ComboBox 控件演示用的 PMP 数据模型。
    /// </summary>
    /// <remarks>
    /// 此 PMP 专门演示了 ComboBox 控件的多种配置方式：
    /// - 使用自定义项目提供者（MyCustomItemsProvider）
    /// - 使用静态整数选项（1, 2, 3）
    /// - 通过元数据绑定（ItemsSource = nameof(List1)）动态提供数据源
    /// - 依赖其他控件值动态生成选项（如 Option6 依赖 Option3Set）
    ///
    /// 此模型实现了 INotifyPropertyChanged 以支持数据绑定方案的 UI 更新。
    /// 中文：演示 ComboBox 各种用法的 PMP 数据模型
    /// </remarks>
    [ComVisible(true)]
    public class PmpComboBoxData : SwPropertyManagerPageHandler, INotifyPropertyChanged
    {
        private MyItem[] m_List1;
        private MyItem m_Option3Set;

        /// <summary>
        /// ComboBox，使用自定义项目提供者，默认值。
        /// </summary>
        /// <remarks>
        /// 未在构造函数中显式设置值，因此使用 MyItem 的默认值。
        /// 中文：使用自定义项目提供者的下拉框
        /// </remarks>
        [ComboBox(typeof(MyCustomItemsProvider))]
        public MyItem Option1Default { get; set; }

        /// <summary>
        /// ComboBox，使用自定义项目提供者，在构造函数中设置为最后一项。
        /// </summary>
        [ComboBox(typeof(MyCustomItemsProvider))]
        public MyItem Option1Set { get; set; }

        /// <summary>
        /// 枚举类型 ComboBox，默认值。
        /// </summary>
        public Opts Option2Default { get; set; }

        /// <summary>
        /// 枚举类型 ComboBox，在构造函数中设置为 Opts.Opt2。
        /// </summary>
        public Opts Option2Set { get; set; }

        /// <summary>
        /// 动态数据源数组，通过 [Metadata] 特性暴露给其他控件。
        /// </summary>
        /// <remarks>
        /// 此属性实现了完整的属性更改通知（INotifyPropertyChanged）模式。
        /// 当设置新值时，会触发 PropertyChanged 事件以通知 UI 刷新。
        /// 其他控件（如 Option3Default）可以通过 [ComboBox(ItemsSource = nameof(List1))] 引用此数据源。
        /// </remarks>
        [Metadata(nameof(List1))]
        public MyItem[] List1
        {
            get => m_List1;
            set
            {
                m_List1 = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(List1)));
            }
        }

        /// <summary>
        /// ComboBox，使用元数据绑定（List1）作为数据源，默认值。
        /// </summary>
        /// <remarks>
        /// ItemsSource 直接引用 List1 属性的名称（nameof(List1)），
        /// 框架会自动解析此名称并获取对应的数据。
        /// </remarks>
        [ComboBox(ItemsSource = nameof(List1))]
        public MyItem Option3Default { get; set; }

        /// <summary>
        /// ComboBox，使用元数据绑定（List1）作为数据源，并标记为可控制的标签。
        /// </summary>
        /// <remarks>
        /// [ControlTag(nameof(Option3Set))] 将此控件标记为 Option3Set，
        /// 可被其他控件作为依赖源引用。
        /// </remarks>
        [ComboBox(ItemsSource = nameof(List1))]
        [ControlTag(nameof(Option3Set))]
        public MyItem Option3Set
        {
            get => m_Option3Set;
            set
            {
                m_Option3Set = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Option3Set)));
            }
        }

        /// <summary>
        /// 静态整数 ComboBox（1, 2, 3），标记为 Option4Default。
        /// </summary>
        /// <remarks>
        /// [ControlTag] 用于标记控件，使其可被其他控件引用为依赖源。
        /// </remarks>
        [ComboBox(1, 2, 3)]
        [ControlTag(nameof(Option4Default))]
        public int Option4Default { get; set; }

        /// <summary>
        /// 静态整数 ComboBox（1, 2, 3），在构造函数中设置为 2。
        /// </summary>
        [ComboBox(1, 2, 3)]
        public int Option4Set { get; set; }

        /// <summary>
        /// 自定义项目 ComboBox，依赖 Option4Default 的当前值。
        /// </summary>
        /// <remarks>
        /// [ComboBox(typeof(MyCustomItemsProvider), nameof(Option4Default))] 表明此控件
        /// 依赖 Option4Default 控件，当后者的值变化时可能会触发刷新。
        /// </remarks>
        [ComboBox(typeof(MyCustomItemsProvider), nameof(Option4Default))]
        public MyItem Option5Default { get; set; }

        /// <summary>
        /// 自定义项目 ComboBox，依赖 Option4Default，在构造函数中设置为最后一项。
        /// </summary>
        [ComboBox(typeof(MyCustomItemsProvider), nameof(Option4Default))]
        public MyItem Option5Set { get; set; }

        /// <summary>
        /// 动态字符串 ComboBox，依赖 Option3Set 的当前值动态生成选项。
        /// </summary>
        /// <remarks>
        /// MyCustomItems1Provider 会根据 Option3Set 的 MyItem.Name 属性
        /// 动态生成选项列表（如 "1_A", "2_A", "3_A", "4_A"）。
        /// 这展示了 ComboBox 选项动态化的完整实现。
        /// </remarks>
        [ComboBox(typeof(MyCustomItems1Provider), nameof(Option3Set))]
        public string Option6 { get; set; }

        /// <summary>
        /// 按钮，点击时更新 List1 和 Option3Set 的值。
        /// </summary>
        /// <remarks>
        /// 此按钮演示了如何在运行时动态修改数据源并刷新 UI。
        /// 点击后 List1 被替换为新的数组，Option3Set 设置为新数组的最后一个元素。
        /// </remarks>
        public Action Button { get; }

        /// <summary>
        /// 属性变更事件。
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 构造函数：初始化数据源和默认值。
        /// </summary>
        /// <remarks>
        /// 初始化过程：
        /// 1. 将 Button 赋值为 lambda 表达式，更新 List1 和 Option3Set
        /// 2. 设置 List1 为 MyItem.All
        /// 3. 设置 Option1Set 为 MyItem.All 的最后一项
        /// 4. 设置 Option2Set 为 Opts.Opt2
        /// 5. 设置 m_Option3Set 为 MyItem.All 的最后一项
        /// 6. 设置 Option4Set 为 2
        /// 7. 设置 Option5Set 为 MyItem.All 的最后一项
        /// </remarks>
        public PmpComboBoxData()
        {
            Button = new Action(() =>
            {
                List1 = new MyItem[] { new MyItem() { Name = "_", Id = -1 }, new MyItem() { Name = "-", Id = -2 } };
                Option3Set = List1.Last();
            });

            List1 = MyItem.All;
            Option1Set = MyItem.All.Last();
            Option2Set = Opts.Opt2;
            m_Option3Set = MyItem.All.Last();
            Option4Set = 2;
            Option5Set = MyItem.All.Last();

            //Option1Set = new MyItem() { Name = "_", Id = -1 };
            //Option2Set = (Opts)5;
            //Option3Set = new MyItem() { Name = "-", Id = -2 };
            //Option4Set = 5;
            //Option5Set = new MyItem() { Name = "+", Id = -3 };
        }
    }

    /// <summary>
    /// 演示可勾选分组框（Checkable Group Box）的 PMP 数据模型。
    /// </summary>
    /// <remarks>
    /// 此 PMP 展示了如何使用 [CheckableGroupBox] 特性创建一个可勾选的分组框。
    /// 分组框的勾选状态可以控制其内部子控件的启用/禁用状态。
    ///
    /// 设计模式：
    /// - Group 类作为分组框内的所有控件的容器
    /// - [CheckableGroupBox(nameof(Group.IsChecked))] 将分组框的勾选状态绑定到 Group.IsChecked 属性
    /// - IMetadataDependencyHandler 实现对元数据变化的响应
    ///
    /// 中文：演示可勾选分组框的 PMP 数据模型
    /// </remarks>
    [ComVisible(true)]
    public class ToggleGroupPmpData : SwPropertyManagerPageHandler
    {
        /// <summary>
        /// 元数据依赖处理器：当分组框被勾选时禁用源控件。
        /// </summary>
        /// <remarks>
        /// 实现 IMetadataDependencyHandler 接口，用于处理元数据级别的依赖关系。
        /// 此处理器监听 Group.IsChecked 的值，当分组框被勾选时，禁用受控控件。
        /// 与 IDependencyHandler 不同的是，这里处理的是元数据（Metadata）而非控件值。
        /// 中文：元数据依赖处理器；当分组框被勾选时禁用源控件
        /// </remarks>
        public class IsCheckedDepHandler : IMetadataDependencyHandler
        {
            /// <summary>
            /// 状态更新回调：对 IsChecked 取反以决定是否启用受控控件。
            /// </summary>
            /// <param name="app">SolidWorks 应用程序实例</param>
            /// <param name="source">需要更新启用状态的源控件</param>
            /// <param name="metadata">元数据数组，第一个元素为 IsChecked 的值</param>
            /// <remarks>
            /// 当 IsChecked 为 true（分组框被勾选）时，禁用源控件；
            /// 当 IsChecked 为 false 时，启用源控件。
            /// 中文：UpdateState：对 IsChecked 取反以决定是否启用（勾选时禁用依赖控件）
            /// </remarks>
            public void UpdateState(IXApplication app, IControl source, IMetadata[] metadata)
            {
                source.Enabled = !(bool)metadata.First().Value;
            }
        }

        /// <summary>
        /// 分组框内的控件容器类。
        /// </summary>
        /// <remarks>
        /// 此类实现了 INotifyPropertyChanged，IsChecked 属性会触发变更通知。
        /// 包含文本框、数值控件和按钮，作为分组框内的子控件。
        /// 中文：表示可勾选分组框内所有控件的嵌套类
        /// </remarks>
        public class Group : INotifyPropertyChanged
        {
            /// <summary>
            /// 属性变更事件。
            /// </summary>
            public event PropertyChangedEventHandler PropertyChanged;

            private bool m_IsChecked;

            /// <summary>
            /// 分组框是否被勾选（展开并启用）。
            /// </summary>
            /// <remarks>
            /// 此属性通过 [Metadata(nameof(IsChecked))] 暴露为元数据，
            /// 供 IsCheckedDepHandler 等元数据依赖处理器使用。
            /// 当属性值变化时，触发 PropertyChanged 事件通知 UI 更新。
            /// 中文：分组框是否被勾选（展开并启用）；作为元数据暴露给依赖处理器
            /// </remarks>
            [Metadata(nameof(IsChecked))]
            public bool IsChecked
            {
                get => m_IsChecked;
                set
                {
                    m_IsChecked = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsChecked)));
                }
            }

            /// <summary>
            /// 分组框内的文本框控件。
            /// </summary>
            public string TextBox { get; set; }

            /// <summary>
            /// 分组框内的数值（整数）控件。
            /// </summary>
            public int Number { get; set; }

            /// <summary>
            /// 按钮，每次点击时切换分组框的勾选状态。
            /// </summary>
            /// <remarks>
            /// 在构造函数中被绑定为切换 IsChecked 值的 Action。
            /// 中文：每次点击时切换分组框勾选状态的按钮
            /// </remarks>
            public Action Button { get; }

            /// <summary>
            /// 构造函数：初始勾选状态为 true；将按钮绑定为切换 IsChecked 的操作。
            /// </summary>
            /// <remarks>
            /// Group 的默认状态为勾选（IsChecked = true），按钮点击时会取反此状态。
            /// 中文：构造函数：初始勾选状态为 true；将按钮绑定为切换 IsChecked 的操作
            /// </remarks>
            public Group()
            {
                m_IsChecked = true;
                Button = new Action(() => IsChecked = !IsChecked);
            }
        }

        /// <summary>
        /// 可勾选分组框控件。
        /// </summary>
        /// <remarks>
        /// [CheckableGroupBox(nameof(Group.IsChecked))] 将分组框的勾选状态绑定到
        /// Group.IsChecked 属性。当用户点击分组框的复选框时，IsChecked 值会更新，
        /// 触发所有依赖此元数据的控件的状态更新。
        /// 中文：可勾选分组框控件；[CheckableGroupBox] 特性将勾选状态绑定到 Group.IsChecked 属性
        /// </remarks>
        [CheckableGroupBox(nameof(Group.IsChecked))]
        //[GroupBoxOptions(GroupBoxOptions_e.Collapsed)]
        public Group Grp { get; set; }

        /// <summary>
        /// 分组框外的数值控件，当分组框被勾选时将被禁用。
        /// </summary>
        /// <remarks>
        /// [DependentOnMetadata(typeof(IsCheckedDepHandler), nameof(Group.IsChecked))] 将此控件
        /// 绑定到 IsCheckedDepHandler 元数据依赖处理器。当 Group.IsChecked 为 true 时，
        /// 此控件被禁用；为 false 时启用。
        /// 中文：分组框外的数值控件；当分组框被勾选时通过 IsCheckedDepHandler 将其禁用
        /// </remarks>
        [DependentOnMetadata(typeof(IsCheckedDepHandler), nameof(Group.IsChecked))]
        public double Number1 { get; set; }

        /// <summary>
        /// 构造函数：以默认值实例化 Group。
        /// </summary>
        /// <remarks>
        /// 创建 Group 实例，其默认 IsChecked 值为 true（分组框默认勾选）。
        /// 中文：构造函数：以默认值实例化 Group
        /// </remarks>
        public ToggleGroupPmpData()
        {
            Grp = new Group();
        }
    }
}
