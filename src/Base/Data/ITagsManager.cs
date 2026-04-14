// -*- coding: utf-8 -*-
// ITagsManager.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义标签管理器接口，用于管理 IXObject.Tags 中的用户自定义元数据。
// 提供添加、获取、弹出和检查元数据的方法，支持任意类型的标签数据存储。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad.Data
{
    /// <summary>
    /// Manages custom user metadata within this <see cref="IXObject.Tags"/>
    /// 管理 <see cref="IXObject.Tags"/> 中的用户自定义元数据
    /// </summary>
    public interface ITagsManager
    {
        /// <summary>
        /// Checks if the tags manager has any tags
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// Checks if the tags contains named metadata
        /// </summary>
        /// <param name="name">Name of the metadata</param>
        /// <returns>True if metadata exists</returns>
        bool Contains(string name);
        
        /// <summary>
        /// Adds the specified metadata to the tags
        /// </summary>
        /// <typeparam name="T">Type of metadata</typeparam>
        /// <param name="name">Name of metadata</param>
        /// <param name="value">Metadata</param>
        void Put<T>(string name, T value);

        /// <summary>
        /// Returns the specified metadata by name
        /// </summary>
        /// <typeparam name="T">Type of the metadata</typeparam>
        /// <param name="name">Name of the metadata</param>
        /// <returns>Instance of the metadata</returns>
        T Get<T>(string name);

        /// <summary>
        /// Pops the specified metadata from the tags (instance is removed from the tags)
        /// </summary>
        /// <typeparam name="T">Type of the metadata</typeparam>
        /// <param name="name">Name of the metadata</param>
        /// <returns>Instance of the metadata</returns>
        T Pop<T>(string name);
    }
}
