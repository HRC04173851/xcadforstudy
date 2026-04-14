// -*- coding: utf-8 -*-
// src/Base/Sketch/IXSketchText.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件定义草图文字（Sketch Text）的跨CAD平台接口。
// Sketch Text 是草图中的文本元素。
//
// Sketch Text 核心概念：
// 1. 文本内容：文字的字符串内容
// 2. 字体样式：字体、大小、粗体、斜体等
// 3. 位置与方向：文本在草图中的位置和旋转角度
// 4. 对齐方式：文本的对齐模式（左对齐、居中、右对齐等）
//
// 使用场景：
// - 在工程图中添加注释文字
// - 创建带有文字的草图符号
// - 添加标签和说明
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.Sketch
{
    /// <summary>
    /// Represents sketch text
    /// 表示草图文本
    /// </summary>
    public interface IXSketchText : IXSketchSegment
    {
    }
}
