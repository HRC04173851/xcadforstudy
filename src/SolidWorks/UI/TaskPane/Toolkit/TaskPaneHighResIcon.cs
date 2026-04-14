// -*- coding: utf-8 -*-
// TaskPane/Toolkit/TaskPaneHighResIcon.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 任务窗格高分辨率图标，支持多种尺寸的高分辨率任务窗格图标规格。
//*********************************************************************

using System.Collections.Generic;
using System.Drawing;
using Xarial.XCad.SolidWorks.Base;
using Xarial.XCad.Toolkit.Base;
using Xarial.XCad.UI;

namespace Xarial.XCad.SolidWorks.UI.Commands.Toolkit.Structures
{
    internal class TaskPaneHighResIcon : TaskPaneIcon
    {
        internal TaskPaneHighResIcon(IXImage icon) : base(icon)
        {
            IconSizes = new IIconSpec[]
            {
                new IconSpec(m_Icon, new Size(20, 20)),
                new IconSpec(m_Icon, new Size(32, 32)),
                new IconSpec(m_Icon, new Size(40, 40)),
                new IconSpec(m_Icon, new Size(64, 64)),
                new IconSpec(m_Icon, new Size(96, 96)),
                new IconSpec(m_Icon, new Size(128, 128))
            };
        }

        public override IIconSpec[] IconSizes { get; }
    }
}