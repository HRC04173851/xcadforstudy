//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.Toolkit.Exceptions
{
    /// <summary>
    /// Exception indicates that the element is already committed.
    /// <para>异常指示元素（Element，例如CAD实体或特征）已经被提交执行。</para>
    /// </summary>
    public class ElementAlreadyCommittedException : Exception
    {
        /// <summary>
        /// Default constructor.
        /// <para>默认构造函数。</para>
        /// </summary>
        public ElementAlreadyCommittedException() : base("This element already committed") 
        {
        }
    }
}
