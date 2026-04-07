//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using Xarial.XCad.Base;
using Xarial.XCad.Features.CustomFeature;
using Xarial.XCad.Features.Delegates;

namespace Xarial.XCad.Features
{
    /// <summary>
    /// Represents collection of features in the document
    /// <para>中文：表示文档中特征的集合</para>
    /// </summary>
    public interface IXFeatureRepository : IXRepository<IXFeature>
    {
        /// <summary>
        /// Raised when new feature is created
        /// <para>中文：创建新特征时触发此事件</para>
        /// </summary>
        event FeatureCreatedDelegate FeatureCreated;

        /// <summary>
        /// Creates a custom feature with built-in editor for the property page
        /// <para>中文：创建带有属性管理器页面内置编辑器的自定义特征</para>
        /// </summary>
        /// <param name="data">Feature data</param>
        /// <typeparam name="TDef">Definition of the custom feature</typeparam>
        /// <typeparam name="TParams">Type which defines the data structure of the custom feature</typeparam>
        /// <typeparam name="TPage">Type which defines the data model for the property page</typeparam>
        void CreateCustomFeature<TDef, TParams, TPage>(TParams data)
            where TParams : class
            where TPage : class
            where TDef : class, IXCustomFeatureDefinition<TParams, TPage>, new();

        /// <summary>
        /// Enables or disables feature tree
        /// <para>中文：启用或禁用特征管理器树</para>
        /// </summary>
        /// <param name="enable">True to enable, False to disable</param>
        void Enable(bool enable);
    }
}