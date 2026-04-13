//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xarial.XCad.UI.PropertyPage.Base;

namespace Xarial.XCad.Utils.PageBuilder.Base
{
    /// <summary>
    /// Manages dynamic dependencies between controls and metadata.
    /// <para>管理控件与元数据之间的动态依赖关系。</para>
    /// </summary>
    public interface IDependencyManager
    {
        /// <summary>
        /// Initializes dependency manager with raw dependency definitions.
        /// <para>使用原始依赖定义初始化依赖管理器。</para>
        /// </summary>
        void Init(IXApplication app, IRawDependencyGroup depGroup);
        /// <summary>
        /// Re-evaluates and updates all registered dependencies.
        /// <para>重新计算并更新所有已注册依赖。</para>
        /// </summary>
        void UpdateAll();
    }
}