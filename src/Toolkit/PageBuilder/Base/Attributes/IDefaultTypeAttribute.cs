// -*- coding: utf-8 -*-
// src/Toolkit/PageBuilder/Base/Attributes/IDefaultTypeAttribute.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件定义默认类型特性接口 IDefaultTypeAttribute。
// 指示对应构造器应作为指定数据类型的默认控件构造器。
// 必须应用于 IPageElementConstructor 或 IPageConstructor。
//*********************************************************************

using System;
using Xarial.XCad.UI.PropertyPage.Base;

namespace Xarial.XCad.Utils.PageBuilder.Base.Attributes
{
    ///<summary>
    ///This attribute indicates that the constructor must be used as a default control constructor for the nominated type
    /// <para>该特性指示对应构造器应作为指定类型的默认控件构造器。</para>
    ///</summary>
    /// <remarks>
    /// Must be applied to <see cref="IPageElementConstructor{TGroup, TPage}"/>
    /// or <see cref="IPageConstructor{TPage}"/> only
    /// </summary>
    public interface IDefaultTypeAttribute : IAttribute
    {
        /// <summary>
        /// Specifies the type of the data which this constructor creates control for
        /// <para>指定该构造器要创建控件所对应的数据类型。</para>
        /// </summary>
        Type Type { get; }
    }
}