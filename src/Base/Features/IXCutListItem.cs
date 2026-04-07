//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xarial.XCad.Data;
using Xarial.XCad.Enums;
using Xarial.XCad.Features.Delegates;
using Xarial.XCad.Geometry;

namespace Xarial.XCad.Features
{
    /// <summary>
    /// Represents the cut-list item feature
    /// <para>中文：表示切割清单项目特征</para>
    /// </summary>
    public interface IXCutListItem : IXFeature, IPropertiesOwner
    {
        /// <summary>
        /// Status of this cut-list item
        /// <para>中文：此切割清单项目的状态</para>
        /// </summary>
        CutListStatus_e Status { get; }

        /// <summary>
        /// Type of the cut-list
        /// <para>中文：切割清单的类型（例如钣金或焊件）</para>
        /// </summary>
        CutListType_e Type { get; }

        /// <summary>
        /// Bodies of this cut-list item
        /// <para>中文：此切割清单项目包含的实体列表</para>
        /// </summary>
        IEnumerable<IXSolidBody> Bodies { get; }

        /// <summary>
        /// Updates cut-lists folder
        /// <para>中文：更新切割清单文件夹</para>
        /// </summary>
        void Update();
    }

    /// <summary>
    /// Additional methods of <see cref="IXCutListItem"/>
    /// <para>中文：<see cref="IXCutListItem"/> 的附加扩展方法</para>
    /// </summary>
    public static class XCutListItemExtension 
    {
        /// <summary>
        /// Gets the quantity of this cut-list-item
        /// <para>中文：获取此切割清单项目的数量</para>
        /// </summary>
        /// <param name="item">Input item</param>
        /// <returns>Quantity</returns>
        public static int Quantity(this IXCutListItem item) => item.Bodies.Count();
    }
}
