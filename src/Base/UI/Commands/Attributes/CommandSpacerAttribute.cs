// -*- coding: utf-8 -*-
// src/Base/UI/Commands/Attributes/CommandSpacerAttribute.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 标记命令在菜单或工具栏中前置分隔符，用于在命令组中创建视觉分隔效果
//*********************************************************************

using System;

namespace Xarial.XCad.UI.Commands.Attributes
{
    /// <summary>
    /// Marks the command to be separated by the spacer (separator) in the menu and the toolbar
    /// 标记该命令在菜单/工具栏中前置分隔符
    /// </summary>
    /// <remarks>Spacer is added before the command marked with this attribute</remarks>
    [AttributeUsage(AttributeTargets.Field)]
    public class CommandSpacerAttribute : Attribute
    {
    }
}