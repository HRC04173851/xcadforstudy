//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

namespace Xarial.XCad.Features.CustomFeature.Enums
{
    /// <summary>
    /// Reason of macro feature handler unloading. Used in <see cref="Services.ICustomFeatureHandler.Unload(CustomFeatureUnloadReason_e)"/>
    /// 宏特征处理器卸载原因（用于 <see cref="Services.ICustomFeatureHandler.Unload(CustomFeatureUnloadReason_e)"/>）
    /// </summary>
    public enum CustomFeatureUnloadReason_e
    {
        /// <summary>
        /// Model containing this macro feature is closed
        /// 包含该宏特征的模型已关闭
        /// </summary>
        ModelClosed,

        /// <summary>
        /// This macro feature is deleted
        /// 该宏特征被删除
        /// </summary>
        Deleted
    }
}