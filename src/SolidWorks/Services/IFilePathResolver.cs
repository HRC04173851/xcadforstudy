//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Documents;
using Xarial.XCad.SolidWorks.Documents;

namespace Xarial.XCad.SolidWorks.Services
{
    /// <summary>
    /// 文件路径解析服务接口。
    /// 用于解析装配体/零件引用文件的实际路径。
    /// </summary>
    public interface IFilePathResolver
    {
        string ResolvePath(string parentDocPath, string path);
    }
}
