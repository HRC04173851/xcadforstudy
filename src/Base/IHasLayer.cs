// -*- coding: utf-8 -*-
// src/Base/IHasLayer.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 图层接口，表示可放置在图层上的实体，继承自 IXObject，支持获取和设置对象所属图层。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xarial.XCad.Documents;

namespace Xarial.XCad
{
    /// <summary>
    /// Indicates that this entity can be placed on the layer
    /// 表示此实体可放置在图层上
    /// </summary>
    public interface IHasLayer : IXObject
    {
        /// <summary>
        /// Layer of this entity
        /// 此实体所在的图层
        /// </summary>
        IXLayer Layer { get; set; }
    }
}
