// -*- coding: utf-8 -*-
// src/Base/Defaults.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 提供xCAD框架的默认对象集合，包括默认图标等基础资源的定义和使用
//*********************************************************************

using System.Drawing;
using Xarial.XCad.Properties;
using Xarial.XCad.Reflection;
using Xarial.XCad.UI;

namespace Xarial.XCad
{
    /// <summary>
    /// Collection of default objects
    /// 默认对象集合
    /// </summary>
    public static class Defaults
    {
        /// <summary>
        /// Default icon
        /// 默认图标
        /// </summary>
        public static IXImage Icon
            => new BaseImage(Resources.default_icon);
    }
}