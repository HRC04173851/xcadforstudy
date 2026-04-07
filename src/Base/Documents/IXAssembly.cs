//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using Xarial.XCad.Documents.Delegates;
using Xarial.XCad.Geometry;

namespace Xarial.XCad.Documents
{
    /// <summary>
    /// Represents an assembly document (composition of <see cref="IXPart"/> and other <see cref="IXAssembly"/>)
    /// <para>中文：表示装配体文档（由零件和子装配体组成）</para>
    /// </summary>
    public interface IXAssembly : IXDocument3D
    {
        /// <summary>
        /// Raised when new component is inserted into the assembly
        /// <para>中文：新组件插入装配体时触发</para>
        /// </summary>
        event ComponentInsertedDelegate ComponentInserted;

        /// <summary>
        /// Raised when component is about to be deleted from the assembly
        /// <para>中文：组件即将从装配体中删除时触发</para>
        /// </summary>
        event ComponentDeletingDelegate ComponentDeleting;

        /// <summary>
        /// Raised when component is deleted from the assembly
        /// <para>中文：组件已从装配体中删除时触发</para>
        /// </summary>
        event ComponentDeletedDelegate ComponentDeleted;

        /// <inheritdoc/>
        new IXAssemblyConfigurationRepository Configurations { get; }

        /// <inheritdoc/>
        new IXAssemblyEvaluation Evaluation { get; }

        /// <summary>
        /// Returns the component which is currently being editied in-context or null
        /// <para>中文：返回当前正在进行关联编辑的组件，若无则返回 null</para>
        /// </summary>
        IXComponent EditingComponent { get; }
    }
}