// -*- coding: utf-8 -*-
// src/Base/IXMaterialsDatabaseRepository.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 材料数据库仓储接口，用于管理多个材料数据库的集合，继承自仓库接口提供统一的访问方式。
//*********************************************************************

using Xarial.XCad.Base;

namespace Xarial.XCad
{
    /// <summary>
    /// Represents the materials database library
    /// 表示材料数据库库（多个材料数据库的集合）
    /// </summary>
    public interface IXMaterialsDatabaseRepository : IXRepository<IXMaterialsDatabase>
    {
    }
}
