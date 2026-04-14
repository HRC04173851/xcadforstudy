// -*- coding: utf-8 -*-
// Commands/Exceptions/DuplicateCommandUserIdsException.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 重复命令用户ID异常，当命令组中存在重复的用户ID时抛出此异常。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.SolidWorks.UI.Commands.Exceptions
{
    public class DuplicateCommandUserIdsException : Exception
    {
        public DuplicateCommandUserIdsException(string groupTitle, int groupUserId, int[] dupUserIds)
            : base($"The following command user ids are duplicate in the group {groupTitle} [{groupUserId}]: {string.Join(", ", dupUserIds)}") 
        {
        }
    }
}
