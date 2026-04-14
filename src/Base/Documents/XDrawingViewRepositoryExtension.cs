// -*- coding: utf-8 -*-
// src/Base/Documents/XDrawingViewRepositoryExtension.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 为工程图视图仓储提供扩展方法，支持基于模型视图创建工程图视图的功能
//*********************************************************************

using Xarial.XCad.Base;

namespace Xarial.XCad.Documents
{
    /// <summary>
    /// Extension methods for the <see cref="IXDrawingViewRepository"/>
    /// </summary>
    public static class XDrawingViewRepositoryExtension
    {
        /// <summary>
        /// Creates a view based on the model 3D view
        /// </summary>
        /// <param name="repo">Views repositry</param>
        /// <param name="view">Model based view to create drawing view from</param>
        /// <returns>Created drawing view</returns>
        public static IXModelViewBasedDrawingView CreateModelViewBased(this IXDrawingViewRepository repo, IXModelView view)
        {
            var drwView = repo.PreCreate<IXModelViewBasedDrawingView>();
            drwView.SourceModelView = view;

            repo.Add(drwView);

            return drwView;
        }
    }
}
