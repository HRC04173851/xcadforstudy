//*********************************************************************
//xCAD
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
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
    // Simple enumeration used for OptionBox and ComboBox controls in the PropertyManager Page
    // 中文：用于属性管理器页面中选项框和下拉框控件的简单枚举
    public enum Opts
    {
        Opt1,
        Opt2,
        Opt3
    }

    // Second enumeration for demonstrating multiple independent OptionBox groups
    // 中文：用于演示多个独立选项框分组的第二个枚举
    public enum Opts1
    {
        Opt4,
        Opt5,
        Opt6
    }

    // Bit-flag enumeration; each member represents a power-of-two bit that can be combined with bitwise OR
    // 中文：位标志枚举；每个成员代表二的幂次位，可通过按位 OR 组合使用
    [Flags]
    public enum OptsFlag 
    {
        Opt1 = 1,
        Opt2 = 2,
        Opt3 = 4,
        Opt4 = 8
    }

    // Bit-flag enumeration with Title/Description attributes for rich display in CheckBoxList controls
    // 中文：带有 Title/Description 特性的位标志枚举，用于在复选框列表控件中显示丰富的名称和描述
    [Flags]
    public enum OptsFlag2
    {
        None = 0,

        [Title("Option #1")]
        [Description("First Option")]
        Opt1 = 1,
        Opt2 = 2,

        // Combined value representing both Opt1 and Opt2 selected simultaneously
        // 中文：表示同时选中 Opt1 和 Opt2 的组合值
        [Title("Opt1 + Opt2")]
        Opt1_2 = Opt1 | Opt2,

        Opt3 = 4,
        Opt4 = 8
    }

    // Data context (ViewModel) for the WPF custom control; implements INotifyPropertyChanged for two-way binding
    // 中文：WPF 自定义控件的数据上下文（视图模型）；实现 INotifyPropertyChanged 以支持双向数据绑定
    public class CustomControlDataContext : INotifyPropertyChanged
    {
        // Fires when Value changes, allowing the PMP to be notified that the custom control value has updated
        // 中文：当 Value 属性更改时触发，通知属性管理器页面自定义控件的值已更新
        public event Action<CustomControlDataContext, OptsFlag> ValueChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        private OptsFlag m_Value;

        // The current value of the custom control, bound two-way to the PMP via IXCustomControl.Value
        // 中文：自定义控件的当前值，通过 IXCustomControl.Value 与属性管理器页面进行双向绑定
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

        // Constructor: pre-populate sample items for display in the WPF control's item list
        // 中文：构造函数：预填充示例项目，供 WPF 控件的列表展示使用
        public CustomControlDataContext() 
        {
            Items = new ObservableCollection<Item>();
            Items.Add(new Item() { Name = "ABC", Value = "XYZ" });
            Items.Add(new Item() { Name = "ABC1", Value = "XYZ1" });
        }

        // Observable collection of items displayed in the WPF control's list
        // 中文：在 WPF 控件列表中显示的可观察项目集合
        public ObservableCollection<Item> Items { get; }
    }

    // Simple key-value pair model used by the WPF custom control's item list
    // 中文：WPF 自定义控件项目列表使用的简单键值对数据模型
    public class Item 
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    // Sample item model used by custom items providers for ComboBox, OptionBox, and CheckBoxList controls
    // 中文：自定义项目提供者使用的示例数据模型，用于下拉框、选项框和复选框列表控件
    public class MyItem 
    {
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

        public string Name { get; set; }
        public int Id { get; set; }

        public override string ToString() => Name;

        public override bool Equals(object obj)
        {
            if (obj is MyItem) 
            {
                return (obj as MyItem).Id == Id;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return 0;
        }
    }

    public class MyItem1 
    {
        public class MyItem1Name 
        {
            public string Name { get; }

            public MyItem1Name(string name)
            {
                Name = name;
            }
        }
        
        public string Value { get; }

        public MyItem1Name DisplayName { get; }

        public MyItem1(string val) 
        {
            Value = val;
            DisplayName = new MyItem1Name("[" + val + "]");
        }

        public override string ToString() => Value;
    }

    // Custom items provider: supplies MyItem[] to any control decorated with [ComboBox/OptionBox/CheckBoxList(typeof(MyCustomItemsProvider))]
    // 中文：自定义项目提供者：为使用 [ComboBox/OptionBox/CheckBoxList(typeof(MyCustomItemsProvider))] 装饰的控件提供 MyItem[] 数据
    public class MyCustomItemsProvider : SwCustomItemsProvider<MyItem>
    {
        public override IEnumerable<MyItem> ProvideItems(ISwApplication app, IControl[] dependencies)
            => MyItem.All;
    }

    // Custom items provider that returns string items dependent on the value of a linked control
    // 中文：自定义项目提供者，根据关联控件的当前值动态返回字符串列表
    public class MyCustomItems1Provider : SwCustomItemsProvider<string>
    {
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


    // Custom selection filter that only allows planar faces to be chosen in a selection box
    // 中文：自定义选择过滤器，仅允许在选择框中选择平面（planar face）
    public class PlanarFaceFilter : ISelectionCustomFilter
    {
        // Filter callback: invoked for each candidate selection; sets args.Filter=true to accept, false to reject
        // 中文：过滤回调：对每个候选选择项调用；设置 args.Filter=true 表示接受，false 表示拒绝
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

    // Dependency handler that shows or hides a control based on the value of a boolean dependency control
    // 中文：依赖处理器，根据布尔类型依赖控件的值来显示或隐藏目标控件
    public class VisibilityHandler : IDependencyHandler
    {
        // UpdateState: called whenever the dependency control's value changes
        // 中文：UpdateState：每当依赖控件的值更改时调用
        public void UpdateState(IXApplication app, IControl source, IControl[] dependencies)
        {
            // Toggle the visibility of the source control based on the first dependency's boolean value
            // 中文：根据第一个依赖控件的布尔值切换源控件的显示/隐藏状态
            source.Visible = (bool)dependencies.First().GetValue();
        }
    }

    // Dependency handler that enables or disables a control based on whether the Opt2 flag is set
    // 中文：依赖处理器，根据是否设置了 Opt2 标志位来启用或禁用目标控件
    public class CustomControlDependantHandler : IDependencyHandler
    {
        // UpdateState: enables the source control only when the custom control has the Opt2 flag set
        // 中文：UpdateState：仅当自定义控件设置了 Opt2 标志时才启用源控件
        public void UpdateState(IXApplication app, IControl source, IControl[] dependencies)
        {
            var val = (OptsFlag)dependencies.First().GetValue();

            // Enable the control only if the Opt2 bit is present in the dependency value
            // 中文：仅当依赖值中包含 Opt2 位时才启用该控件
            source.Enabled = val.HasFlag(OptsFlag.Opt2);
        }
    }

    // Main PropertyManager Page (PMP) data model for the add-in sample
    // 中文：插件示例的主属性管理器页面（PMP）数据模型
    // Inherits SwPropertyManagerPageHandler to handle SolidWorks PMP lifecycle events
    // 中文：继承 SwPropertyManagerPageHandler 以处理 SolidWorks 属性管理器页面的生命周期事件
    // Demonstrates virtually all supported control types: selection boxes, combos, option boxes,
    // check boxes, list boxes, text blocks, custom WPF/WinForms controls, dynamic controls, etc.
    // 中文：演示了几乎所有支持的控件类型：选择框、下拉框、选项框、复选框、列表框、
    // 中文：文本块、自定义 WPF/WinForms 控件、动态控件等
    [ComVisible(true)]
    [Help("https://xcad.net/")]
    //[PageOptions(PageOptions_e.OkayButton | PageOptions_e.CancelButton | PageOptions_e.HandleKeystrokes)]
    public class PmpData : SwPropertyManagerPageHandler, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        // Selection box for any SolidWorks object; currently filtered to annotation tables
        // 中文：任意 SolidWorks 对象的选择框；当前过滤为注释表格（AnnotationTables）类型
        //[SelectionBoxOptions(Filters = new Type[] { typeof(IXFace) })]
        [SwSelectionBoxOptions(Filters = new swSelectType_e[] { swSelectType_e.swSelANNOTATIONTABLES })]
        public ISwSelObject UnknownObject { get; set; }

        // Custom WPF control embedded directly inside the PMP; its value is an OptsFlag bit-field
        // 中文：直接嵌入属性管理器页面的 WPF 自定义控件；其值为 OptsFlag 位字段
        [CustomControl(typeof(WpfUserControl))]
        //[CustomControl(typeof(WinUserControl))]
        [ControlOptions(height: 300)]
        [ControlTag(nameof(CustomControl))]
        public OptsFlag CustomControl { get; set; }

        // Selection box whose enabled state depends on whether CustomControl has the Opt2 flag set
        // 中文：选择框，其启用状态取决于 CustomControl 是否包含 Opt2 标志
        [DependentOn(typeof(CustomControlDependantHandler), nameof(CustomControl))]
        [Description("Any object selection")]
        public ISwSelObject AnyObject { get; set; }

        // Selection box for planar faces only; uses both standard type filter (faces) and PlanarFaceFilter
        // 中文：仅用于选择平面的选择框；同时使用标准类型过滤（面）和自定义 PlanarFaceFilter
        [SwSelectionBoxOptions(CustomFilter = typeof(PlanarFaceFilter), Filters = new swSelectType_e[] { swSelectType_e.swSelFACES })] //setting the standard filter to faces and custom filter to only filter planar faces
        // 中文：将标准过滤器设置为面类型，并将自定义过滤器设置为仅允许平面
        [AttachMetadata(nameof(ComponentsMetadata))]
        [AttachMetadata(nameof(CircEdgeMetadata))]
        public ISwFace PlanarFace { get; set; }

        // Multi-selection list of assembly components
        // 中文：装配体零部件的多选列表
        public List<ISwComponent> Components { get; set; }

        // Selection box for solid bodies; automatically receives focus when the PMP opens
        // 中文：实体（Solid Body）的选择框；属性管理器页面打开时自动获取输入焦点
        [SelectionBoxOptions(Focused = true)]
        public ISwBody Body { get; set; }

        [Metadata(nameof(ComponentsMetadata))]
        public List<ISwComponent> ComponentsMetadata => Components;

        [Metadata(nameof(CircEdgeMetadata))]
        public ISwCircularEdge CircEdgeMetadata => CircEdge;

        public ISwCircularEdge CircEdge { get; set; }

        private string m_TextBlockText = "Hello World";

        [TextBlock]
        [TextBlockOptions(TextAlignment_e.Center, FontStyle_e.Bold | FontStyle_e.Italic)]
        [ControlOptions(backgroundColor: System.Drawing.KnownColor.Yellow, textColor: System.Drawing.KnownColor.Green)]
        public string TextBlockText => m_TextBlockText;

        [BitmapToggleButton(typeof(Resources), nameof(Resources.vertical), nameof(Resources.horizontal), 96, 96)]
        [Description("Dynamic icon1")]
        public bool CheckBox1 { get; set; } = true;

        [BitmapToggleButton(typeof(Resources), nameof(Resources.vertical), BitmapEffect_e.Grayscale | BitmapEffect_e.Transparent, 24, 24)]
        [Description("Dynamic icon2")]
        public bool CheckBox2 { get; set; } = false;

        [BitmapButton(typeof(Resources), nameof(Resources.horizontal), 48, 48)]
        public bool CheckBox { get; set; }

        [BitmapButton(typeof(Resources), nameof(Resources.xarial))]
        public Action Button { get; }

        [Title("Action Button")]
        [Description("Sample button")]
        public Action Button1 { get; }

        [DynamicControls("_Test_")]
        public Dictionary<string, object> DynamicControls { get; }

        //public List<string> List { get; set; }

        [ComboBox(1, 2, 3, 4, 5)]
        [Label("Static Combo Box:", ControlLeftAlign_e.Indent)]
        public int StaticComboBox { get; set; }

        [Metadata("_SRC_")]
        public MyItem1[] Source { get; } = new MyItem1[] { new MyItem1("X"), new MyItem1("Y"), new MyItem1("Z") };

        [ComboBox(ItemsSource = "_SRC_")]
        public MyItem1 ItemsSourceComboBox { get; set; }

        [ListBox(ItemsSource = "_SRC_", DisplayMemberPath = "DisplayName.Name")]
        [Label("List Box1:", ControlLeftAlign_e.LeftEdge, FontStyle_e.Bold)]
        [ControlOptions(align: ControlLeftAlign_e.Indent)]
        public MyItem1 ListBox1 { get; set; }

        [ListBox("A1", "A2", "A3")]
        public string ListBox2 { get; set; }

        [ListBox(1, 2, 3, 4)]
        public List<int> ListBox3 { get; set; }

        //[ListBox]
        [OptionBox]
        [Label("Sample Option Box 4:", fontStyle: FontStyle_e.Underline)]
        public Opts OptionBox4 { get; set; }

        [OptionBox]
        [Label("Sample Option Box 5:")]
        public Opts1 OptionBox5 { get; set; }

        [OptionBox(1, 2, 3, 4)]
        public int OptionBox6 { get; set; }

        [OptionBox(typeof(MyCustomItemsProvider))]
        public MyItem OptionBox7 { get; set; }

        [ListBox]
        public OptsFlag ListBox5 { get; set; } = OptsFlag.Opt1 | OptsFlag.Opt3;

        [CheckBoxList]
        [CheckBoxListOptions]
        [ControlOptions(align: ControlLeftAlign_e.Indent)]
        public OptsFlag2 FlagEnumCheckBoxes { get; set; }

        [CheckBoxList(1, 2, 3, 4)]
        public List<int> CheckBoxList2 { get; set; }

        [CheckBoxList(typeof(MyCustomItemsProvider))]
        public List<MyItem> CheckBoxList3 { get; set; }

        [ControlTag(nameof(Visible))]
        public bool Visible { get; set; }

        [DependentOn(typeof(VisibilityHandler), nameof(Visible))]
        [Label("Numeric Control")]
        public double Number { get; set; }

        public IXCoordinateSystem CoordSystem { get; set; }

        // Removes the last component from the Components selection list and notifies the PMP to refresh
        // 中文：从零部件选择列表中移除最后一项，并通知属性管理器页面刷新控件显示
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

        // Constructor: wires up button actions and sets default values for all PMP controls
        // 中文：构造函数：绑定按钮操作并为所有属性管理器页面控件设置初始默认值
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

    // PMP data model for a macro feature (custom feature); demonstrates dimension parameters and selection inputs
    // 中文：宏特征（自定义特征）的属性管理器页面数据模型；演示尺寸参数和选择框输入的用法
    [ComVisible(true)]
    public class PmpMacroFeatData : SwPropertyManagerPageHandler
    {
        // Text input parameter stored as a macro feature parameter
        // 中文：存储为宏特征参数的文本输入
        public string Text { get; set; }

        // Linear dimension parameter; [ExcludeControl] means it shows as a dimension handle on the model,
        // not as a numeric control in the PMP panel
        // 中文：线性尺寸参数；[ExcludeControl] 表示它在模型上以尺寸标注形式显示，
        // 中文：而不在属性管理器页面中显示为数值控件
        [ParameterDimension(CustomFeatureDimensionType_e.Linear)]
        [ExcludeControl]
        public double Number { get; set; } = 0.1;

        // Enum option parameter stored as a macro feature parameter
        // 中文：存储为宏特征参数的枚举选项
        public Opts Options { get; set; }

        // Selection box for a circular edge; used as input geometry for this macro feature
        // 中文：圆形边线的选择框；用作此宏特征的输入几何体
        public ISwCircularEdge Selection { get; set; }
        
        // Custom items combo box; [ParameterExclude] means this value is NOT stored as a macro feature parameter
        // 中文：自定义项目下拉框；[ParameterExclude] 表示此值不作为宏特征参数保存
        [ParameterExclude]
        [ComboBox(typeof(MyCustomItemsProvider))]
        public MyItem Option2 { get; set; }

        // Angular dimension parameter (in radians); displayed as an angle handle on the model
        // 中文：角度尺寸参数（弧度制）；在模型上以角度标注形式显示
        [ParameterDimension(CustomFeatureDimensionType_e.Angular)]
        [ExcludeControl]
        public double Angle { get; set; } = Math.PI / 9;

        // Constructor: sets the default combo box option to the last available item
        // 中文：构造函数：将下拉框的默认选项设置为最后一个可用项目
        public PmpMacroFeatData() 
        {
            Option2 = MyItem.All.Last();
        }
    }

    [ComVisible(true)]
    public class PmpComboBoxData : SwPropertyManagerPageHandler, INotifyPropertyChanged
    {
        private MyItem[] m_List1;
        private MyItem m_Option3Set;

        [ComboBox(typeof(MyCustomItemsProvider))]
        public MyItem Option1Default { get; set; }

        [ComboBox(typeof(MyCustomItemsProvider))]
        public MyItem Option1Set { get; set; }

        public Opts Option2Default { get; set; }

        public Opts Option2Set { get; set; }

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

        [ComboBox(ItemsSource = nameof(List1))]
        public MyItem Option3Default { get; set; }

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

        [ComboBox(1, 2, 3)]
        [ControlTag(nameof(Option4Default))]
        public int Option4Default { get; set; }

        [ComboBox(1, 2, 3)]
        public int Option4Set { get; set; }

        [ComboBox(typeof(MyCustomItemsProvider), nameof(Option4Default))]
        public MyItem Option5Default { get; set; }

        [ComboBox(typeof(MyCustomItemsProvider), nameof(Option4Default))]
        public MyItem Option5Set { get; set; }

        [ComboBox(typeof(MyCustomItems1Provider), nameof(Option3Set))]
        public string Option6 { get; set; }

        public Action Button { get; }

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

        public event PropertyChangedEventHandler PropertyChanged;
    }

    // PMP data model demonstrating a checkable group box that enables/disables child controls
    // 中文：演示可勾选分组框的属性管理器页面数据模型；分组框勾选状态可控制子控件的启用/禁用
    [ComVisible(true)]
    public class ToggleGroupPmpData : SwPropertyManagerPageHandler
    {
        // Metadata dependency handler: disables the source control when the group box is checked
        // 中文：元数据依赖处理器：当分组框被勾选时禁用源控件
        public class IsCheckedDepHandler : IMetadataDependencyHandler
        {
            // UpdateState: inverts IsChecked to determine enabled state (checked = disable the dependent control)
            // 中文：UpdateState：对 IsChecked 取反以决定是否启用（勾选时禁用依赖控件）
            public void UpdateState(IXApplication app, IControl source, IMetadata[] metadata)
            {
                source.Enabled = !(bool)metadata.First().Value;
            }
        }

        // Nested class representing all controls grouped inside the checkable group box
        // 中文：表示可勾选分组框内所有控件的嵌套类
        public class Group : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            private bool m_IsChecked;

            // Whether the group box is checked (expanded and enabled); exposed as metadata for dependency handlers
            // 中文：分组框是否被勾选（展开并启用）；作为元数据暴露给依赖处理器
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

            // Text box control inside the group box
            // 中文：分组框内的文本框控件
            public string TextBox { get; set; }
            // Numeric (integer) control inside the group box
            // 中文：分组框内的数值（整数）控件
            public int Number { get; set; }
            // Button that toggles the group box's checked state on each click
            // 中文：每次点击时切换分组框勾选状态的按钮
            public Action Button { get; }

            // Constructor: starts with group checked=true; wires button to toggle IsChecked
            // 中文：构造函数：初始勾选状态为 true；将按钮绑定为切换 IsChecked 的操作
            public Group() 
            {
                m_IsChecked = true;
                Button = new Action(() => IsChecked = !IsChecked);
            }
        }

        // Checkable group box control; the [CheckableGroupBox] attribute binds the check state to Group.IsChecked
        // 中文：可勾选分组框控件；[CheckableGroupBox] 特性将勾选状态绑定到 Group.IsChecked 属性
        [CheckableGroupBox(nameof(Group.IsChecked))]
        //[GroupBoxOptions(GroupBoxOptions_e.Collapsed)]
        public Group Grp { get; set; }

        // Numeric control outside the group box; disabled when the group box is checked (via IsCheckedDepHandler)
        // 中文：分组框外的数值控件；当分组框被勾选时通过 IsCheckedDepHandler 将其禁用
        [DependentOnMetadata(typeof(IsCheckedDepHandler), nameof(Group.IsChecked))]
        public double Number1 { get; set; }

        // Constructor: instantiate the Group with its default values
        // 中文：构造函数：以默认值实例化 Group
        public ToggleGroupPmpData() 
        {
            Grp = new Group();
        }
    }
}
