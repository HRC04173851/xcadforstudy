//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Xarial.XCad.Exceptions;
using Xarial.XCad.Geometry;
using Xarial.XCad.Geometry.Curves;
using Xarial.XCad.Geometry.Wires;
using Xarial.XCad.Services;
using Xarial.XCad.SolidWorks.Documents;
using Xarial.XCad.SolidWorks.Geometry.Curves;

namespace Xarial.XCad.SolidWorks.Geometry
{
    /// <summary>
    /// SolidWorks 环（Loop）接口。
    /// 环是面的边界闭合链，可表示外环（外边界）或内环（孔边界）。
    /// </summary>
    public interface ISwLoop : IXLoop, ISwSelObject
    {
        ILoop2 Loop { get; }
        ISwCurve[] Curves { get; set; }
    }

    /// <summary>
    /// SolidWorks 环实现类。
    /// 支持“虚拟环”模式：当环未提交时仅缓存曲线集合，用于需要环参数的构造流程。
    /// </summary>
    internal class SwLoop : SwSelObject, ISwLoop
    {
        IXSegment[] IXLoop.Segments
        {
            get => Curves;
            set => Curves = value?.Cast<ISwCurve>().ToArray();
        }

        public ILoop2 Loop 
        {
            get 
            {
                if (IsCommitted)
                {
                    if (!m_IsVirtual)
                    {
                        return m_Creator.Element;
                    }
                    else 
                    {
                        throw new Exception("Virtual loop cannot be accessed");
                    }
                }
                else 
                {
                    throw new NonCommittedElementAccessException();
                }
            }
        }

        public override bool IsCommitted => m_Creator.IsCreated;

        private readonly ElementCreator<ILoop2> m_Creator;

        private bool m_IsVirtual;

        internal SwLoop(ILoop2 loop, SwDocument doc, SwApplication app) : base(loop, doc, app)
        {
            //new loops cannot be create, so this loop is just a placeholder of curves for other methods which requires loops
            // 中文：Loop 在 SolidWorks API 中通常不可直接新建，这里使用虚拟环占位保存曲线参数
            m_IsVirtual = loop == null;
            m_Creator = new ElementCreator<ILoop2>(CreateLoop, loop, loop != null);
        }

        private ILoop2 CreateLoop(CancellationToken cancellationToken) => null;

        public ISwCurve[] Curves
        {
            get
            {
                if (!m_IsVirtual)
                {
                    return (Loop.GetEdges() as object[])
                        .Cast<IEdge>()
                        .Select(e => OwnerApplication.CreateObjectFromDispatch<ISwCurve>(e.IGetCurve().ICopy(), OwnerDocument))
                        .ToArray();
                }
                else
                {
                    return m_Creator.CachedProperties.Get<ISwCurve[]>();
                }
            }
            set 
            {
                if (!IsCommitted)
                {
                    m_Creator.CachedProperties.Set(value);
                }
                else 
                {
                    throw new CommitedElementReadOnlyParameterException();
                }
            }
        }

        public override void Commit(CancellationToken cancellationToken) => m_Creator.Create(cancellationToken);
    }
}
