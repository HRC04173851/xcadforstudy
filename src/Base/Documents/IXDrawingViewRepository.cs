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
using Xarial.XCad.Documents.Delegates;

namespace Xarial.XCad.Documents
{
    /// <summary>
    /// Represents the collection of drawing views
    /// 表示工程图视图集合
    /// </summary>
    public interface IXDrawingViewRepository : IXRepository<IXDrawingView>
    {
        /// <summary>
         /// Raised when new view is created
         /// 新建工程图视图时触发
         /// </summary>
        event DrawingViewCreatedDelegate ViewCreated;
    }
}
