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
using Xarial.XCad.Geometry;
using Xarial.XCad.Geometry.Curves;
using Xarial.XCad.Geometry.Structures;
using Xarial.XCad.Geometry.Wires;
using Xarial.XCad.SolidWorks.Documents;
using Xarial.XCad.SolidWorks.Geometry.Curves;
using Xarial.XCad.Utils.Reflection;

namespace Xarial.XCad.SolidWorks.Geometry
{
    /// <summary>
    /// SolidWorks 轮廓边接口，表示投影或功能视图中的轮廓边元素。
    /// </summary>
    public interface ISwSilhouetteEdge : ISwEntity, IXSilhouetteEdge
    {
        ISilhouetteEdge SilhouetteEdge { get; }
    }

    /// <summary>
    /// SolidWorks 轮廓边实现类。
    /// <para>注意：<see cref="ISilhouetteEdge"/> 不是 <see cref="IEntity"/>，直接访问 <see cref="Entity"/> 属性将抛出异常。</para>
    /// </summary>
    internal class SwSilhouetteEdge : SwEntity, ISwSilhouetteEdge
    {
        public ISilhouetteEdge SilhouetteEdge { get; }

        //NOTE: ISilhouetteEdge is not an IEntity
        internal SwSilhouetteEdge(ISilhouetteEdge silhouetteEdge, SwDocument doc, SwApplication app) : base(silhouetteEdge as IEntity, doc, app)
        {
            SilhouetteEdge = silhouetteEdge;
        }

        public override IEntity Entity => throw new NotSupportedException($"{nameof(ISilhouetteEdge)} is not an {nameof(IEntity)}");

        public IXFace Face => OwnerDocument.CreateObjectFromDispatch<ISwFace>(SilhouetteEdge.GetFace());

        public IXCurve Definition => OwnerApplication.CreateObjectFromDispatch<SwCurve>(SilhouetteEdge.GetCurve(), OwnerDocument);

        public IXPoint StartPoint => new SwMathPoint(SilhouetteEdge.GetStartPoint(), OwnerDocument, OwnerApplication);

        public IXPoint EndPoint => new SwMathPoint(SilhouetteEdge.GetStartPoint(), OwnerDocument, OwnerApplication);

        public double Length => Definition.Length;

        public override ISwBody Body => (ISwBody)Face.Body;

        public override ISwEntityRepository AdjacentEntities => new EmptySwEntityRepository();

        public override object Dispatch => SilhouetteEdge;

        public override ISwComponent Component 
        {
            get 
            {
                var comp = (IComponent2)((IEntity)SilhouetteEdge.GetFace()).GetComponent();

                if (comp != null)
                {
                    return OwnerDocument.CreateObjectFromDispatch<ISwComponent>(comp);
                }
                else
                {
                    return null;
                }
            }
        }

        internal override void Select(bool append, ISelectData selData)
        {
            if (OwnerApplication.IsVersionNewerOrEqual(Enums.SwVersion_e.Sw2014))
            {
                SilhouetteEdge.Select2(append, (SelectData)selData);
            }
            else 
            {
                SilhouetteEdge.Select(append, (SelectData)selData);
            }
        }

        public override Point FindClosestPoint(Point point)
            => new Point(((double[])SilhouetteEdge.GetCurve().GetClosestPointOn(point.X, point.Y, point.Z)).Take(3).ToArray());
    }

    internal class EmptySwEntityRepository : SwEntityRepository
    {
        protected override IEnumerable<ISwEntity> IterateEntities(bool faces, bool edges, bool vertices, bool silhouetteEdges)
        {
            yield break;
        }
    }
}