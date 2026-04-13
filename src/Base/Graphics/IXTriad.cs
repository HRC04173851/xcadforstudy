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
using Xarial.XCad.Geometry.Structures;

namespace Xarial.XCad.Graphics
{
    /// <summary>
    /// Elements of triad manipulator
    /// 三轴坐标系操作器的元素
    /// </summary>
    [Flags]
    public enum TriadElements_e 
    {
        /// <summary>
        /// Origin
        /// 原点
        /// </summary>
        Origin = 1,

        /// <summary>
        /// X-Axis
        /// X 轴
        /// </summary>
        AxisX = 2,

        /// <summary>
        /// Y-Axis
        /// Y 轴
        /// </summary>
        AxisY = 4,

        /// <summary>
        /// Z-Axis
        /// Z 轴
        /// </summary>
        AxisZ = 8,

        /// <summary>
        /// Shows all elements of triad
        /// 显示三轴座标系操作器的所有元素
        /// </summary>
        All = Origin | AxisX | AxisY | AxisZ,
    }

    /// <summary>
    /// Delegate for <see cref="IXTriad.Selected"/> event
    /// <see cref="IXTriad.Selected"/> 事件的委托
    /// </summary>
    /// <param name="sender">Triad</param>
    /// <param name="element">Element being selected</param>
    public delegate void TriadSelectedDelegate(IXTriad sender, TriadElements_e element);

    /// <summary>
    /// Represents the triad manipulator
    /// 三轴坐标系操作器
    /// </summary>
    public interface IXTriad : IXObject, IXTransaction, IDisposable
    {
        /// <summary>
        /// Raised when the element of the triad is selected
        /// 三轴坐标系元素被选中时触发
        /// </summary>
        event TriadSelectedDelegate Selected;

        /// <summary>
        /// Elements of this triad
        /// 三轴坐标系包含的元素
        /// </summary>
        TriadElements_e Elements { get; set; }

        /// <summary>
        /// Transformation of this triad
        /// 三轴坐标系的变换矩阵
        /// </summary>
        TransformMatrix Transform { get; set; }

        /// <summary>
        /// Controls the visibility of this triad
        /// 控制三轴坐标系的可见性
        /// </summary>
        bool Visible { get; set; }
    }
}
