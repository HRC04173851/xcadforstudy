//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Data.Enums;

namespace Xarial.XCad.Toolkit.Utils
{
    /// <summary>
    /// Helper methods for access mode conversion.
    /// <para>用于访问模式转换的辅助方法。</para>
    /// </summary>
    public static class AccessTypeHelper
    {
        /// <summary>
        /// Determines whether access mode is write-enabled.
        /// <para>判断访问类型是否为写入模式。</para>
        /// </summary>
        public static bool GetIsWriting(AccessType_e type)
        {
            switch (type)
            {
                case AccessType_e.Write:
                    return true;

                case AccessType_e.Read:
                    return false;

                default:
                    throw new NotSupportedException();
            }
        }
    }
}
