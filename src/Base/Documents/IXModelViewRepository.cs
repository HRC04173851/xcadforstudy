//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Base;

namespace Xarial.XCad.Documents
{
    /// <summary>
    /// Represents collection of views in the <see cref="IXDocument"/>
    /// 表示 <see cref="IXDocument"/> 的视图集合
    /// </summary>
    public interface IXModelViewRepository : IXRepository<IXModelView>
    {
        /// <summary>
        /// Gets active view
        /// 获取当前激活视图
        /// </summary>
        IXModelView Active { get; }
    }

    /// <summary>
    /// Represents collection of views in the <see cref="IXDocument3D"/>
    /// 表示 <see cref="IXDocument3D"/> 的三维视图集合
    /// </summary>
    public interface IXModelView3DRepository : IXModelViewRepository
    {
        /// <summary>
        /// Returns standard view by type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        IXStandardView this[StandardViewType_e type] { get; }
    }
}
