//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using Xarial.XCad.Base;
using Xarial.XCad.Documents.Delegates;

namespace Xarial.XCad.Documents
{
    /// <summary>
    /// Represents sheets collection
    /// 表示图纸集合仓储
    /// </summary>
    public interface IXSheetRepository : IXRepository<IXSheet>
    {
        /// <summary>
        /// Fired when sheet is activated
        /// 图纸激活时触发
        /// </summary>
        event SheetActivatedDelegate SheetActivated;

        /// <summary>
        /// Fired when new sheet is created
        /// 新建图纸时触发
        /// </summary>
        event SheetCreatedDelegate SheetCreated;

        /// <summary>
        /// Returns or sets the active sheet in this sheets repository
        /// 获取或设置当前激活图纸
        /// </summary>
        IXSheet Active { get; set; }
    }
}
