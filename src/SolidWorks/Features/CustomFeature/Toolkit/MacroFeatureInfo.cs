// -*- coding: utf-8 -*-
// src/SolidWorks/Features/CustomFeature/Toolkit/MacroFeatureInfo.cs

//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Xarial.XCad.Features.CustomFeature.Attributes;
using Xarial.XCad.Reflection;
using Xarial.XCad.Utils.Reflection;

namespace Xarial.XCad.SolidWorks.Features.CustomFeature.Toolkit
{
    /// <summary>
    /// 宏特性信息工具类
    /// <para>提供从宏特性类型提取名称和 ProgID 的方法</para>
    /// </summary>
    /// <remarks>
    /// 宏特性的名称和 ProgID 可以通过特性（Attribute）指定，
    /// 也可以使用类型的默认名称。
    /// </remarks>
    internal static class MacroFeatureInfo
    {
        /// <summary>
        /// 获取宏特性的基名称
        /// </summary>
        /// <typeparam name="TMacroFeature">宏特性定义类型</typeparam>
        /// <returns>特性在特征树中显示的名称</returns>
        internal static string GetBaseName<TMacroFeature>()
            where TMacroFeature : SwMacroFeatureDefinition
        {
            return GetBaseName(typeof(TMacroFeature));
        }

        /// <summary>
        /// 从类型获取宏特性基名称
        /// </summary>
        /// <param name="macroFeatType">宏特性定义类型</param>
        /// <returns>特性在特征树中显示的名称</returns>
        /// <remarks>
        /// 优先使用 <see cref="DisplayNameAttribute"/> 指定的名称，
        /// 如果没有则使用类型的名称。
        /// </remarks>
        internal static string GetBaseName(Type macroFeatType)
        {
            if (!typeof(SwMacroFeatureDefinition).IsAssignableFrom(macroFeatType))
            {
                throw new InvalidCastException(
                    $"{macroFeatType.FullName} must inherit {typeof(SwMacroFeatureDefinition).FullName}");
            }

            string baseName = "";

            // 从 DisplayNameAttribute 获取显示名称
            macroFeatType.TryGetAttribute<DisplayNameAttribute>(a =>
            {
                baseName = a.DisplayName;
            });

            // 如果没有显示名称，使用类型名称
            if (string.IsNullOrEmpty(baseName))
            {
                baseName = macroFeatType.Name;
            }

            return baseName;
        }

        /// <summary>
        /// 获取宏特性的 ProgID
        /// </summary>
        /// <typeparam name="TMacroFeature">宏特性定义类型</typeparam>
        /// <returns>用于 COM 注册的 ProgID</returns>
        internal static string GetProgId<TMacroFeature>()
            where TMacroFeature : SwMacroFeatureDefinition
        {
            return GetProgId(typeof(TMacroFeature));
        }

        /// <summary>
        /// 从类型获取宏特性 ProgID
        /// </summary>
        /// <param name="macroFeatType">宏特性定义类型</param>
        /// <returns>用于 COM 注册的 ProgID</returns>
        /// <remarks>
        /// ProgID 用于在 Windows 注册表中查找 COM 服务器。
        /// 优先使用 <see cref="ProgIdAttribute"/> 指定的值，
        /// 如果没有则使用类型的完整名称。
        /// </remarks>
        internal static string GetProgId(Type macroFeatType)
        {
            if (!typeof(SwMacroFeatureDefinition).IsAssignableFrom(macroFeatType))
            {
                throw new InvalidCastException(
                    $"{macroFeatType.FullName} must inherit {typeof(SwMacroFeatureDefinition).FullName}");
            }

            string progId = "";

            // 从 ProgIdAttribute 获取 ProgID
            if (!macroFeatType.TryGetAttribute<ProgIdAttribute>(a => progId = a.Value))
            {
                // 如果没有指定，使用类型的完整名称作为后备
                progId = macroFeatType.FullName;
            }

            return progId;
        }
    }
}