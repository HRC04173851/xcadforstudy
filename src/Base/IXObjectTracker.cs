//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Base.Enums;
using Xarial.XCad.Documents;
using Xarial.XCad.Geometry;

namespace Xarial.XCad
{
    /// <summary>
    /// Allows to track objects across operations
    /// 允许跨操作跟踪对象
    /// </summary>
    public interface IXObjectTracker : IDisposable
    {
        /// <summary>
        /// Tracks the specified object
        /// 跟踪指定对象
        /// </summary>
        /// <param name="obj">Object to track 要跟踪的对象</param>
        /// <param name="trackId">Tracking id 跟踪 ID</param>
        void Track(IXObject obj, int trackId);

        /// <summary>
        /// Stops tracking of the specified object
        /// 停止跟踪指定对象
        /// </summary>
        /// <param name="obj">Object to untrack 要取消跟踪的对象</param>
        void Untrack(IXObject obj);

        /// <summary>
        /// Checks if the object is currently being tracked
        /// 检查对象是否正在被跟踪
        /// </summary>
        /// <param name="obj">Object to check 要检查的对象</param>
        /// <returns>True if object is tracked, False if not 对象正在被跟踪返回 true</returns>
        bool IsTracked(IXObject obj);

        /// <summary>
        /// Finds the tracked objects of this tracker in the specified context
        /// 在指定上下文中查找此跟踪器跟踪的对象
        /// </summary>
        /// <param name="doc">Document where to find tracked objects 要查找被跟踪对象的文档</param>
        /// <param name="searchBody">Optional body where to find the tracked objects, null to search in all objects 可选搜索范围实体，null 表示全部</param>
        /// <param name="searchFilter">Optional filters of the objects to find, null to find all ids 可选对象类型过滤器</param>
        /// <param name="searchTrackIds">Optional ids to find, null to find all ids 可选跟踪 ID 列表</param>
        /// <returns>Tracked object or empty if no objects found 跟踪对象数组，未找到时返回空数组</returns>
        IXObject[] FindTrackedObjects(IXDocument doc, IXBody searchBody = null, Type[] searchFilter = null, int[] searchTrackIds = null);

        /// <summary>
        /// Finds the tracking id of the specified object
        /// 获取指定对象的跟踪 ID
        /// </summary>
        /// <param name="obj">Object to find th tracking id 要获取跟踪 ID 的对象</param>
        /// <returns>Tracking id 跟踪 ID</returns>
        int GetTrackingId(IXObject obj);
    }
}
