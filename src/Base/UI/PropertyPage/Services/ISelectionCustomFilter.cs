// -*- coding: utf-8 -*-
// src/Base/UI/PropertyPage/Services/ISelectionCustomFilter.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义选择过滤接口，允许在选择实体时执行自定义过滤逻辑
//*********************************************************************

using Xarial.XCad.Base.Enums;
using Xarial.XCad.UI.PropertyPage.Base;
using Xarial.XCad.UI.PropertyPage.Structures;

namespace Xarial.XCad.UI.PropertyPage.Services
{
    /// <summary>
    /// Enables custom logic for filtering the selection
    /// 启用选择过滤自定义逻辑
    /// </summary>
    /// <remarks>Assigned via <see cref="Attributes.SelectionBoxOptionsAttribute.CustomFilter"/></remarks>
    public interface ISelectionCustomFilter
    {
        /// <summary>
        /// Called when entity is about to be selected
        /// 实体即将被选择时调用
        /// </summary>
        /// <param name="selBox">Sender selection box</param>
        /// <param name="selection">Selection object</param>
        /// <param name="args">Filtering arguments</param>
        /// <returns></returns>
        void Filter(IControl selBox, IXSelObject selection, SelectionCustomFilterArguments args);
    }
}