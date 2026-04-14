// -*- coding: utf-8 -*-
// PropertyPage/Toolkit/Constructors/PropertyManagerPageCheckBoxListConstructor.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 复选框列表控件构造函数，用于创建和管理一组复选框控件。
//*********************************************************************

using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Linq;
using Xarial.XCad.SolidWorks.Enums;
using Xarial.XCad.SolidWorks.Services;
using Xarial.XCad.SolidWorks.UI.PropertyPage.Toolkit.Controls;
using Xarial.XCad.SolidWorks.Utils;
using Xarial.XCad.Toolkit.Services;
using Xarial.XCad.UI.PropertyPage.Attributes;
using Xarial.XCad.UI.PropertyPage.Base;
using Xarial.XCad.Utils.PageBuilder.Base;
using Xarial.XCad.Utils.PageBuilder.PageElements;
using Xarial.XCad.Utils.Reflection;

namespace Xarial.XCad.SolidWorks.UI.PropertyPage.Toolkit.Constructors
{
    internal class PropertyManagerPageCheckBoxListConstructor
        : PropertyManagerPageBaseControlConstructor<PropertyManagerPageCheckBoxListControl, PropertyManagerPageCheckBoxList>, ICheckBoxListConstructor
    {
        public PropertyManagerPageCheckBoxListConstructor(SwApplication app, IIconsCreator iconsConv) : base(app, iconsConv)
        {
        }

        protected override PropertyManagerPageCheckBoxListControl Create(IGroup parentGroup, IAttributeSet atts, IMetadata[] metadata, ref int numberOfUsedIds)
            => new PropertyManagerPageCheckBoxListControl(m_App, parentGroup, m_IconConv, atts, metadata, ref numberOfUsedIds);
    }
}