// -*- coding: utf-8 -*-
// src/Base/Annotations/IXAnnotationRepository.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件定义标注集合仓储（Annotation Repository）的接口。
// 标注仓储提供对文档或视图中所有标注的统一访问。
//
// Repository 功能：
// - 遍历所有标注
// - 按名称或类型查找标注
// - 添加/删除标注
//
// 仓储类型：
// - IXDimensionRepository：尺寸仓储
// - IXAnnotationRepository：通用标注仓储
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xarial.XCad.Base;

namespace Xarial.XCad.Annotations
{
    /// <summary>
    /// Collection of annotations
    /// 标注集合
    /// </summary>
    public interface IXAnnotationRepository : IXRepository<IXAnnotation>
    {
    }
}
