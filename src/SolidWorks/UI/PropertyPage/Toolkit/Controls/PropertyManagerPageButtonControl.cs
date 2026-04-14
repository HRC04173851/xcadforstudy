// -*- coding: utf-8 -*-
// PropertyPage/Toolkit/Controls/PropertyManagerPageButtonControl.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 按钮控件包装类，管理SOLIDWORKS按钮控件的点击事件和状态。
//*********************************************************************

using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using Xarial.XCad.SolidWorks.Services;
using Xarial.XCad.Toolkit.Services;
using Xarial.XCad.UI.PropertyPage.Base;
using Xarial.XCad.Utils.PageBuilder.Base;
using Xarial.XCad.Utils.PageBuilder.PageElements;

namespace Xarial.XCad.SolidWorks.UI.PropertyPage.Toolkit.Controls
{
    internal class PropertyManagerPageButtonControl : PropertyManagerPageBaseControl<Action, IPropertyManagerPageButton>
    {
#pragma warning disable CS0067

        protected override event ControlValueChangedDelegate<Action> ValueChanged;

#pragma warning restore CS0067

        private Action m_ButtonClickHandler;

        public PropertyManagerPageButtonControl(SwApplication app, IGroup parentGroup, IIconsCreator iconConv,
            IAttributeSet atts, IMetadata[] metadata, ref int numberOfUsedIds)
            : base(app, parentGroup, iconConv, atts, metadata, swPropertyManagerPageControlType_e.swControlType_Button, ref numberOfUsedIds)
        {
            m_Handler.ButtonPressed += OnButtonPressed;
        }

        protected override void SetOptions(IPropertyManagerPageButton ctrl, IControlOptionsAttribute opts, IAttributeSet atts)
        {
            ctrl.Caption = atts.Name;
        }

        private void OnButtonPressed(int id)
        {
            if (Id == id)
            {
                if (m_ButtonClickHandler == null) 
                {
                    throw new NullReferenceException("Button click handler is not specified. Set the value of the delegate to the handler method");
                }

                m_ButtonClickHandler.Invoke();
            }
        }

        protected override Action GetSpecificValue() => m_ButtonClickHandler;

        protected override void SetSpecificValue(Action value) => m_ButtonClickHandler = value;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                base.Dispose(disposing);

                m_Handler.ButtonPressed -= OnButtonPressed;
            }
        }
    }
}