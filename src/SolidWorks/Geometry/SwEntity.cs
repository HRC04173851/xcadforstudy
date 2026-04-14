// -*- coding: utf-8 -*-
// src/SolidWorks/Geometry/SwEntity.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件实现 SolidWorks 几何实体（Entity）的基本封装类。
// 实体是拓扑元素（Face、Edge、Vertex）的基类，提供选择、最近点查找等功能。
// 支持韧性（Resilience）特性，可在文档重新加载后保持有效引用。
// 作为 Face、Edge、Vertex 等拓扑对象的共同基类。
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
    public interface ISwEntity : ISwSelObject, IXEntity, ISupportsResilience<ISwEntity>
    {
        IEntity Entity { get; }

        new ISwComponent Component { get; }
        new ISwEntityRepository AdjacentEntities { get; }
        new ISwBody Body { get; }
    }

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
            if (!Entity.Select4(append, (SelectData)selData))
            {
                throw new Exception("Failed to select entity");
            }
        }

        public abstract Point FindClosestPoint(Point point);

        public ISwEntity CreateResilient()
        {
            var safeEnt = Entity.GetSafeEntity();

            if (safeEnt == null) 
            {
                throw new NullReferenceException("Failed to get safe entity");
            }

            return OwnerApplication.CreateObjectFromDispatch<SwEntity>(safeEnt, OwnerDocument);
        }
    }
}