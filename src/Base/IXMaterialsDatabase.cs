//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System.Collections.Generic;
using Xarial.XCad.Base;

namespace Xarial.XCad
{
    /// <summary>
    /// Represents the materials database
    /// 表示材料数据库
    /// </summary>
    public interface IXMaterialsDatabase : IXRepository<IXMaterial>, IXTransaction
    {
        /// <summary>
        /// Name of the materials database
        /// 材料数据库的名称
        /// </summary>
        string Name { get; }
    }
}
