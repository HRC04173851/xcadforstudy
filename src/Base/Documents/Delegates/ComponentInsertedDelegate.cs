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
using System.Threading.Tasks;

namespace Xarial.XCad.Documents.Delegates
{
    /// <summary>
    /// Delegate of <see cref="IXAssembly.ComponentInserted"/> notification
    /// <see cref="IXAssembly.ComponentInserted"/> 通知委托
    /// </summary>
    /// <param name="assembly">Assembly where component is inserted</param>
    /// <param name="component">Component inserted into the assembly</param>
    public delegate void ComponentInsertedDelegate(IXAssembly assembly, IXComponent component);
}
