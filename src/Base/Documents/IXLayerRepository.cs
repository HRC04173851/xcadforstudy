// -*- coding: utf-8 -*-
// src/Base/Documents/IXLayerRepository.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义图层仓储接口，管理文档中的图层集合，
// 提供当前激活图层的访问和设置功能。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xarial.XCad.Base;

namespace Xarial.XCad.Documents
{
    /// <summary>
    /// Represents the repository of layers
    /// 表示图层仓储
    /// </summary>
    public interface IXLayerRepository : IXRepository<IXLayer>
    {
        /// <summary>
        /// Gets or sets the current layer
        /// 获取或设置当前图层
        /// </summary>
        IXLayer Active { get; set; }
    }
}
