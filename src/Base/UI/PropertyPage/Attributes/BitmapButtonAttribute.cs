// -*- coding: utf-8 -*-
// src/Base/UI/PropertyPage/Attributes/BitmapButtonAttribute.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 指示当前属性应渲染为位图按钮，支持自定义图标或标准图标。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Reflection;
using Xarial.XCad.UI.PropertyPage.Base;
using Xarial.XCad.UI.PropertyPage.Enums;

namespace Xarial.XCad.UI.PropertyPage.Attributes
{
    /// <summary>
    /// Marker interface for bitmap button control constructor
    /// 位图按钮控件构造器的标记接口
    /// </summary>
    public interface IBitmapButtonConstructor
    {
    }

    /// <summary>
    /// Attribute indicates that current property should be rendered as bitmap button
    /// 指示当前属性应渲染为位图按钮
    /// </summary>
    /// <remarks>This attribute is only applicable for <see cref="bool"/> and <see cref="Action"/> types.
    /// Checkable button will be rendered for the first case
    public class BitmapButtonAttribute : Attribute, ISpecificConstructorAttribute
    {
        /// <summary>
        /// Type of the constructor
        /// 构造器类型
        /// </summary>
        public Type ConstructorType { get; }

        /// <summary>
        /// Image assigned to this icon
        /// 分配给此按钮的图标图像
        /// </summary>
        public IXImage Icon { get; }

        /// <summary>
        /// Width of the button
        /// 按钮宽度（像素）
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// Height of the button
        /// 按钮高度（像素）
        /// </summary>
        public int Height { get; }

        /// <summary>
        /// Standard icon or null if use custom icon
        /// 标准图标，若使用自定义图标则为 null
        /// </summary>
        public BitmapButtonLabelType_e? StandardIcon { get; }

        /// <param name="resType">Type of the static class (usually Resources)</param>
        /// <param name="masterResName">Resource name of the master icon</param>        
        /// <param name="width">Button width</param>
        /// <param name="height">Button height</param>
        public BitmapButtonAttribute(Type resType, string masterResName, int width = 24, int height = 24) : this()
        {
            Icon = ResourceHelper.GetResource<IXImage>(resType, masterResName);

            Width = width;
            Height = height;
        }

        public BitmapButtonAttribute(BitmapButtonLabelType_e standardIcon) : this()
        {
            StandardIcon = standardIcon;
        }

        private BitmapButtonAttribute()
        {
            ConstructorType = typeof(IBitmapButtonConstructor);
        }
    }
}
