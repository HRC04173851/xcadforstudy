//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Geometry.Surfaces;
using Xarial.XCad.SolidWorks;
using Xarial.XCad.SolidWorks.Documents;
using Xarial.XCad.SolidWorks.Geometry.Surfaces;

namespace Xarial.XCad.SolidWorks.Geometry.Surfaces
{
    /// <summary>
    /// SolidWorks 拉伸面接口。
    /// </summary>
    public interface ISwExtrudedSurface : ISwSurface, IXExtrudedSurface
    {
    }

    /// <summary>
    /// SolidWorks 拉伸面实现类。
    /// </summary>
    internal class SwExtrudedSurface : SwSurface, ISwExtrudedSurface
    {
        public SwExtrudedSurface(ISurface surface, SwDocument doc, SwApplication app) : base(surface, doc, app)
        {
        }
    }
}
