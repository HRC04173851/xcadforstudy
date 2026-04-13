//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xarial.XCad.Base;

namespace Xarial.XCad.Documents
{
    /// <summary>
    /// Represents layer for the entitites
    /// 表示实体图层
    /// </summary>
    /// <remarks>Entities which support layer are implementing <see cref="IHasLayer"/></remarks>
    public interface IXLayer : IXTransaction, IXObject, IHasColor
    {
        /// <summary>
        /// Name of the layer
        /// 图层名称
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Visibility of the layer
        /// 图层可见性
        /// </summary>
        bool Visible { get; set; }
    }
}
