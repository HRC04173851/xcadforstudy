// -*- coding: utf-8 -*-
// Commands/Exceptions/InvalidMenuToolbarOptionsException.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 无效的菜单工具栏选项异常，当命令既没有工具栏也没有菜单选项时抛出此异常。
//*********************************************************************

using System;
using Xarial.XCad.UI.Commands.Structures;

namespace Xarial.XCad.SolidWorks.UI.Commands.Exceptions
{
    /// <summary>
    /// Indicates that the command doesn't have either menu or toolbar option set
    /// </summary>
    public class InvalidMenuToolbarOptionsException : InvalidOperationException
    {
        internal InvalidMenuToolbarOptionsException(CommandSpec cmd)
            : base($"Neither toolbar nor menu option is specified for {cmd.Title} ({cmd.UserId}) command. Use")
        {
        }
    }
}