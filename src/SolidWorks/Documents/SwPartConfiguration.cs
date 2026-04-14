// -*- coding: utf-8 -*-
// src/SolidWorks/Documents/SwPartConfiguration.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件实现 SolidWorks 零件配置（Part Configuration）的封装。
// 零件配置用于定义零件模型的不同变体，支持在同一零件文档中管理多个设计状态，
// 如不同的尺寸参数、压缩特征等，是零件设计变体管理的核心机制。
//*********************************************************************

using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Collections.Generic;
using System.Linq;
using Xarial.XCad.Documents;
using Xarial.XCad.Features;
using Xarial.XCad.SolidWorks.Documents.Exceptions;
using Xarial.XCad.SolidWorks.Features;

namespace Xarial.XCad.SolidWorks.Documents
{
    public interface ISwPartConfiguration : ISwConfiguration, IXPartConfiguration
    {
    }

    internal class SwPartConfiguration : SwConfiguration, ISwPartConfiguration
    {
        private readonly SwPart m_Part;

        internal SwPartConfiguration(IConfiguration conf, SwPart part, SwApplication app, bool created) 
            : base(conf, part, app, created)
        {
            m_Part = part;
            CutLists = new SwPartCutListItemCollection(this, part);
        }

        public IXCutListItemRepository CutLists { get; }

        public IXMaterial Material 
        {
            get 
            {
                var materialName = m_Part.Part.GetMaterialPropertyName2(Name, out var database);

                if (!string.IsNullOrEmpty(materialName))
                {
                    return new SwMaterial(materialName, OwnerApplication.MaterialDatabases.GetOrTemp(database));
                }
                else 
                {
                    return null;
                }
            }
            set 
            {
                if (value != null)
                {
                    m_Part.Part.SetMaterialPropertyName2(Name, value.Database.Name, value.Name);
                }
                else 
                {
                    m_Part.Part.SetMaterialPropertyName2(Name, "", "");
                }
            }
        }
    }
}