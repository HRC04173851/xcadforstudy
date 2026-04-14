// -*- coding: utf-8 -*-
// Enums/AccessType_e.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义第三方数据存储的访问类型枚举，包括读取和写入两种访问模式。
// 用于控制对 IStorage 接口实现类的访问权限管理。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.Data.Enums
{
    /// <summary>
    /// Access type of the 3rd party data storage
    /// 第三方数据存储访问类型
    /// </summary>
    public enum AccessType_e
    {
        /// <summary>
        /// Reading access
        /// 读取访问
        /// </summary>
        Read,

        /// <summary>
        /// Writing access
        /// 写入访问
        /// </summary>
        Write
    }
}
