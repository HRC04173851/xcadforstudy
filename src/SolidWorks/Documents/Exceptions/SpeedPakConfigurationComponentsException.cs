// -*- coding: utf-8 -*-
// src/SolidWorks/Documents/Exceptions/SpeedPakConfigurationComponentsException.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本异常在尝试从 SpeedPak 配置中提取组件时抛出。
// SpeedPak 是装配体的一种简化表示，用于提高大型装配体的性能，
// 其内部不保留原始组件的完整几何信息，无法进行组件提取操作。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xarial.XCad.Exceptions;

namespace Xarial.XCad.SolidWorks.Documents.Exceptions
{
    public class SpeedPakConfigurationComponentsException : Exception, IUserException
    {
        public SpeedPakConfigurationComponentsException() : base("Components cannot be extracted from the SpeedPak configuration")
        {
        }
    }
}
