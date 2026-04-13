//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.UI.PropertyPage.Base;

namespace Xarial.XCad.Toolkit.PageBuilder.Binders
{
    /// <summary>
    /// Metadata implementation holding static value.
    /// <para>保存静态值的元数据实现。</para>
    /// </summary>
    public class StaticMetadata : IMetadata
    {
        public event Action<IMetadata, object> Changed;

        public object Tag => null;

        public object Value { get; set; }
        
        /// <summary>
        /// Initializes static metadata.
        /// <para>初始化静态元数据对象。</para>
        /// </summary>
        public StaticMetadata(object value) 
        {
            Value = value;
        }
    }
}
