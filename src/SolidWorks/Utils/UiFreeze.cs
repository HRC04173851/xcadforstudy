// -*- coding: utf-8 -*-
// src/SolidWorks/Utils/UiFreeze.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 提供 UI 冻结功能，用于在批量操作时禁用特征树和图形视图更新以提升性能。
//*********************************************************************

using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xarial.XCad.SolidWorks.Documents;

namespace Xarial.XCad.SolidWorks.Utils
{
    /// <summary>
    /// Freezes feature tree and view
    /// </summary>
    internal class UiFreeze : IDisposable
    {
        private readonly IModelDoc2 m_Model;
        private readonly IModelView m_View;

        internal UiFreeze(ISwDocument doc)
        {
            m_Model = doc.Model;

            m_Model.FeatureManager.EnableFeatureTree = false;
            m_Model.FeatureManager.EnableFeatureTreeWindow = false;

            m_View = m_Model.IActiveView;

            if (m_View != null)
            {
                m_View.EnableGraphicsUpdate = false;
            }
        }

        public void Dispose()
        {
            m_Model.FeatureManager.EnableFeatureTree = true;
            m_Model.FeatureManager.EnableFeatureTreeWindow = true;

            if (m_View != null)
            {
                m_View.EnableGraphicsUpdate = true;
            }
        }
    }
}
