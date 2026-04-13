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
using System.Threading;
using Xarial.XCad.Geometry;
using Xarial.XCad.Geometry.Primitives;
using Xarial.XCad.Services;
using Xarial.XCad.SolidWorks.Documents;

namespace Xarial.XCad.SolidWorks.Geometry.Primitives
{
    /// <summary>
    /// SolidWorks 临时几何体基础接口，表示尚未提交到模型的内存内几何原语。
    /// </summary>
    public interface ISwTempPrimitive : IXPrimitive
    {
        new ISwTempBody[] Bodies { get; }
    }

    /// <summary>
    /// SolidWorks 临时几何体基类，实现延迟创建和提交逻辑。
    /// </summary>
    internal class SwTempPrimitive : ISwTempPrimitive
    {
        IXBody[] IXPrimitive.Bodies => Bodies;

        public ISwTempBody[] Bodies => m_Creator.Element;

        public bool IsCommitted => m_Creator.IsCreated;

        protected readonly ISwApplication m_App;

        protected readonly IModeler m_Modeler;
        protected readonly IMathUtility m_MathUtils;
        
        protected readonly IElementCreator<ISwTempBody[]> m_Creator;
        
        internal SwTempPrimitive(SwTempBody[] bodies, ISwApplication app, bool isCreated) 
        {
            m_App = app;

            m_MathUtils = m_App.Sw.IGetMathUtility();
            m_Modeler = m_App.Sw.IGetModeler();

            m_Creator = new ElementCreator<ISwTempBody[]>(CreateBodies, bodies, isCreated);
        }

        protected virtual ISwTempBody[] CreateBodies(CancellationToken cancellationToken) 
        {
            throw new NotSupportedException();
        }

        public void Commit(CancellationToken cancellationToken) => m_Creator.Create(cancellationToken);
    }
}
