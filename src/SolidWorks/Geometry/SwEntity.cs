//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using SolidWorks.Interop.sldworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Xarial.XCad.Base;
using Xarial.XCad.Documents;
using Xarial.XCad.Geometry;
using Xarial.XCad.Geometry.Structures;
using Xarial.XCad.SolidWorks.Documents;
using Xarial.XCad.Toolkit.Utils;

namespace Xarial.XCad.SolidWorks.Geometry
{
    /// <summary>
    /// SolidWorks 几何实体接口（面、边、顶点等共同基接口）。
    /// </summary>
    public interface ISwEntity : ISwSelObject, IXEntity, ISupportsResilience<ISwEntity>
    {
        /// <summary>底层 SolidWorks IEntity COM 对象。</summary>
        IEntity Entity { get; }

        new ISwComponent Component { get; }
        new ISwEntityRepository AdjacentEntities { get; }
        new ISwBody Body { get; }
    }

    /// <summary>
    /// SolidWorks 几何实体抽象基类。
    /// 提供组件归属、安全实体恢复（Resilient）、选择与最近点查询基础能力。
    /// </summary>
    internal abstract class SwEntity : SwSelObject, ISwEntity
    {
        IXBody IXEntity.Body => Body;
        IXEntityRepository IXEntity.AdjacentEntities => AdjacentEntities;
        IXComponent IXEntity.Component => Component;
        IXObject ISupportsResilience.CreateResilient() => CreateResilient();

        public virtual IEntity Entity { get; }

        public override object Dispatch => Entity;

        public abstract ISwBody Body { get; }

        public abstract ISwEntityRepository AdjacentEntities { get; }

        public virtual ISwComponent Component 
        {
            get 
            {
                // 装配体上下文下，实体可能归属于某个组件（Component2）
                var comp = (IComponent2)Entity.GetComponent();

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

        public override bool IsAlive => this.CheckIsAlive(() => { var test = Entity.IsSafe; });

        public bool IsResilient => Entity.IsSafe;

        internal SwEntity(IEntity entity, SwDocument doc, SwApplication app) : base(entity, doc, app)
        {
            Entity = entity;
        }

        internal override void Select(bool append, ISelectData selData)
        {
            // 实体选择使用 IEntity.Select4，而非通用 MultiSelect2
            if (!Entity.Select4(append, (SelectData)selData))
            {
                throw new Exception("Failed to select entity");
            }
        }

        public abstract Point FindClosestPoint(Point point);

        public ISwEntity CreateResilient()
        {
            // 获取 SafeEntity，用于在拓扑变更后尽量保持实体引用有效
            var safeEnt = Entity.GetSafeEntity();

            if (safeEnt == null) 
            {
                throw new NullReferenceException("Failed to get safe entity");
            }

            return OwnerApplication.CreateObjectFromDispatch<SwEntity>(safeEnt, OwnerDocument);
        }
    }
}