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
using Xarial.XCad.Geometry;

namespace Xarial.XCad.Features
{
    /// <summary>
    /// Represents the flat pattern
    /// 表示钣金展开（Flat Pattern）特征
    /// </summary>
    public interface IXFlatPattern : IXFeature
    {
        /// <summary>
        /// Gets or sets if this flat pattern feature is flattened
        /// 获取或设置该展开特征是否处于展开状态
        /// </summary>
        bool IsFlattened { get; set; }

        /// <summary>
        /// Entity which is used as a fixed face
        /// 作为固定面的参考实体
        /// </summary>
        IXEntity FixedEntity { get; }
    }
}
