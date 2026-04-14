// -*- coding: utf-8 -*-
// src/Toolkit/Graphics/OglGraphicsContext.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件实现基于 OpenGL 的自定义图形上下文占位类 OglGraphicsContext。
// 作为 IXCustomGraphicsContext 接口的占位实现，目前仅提供基础 Dispose 功能。
// 用于支持 CAD 软件中的自定义图形渲染上下文管理。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Documents;

namespace Xarial.XCad.Toolkit.Graphics
{
    /// <summary>
    /// OpenGL-based custom graphics context placeholder implementation.
    /// <para>基于 OpenGL 的自定义图形上下文占位实现。</para>
    /// </summary>
    public class OglGraphicsContext : IXCustomGraphicsContext
    {
        /// <summary>
        /// Disposes graphics context resources.
        /// <para>释放图形上下文资源。</para>
        /// </summary>
        public void Dispose()
        {
        }
    }
}
