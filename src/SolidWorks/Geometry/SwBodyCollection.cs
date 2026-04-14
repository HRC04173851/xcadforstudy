// -*- coding: utf-8 -*-
// src/SolidWorks/Geometry/SwBodyCollection.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件实现 SolidWorks Body 集合（Body Collection）的封装。
// 提供文档中所有几何体的枚举和查询功能。
// 支持按类型（实体、片体、线框）过滤筛选，
// 是访问和管理文档几何体对象的主要入口。
//*********************************************************************

using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Xarial.XCad.Base;
using Xarial.XCad.Geometry;
using Xarial.XCad.SolidWorks.Documents;
using Xarial.XCad.Toolkit.Utils;

namespace Xarial.XCad.SolidWorks.Geometry
{
    public interface ISwBodyCollection : IXBodyRepository 
    {
    }

    internal abstract class SwBodyCollection : ISwBodyCollection
    {
        private readonly ISwDocument m_RootDoc;

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        internal SwBodyCollection(ISwDocument rootDoc)
        {
            m_RootDoc = rootDoc;
        }

        public IXBody this[string name] => RepositoryHelper.Get(this, name);

        public int Count => SelectAllBodies().Count();

        public void AddRange(IEnumerable<IXBody> ents, CancellationToken cancellationToken)
            => throw new NotSupportedException();

        public T PreCreate<T>() where T : IXBody => throw new NotSupportedException();

        public IEnumerator<IXBody> GetEnumerator() => SelectAllBodies().GetEnumerator();

        public void RemoveRange(IEnumerable<IXBody> ents, CancellationToken cancellationToken)
            => throw new NotSupportedException();

        public bool TryGet(string name, out IXBody ent)
        {
            ent = SelectAllBodies().FirstOrDefault(
                b => string.Equals(b.Name, name, StringComparison.CurrentCultureIgnoreCase));

            return ent != null;
        }

        public IEnumerable Filter(bool reverseOrder, params RepositoryFilterQuery[] filters)
        {
            bool solid;
            bool surface;
            bool wire;

            if (filters?.Any() == true)
            {
                solid = false;
                surface = false;
                wire = false;

                foreach (var filter in filters)
                {
                    solid = filter.Type == null || typeof(IXSolidBody).IsAssignableFrom(filter.Type);
                    surface = filter.Type == null || typeof(IXSheetBody).IsAssignableFrom(filter.Type);
                    wire = filter.Type == null || typeof(IXWireBody).IsAssignableFrom(filter.Type);
                }
            }
            else
            {
                solid = true;
                surface = true;
                wire = true;
            }

            foreach (var ent in RepositoryHelper.FilterDefault(TrySelectSpecificBodies(solid, surface, wire), filters, reverseOrder))
            {
                yield return ent;
            }
        }

        private IEnumerable<SwBody> SelectAllBodies() => TrySelectSpecificBodies(true, true, true);

        private IEnumerable<SwBody> TrySelectSpecificBodies(bool solid, bool surface, bool wire) 
        {
            swBodyType_e bodyType;

            if (solid && !surface & !wire)
            {
                bodyType = swBodyType_e.swSolidBody;
            }
            else if (surface && !solid & !wire)
            {
                bodyType = swBodyType_e.swSheetBody;
            }
            else if (wire && !solid & !surface)
            {
                bodyType = swBodyType_e.swWireBody;
            }
            else 
            {
                bodyType = swBodyType_e.swAllBodies;
            }

            foreach (var swBody in SelectSwBodies(bodyType) ?? Enumerable.Empty<IBody2>()) 
            {
                yield return m_RootDoc.CreateObjectFromDispatch<SwBody>(swBody);
            }
        }

        protected abstract IEnumerable<IBody2> SelectSwBodies(swBodyType_e bodyType);
    }
}
