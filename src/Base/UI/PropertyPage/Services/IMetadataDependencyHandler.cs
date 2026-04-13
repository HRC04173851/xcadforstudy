//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using Xarial.XCad.UI.PropertyPage.Base;

namespace Xarial.XCad.UI.PropertyPage.Services
{
    /// <summary>
    /// Handling the dynamic control dependencies
    /// 处理基于元数据的动态依赖关系
    /// </summary>
    /// <remarks>This is asigned via <see cref="Attributes.DependentOnAttribute"/></remarks>
    public interface IMetadataDependencyHandler
    {
        /// <summary>
        /// Invokes when any of the dependencies controls changed
        /// 当元数据依赖变化时调用
        /// </summary>
        /// <param name="app">Main application</param>
        /// <param name="source">This control to update state on</param>
        /// <param name="metadata">List of metadata dependencies</param>
        void UpdateState(IXApplication app, IControl source, IMetadata[] metadata);
    }
}
