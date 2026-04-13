//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Base;
using Xarial.XCad.Enums;
using Xarial.XCad.Geometry.Structures;

namespace Xarial.XCad.Graphics
{
    /// <summary>
    /// Represents the visual key-value element which can be attached to visual elements
    /// 可附加到可视化元素的键值标注接口
    /// </summary>
    public interface IXCalloutBase : IXObject, IXTransaction, IDisposable
    {
        /// <summary>
        /// Rows of this callout
        /// 标注框的行集合
        /// </summary>
        IXCalloutRow[] Rows { get; }

        /// <summary>
        /// Background color of the callout
        /// 标注框的背景颜色
        /// </summary>
        StandardSelectionColor_e? Background { get; set; }

        /// <summary>
        /// Foreground color of the callout
        /// 标注框的前景颜色
        /// </summary>
        StandardSelectionColor_e? Foreground { get; set; }

        /// <summary>
        /// Controls the visibility of this callout
        /// 控制标注框的可见性
        /// </summary>
        bool Visible { get; set; }

        /// <summary>
        /// Pre creates new callout row
        /// 预创建新的标注框行
        /// </summary>
        /// <returns>New row template</returns>
        IXCalloutRow AddRow();
    }

    /// <summary>
    /// Represents the selection independent callout
    /// 独立于选择的标注框
    /// </summary>
    public interface IXCallout : IXCalloutBase 
    {
        /// <summary>
        /// Location of the callout box
        /// 标注框的位置
        /// </summary>
        Point Location { get; set; }

        /// <summary>
        /// Anchor point (attachment) of this callout
        /// 标注框的锁定点（附着点）
        /// </summary>
        Point Anchor { get; set; }
    }

    /// <summary>
    /// Represents the callout associated with the selection
    /// 与选择关联的标注框
    /// </summary>
    public interface IXSelCallout : IXCalloutBase 
    {
        /// <summary>
        /// Attached selection object
        /// 附着的选择对象
        /// </summary>
        IXSelObject Owner { get; set; }
    }

    /// <summary>
    /// Delegate of <see cref="IXCalloutRow.ValueChanged"/> event
    /// <see cref="IXCalloutRow.ValueChanged"/> 事件的委托
    /// </summary>
    /// <param name="callout">Callout where value is changed</param>
    /// <param name="row">Changed row</param>
    /// <param name="newValue">New value</param>
    /// <returns>True to accept value, False to cancel the value change</returns>
    public delegate bool CalloutRowValueChangedDelegate(IXCalloutBase callout, IXCalloutRow row, string newValue);

    /// <summary>
    /// Represents the callout row
    /// 标注框的行
    /// </summary>
    public interface IXCalloutRow
    {
        /// <summary>
        /// Fired when the value of the callout is changed
        /// 标注框值改变时触发
        /// </summary>
        event CalloutRowValueChangedDelegate ValueChanged;

        /// <summary>
        /// True if value of this callout cannot be changed
        /// 该行的值是否为只读
        /// </summary>
        bool IsReadOnly { get; set; }

        /// <summary>
        /// Name of the key in this row
        /// 该行的键名称
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Value of this key
        /// 该键对应的值
        /// </summary>
        string Value { get; set; }
    }
}
