// -*- coding: utf-8 -*-
// src/SolidWorks/Documents/Exception.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件定义 SolidWorks 文档操作相关的通用异常类型。
// DocumentAlreadyOpenedException 用于标识文档已被重复打开的情况，
// 避免同一文档在系统中存在多个实例导致的数据不一致问题。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.SolidWorks.Documents
{
    public class DocumentAlreadyOpenedException : Exception
    {
        public DocumentAlreadyOpenedException(string path) : base($"{path} document already opened") 
        {
        }
    }
}
