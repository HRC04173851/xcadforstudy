// -*- coding: utf-8 -*-
// src/SwDocumentManager/Features/SwDmCutListItem.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 包装焊件或钣金中的切割清单项目，提供 BOM 状态、清单类型判断及占位实体体数量展开功能。
//*********************************************************************

using SolidWorks.Interop.swdocumentmgr;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Xarial.XCad.Annotations;
using Xarial.XCad.Base.Enums;
using Xarial.XCad.Data;
using Xarial.XCad.Documents;
using Xarial.XCad.Enums;
using Xarial.XCad.Features;
using Xarial.XCad.Geometry;
using Xarial.XCad.SwDocumentManager.Data;
using Xarial.XCad.SwDocumentManager.Documents;
using Xarial.XCad.SwDocumentManager.Geometry;

namespace Xarial.XCad.SwDocumentManager.Features
{
    /// <summary>
    /// Cut-list item contract backed by Document Manager.
    /// 由 Document Manager 支持的切割清单项目约定。
    /// </summary>
    public interface ISwDmCutListItem : ISwDmObject, IXCutListItem
    {
        new ISwDmCustomPropertiesCollection Properties { get; }
        ISwDMCutListItem2 CutListItem { get; }
    }

    /// <summary>
    /// Wraps a weldment or sheet-metal cut-list item.
    /// 包装焊件或钣金中的切割清单项目。
    /// </summary>
    [DebuggerDisplay("{" + nameof(Name) + "}")]
    internal class SwDmCutListItem : SwDmSelObject, ISwDmCutListItem
    {
        #region Not Supported
        public IXDimensionRepository Dimensions => throw new NotSupportedException();
        public Color? Color { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
        public IEnumerable<IXFace> Faces => throw new NotSupportedException();
        FeatureState_e IXFeature.State { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
        public IXComponent Component => throw new NotSupportedException();
        public IEditor<IXFeature> Edit() => throw new NotSupportedException();
        public void Update() => throw new NotSupportedException();
        public IXBody Body => throw new NotSupportedException();
        public IXEntityRepository AdjacentEntities => throw new NotSupportedException();
        public XCad.Geometry.Structures.Point FindClosestPoint(XCad.Geometry.Structures.Point point) => throw new NotSupportedException();
        public bool IsUserFeature => throw new NotSupportedException();
        #endregion

        IXPropertyRepository IPropertiesOwner.Properties => Properties;
        
        public ISwDMCutListItem2 CutListItem { get; }

        private readonly Lazy<ISwDmCustomPropertiesCollection> m_Properties;
        private readonly SwDmPart m_Part;
        private readonly ISwDmPartConfiguration m_Conf;

        /// <summary>
        /// Creates a cut-list item wrapper for the document-level cut-list scope.
        /// 为文档级切割清单范围创建切割清单项目包装器。
        /// </summary>
        internal SwDmCutListItem(ISwDMCutListItem2 cutListItem, SwDmPart doc) : base(cutListItem, doc.OwnerApplication, doc)
        {
            CutListItem = cutListItem;
            m_Part = doc;
            
            m_Properties = new Lazy<ISwDmCustomPropertiesCollection>(
                () => new SwDmCutListCustomPropertiesCollection(this, m_Part, m_Conf));
        }

        internal SwDmCutListItem(ISwDMCutListItem2 cutListItem, SwDmPart doc, ISwDmPartConfiguration conf) : this(cutListItem, doc)
        {
            m_Conf = conf;
        }

        /// <summary>
        /// Enumerates placeholder solid bodies based on the cut-list quantity.
        /// 根据切割清单数量枚举占位实体体，用于表达该清单项对应的实体数量。
        /// </summary>
        public IEnumerable<IXSolidBody> Bodies 
        {
            get 
            {
                for (int i = 0; i < CutListItem.Quantity; i++) 
                {
                    yield return new SwDmSolidBody(m_Part);
                }
            }
        }

        public string Name 
        {
            get => CutListItem.Name; 
            set => CutListItem.Name = value; 
        }

        public ISwDmCustomPropertiesCollection Properties => m_Properties.Value;

        /// <summary>
        /// Returns the BOM inclusion/exclusion status of the cut-list item.
        /// 返回切割清单项目在物料清单中的包含或排除状态。
        /// </summary>
        public CutListStatus_e Status
        {
            get 
            {
                if (m_Part.IsVersionNewerOrEqual(SwDmVersion_e.Sw2021))
                {
                    var cutListStatus = (CutListItem as ISwDMCutListItem4).ExcludeFromCutlist;

                    if (cutListStatus == swDMCutListExclusionStatus_e.swDMCutListStatus_Excluded)
                    {
                        return CutListStatus_e.ExcludeFromBom;
                    }
                    else if (cutListStatus == swDMCutListExclusionStatus_e.swDMCutListStatus_Included)
                    {
                        return 0;
                    }
                    else
                    {
                        throw new Exception("Failed to extract the BOM status. Save document in SW 2021 or newer");
                    }
                }
                else 
                {
                    throw new NotSupportedException("This API is available in SW 2021 or newer");
                }
            }
        }

        /// <summary>
        /// Returns the cut-list semantic type such as实体体、钣金或焊件。
        /// Returns the cut-list semantic type such as solid body, sheet metal, or weldment.
        /// </summary>
        public CutListType_e Type 
        {
            get 
            {
                if (m_Part.IsVersionNewerOrEqual(SwDmVersion_e.Sw2021))
                {
                    switch (((ISwDMCutListItem4)CutListItem).CutlistType) 
                    {
                        case swDMCutListType_e.swDMCutListType_SolidBody:
                            return CutListType_e.SolidBody;

                        case swDMCutListType_e.swDMCutListType_Sheetmetal:
                            return CutListType_e.SheetMetal;

                        case swDMCutListType_e.swDMCutListType_Weldment:
                            return CutListType_e.Weldment;

                        default:
                            throw new NotSupportedException("Unrecognized cut-list item type");
                    }
                }
                else 
                {
                    throw new NotSupportedException("This propery is only supported in SOLIDWORKS 2021 or newer");
                }
            }
        }
    }
}
