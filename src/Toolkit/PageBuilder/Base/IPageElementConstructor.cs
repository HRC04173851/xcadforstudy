// -*- coding: utf-8 -*-
// src/Toolkit/PageBuilder/Base/IPageElementConstructor.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件定义页面元素构造器接口 IPageElementConstructor。
// 负责在分组容器中创建页面元素（控件）。
// 创建控件实例并更新已使用的标识符范围。
//*********************************************************************

using Xarial.XCad.UI.PropertyPage.Base;
using Xarial.XCad.Utils.PageBuilder.PageElements;

namespace Xarial.XCad.Utils.PageBuilder.Base
{
    /// <summary>
    /// Creates page elements (controls) inside group containers.
    /// <para>在分组容器中创建页面元素（控件）。</para>
    /// </summary>
    public interface IPageElementConstructor
    {
        /// <summary>
        /// Creates control instance and updates consumed id range.
        /// <para>创建控件实例并更新已使用的标识符范围。</para>
        /// </summary>
        IControl Create(IGroup parentGroup, IAttributeSet atts, IMetadata[] metadata, ref int idRange);
    }
}