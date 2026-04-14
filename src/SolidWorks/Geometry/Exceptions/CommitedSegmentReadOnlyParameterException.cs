// -*- coding: utf-8 -*-
// src/SolidWorks/Geometry/Exceptions/CommitedSegmentReadOnlyParameterException.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件定义曲线段参数在实体提交后被修改时抛出的异常。
// 当尝试修改已提交的曲线段（如起点、终点、半径等）时触发此异常。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Exceptions;

namespace Xarial.XCad.SolidWorks.Geometry.Exceptions
{
    public class CommitedSegmentReadOnlyParameterException : CommitedElementReadOnlyParameterException
    {
        public CommitedSegmentReadOnlyParameterException() : base("Parameter cannot be modified after entity is committed") 
        {
        }
    }
}
