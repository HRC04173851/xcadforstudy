// -*- coding: utf-8 -*-
// src/SolidWorks/Documents/EventHandlers/ConfigurationActivatedEventsHandler.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件实现配置激活事件的回调处理。
// ConfigurationActivatedDelegate 事件在3D文档（零件或装配体）的
// 配置被激活或切换时触发，用于响应配置变更并执行相应操作。
//*********************************************************************

using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Documents.Delegates;
using Xarial.XCad.SolidWorks.Documents;
using Xarial.XCad.SolidWorks.Utils;

namespace Xarial.XCad.SolidWorks.Documents.EventHandlers
{
    internal class ConfigurationActivatedEventsHandler : SwModelEventsHandler<ConfigurationActivatedDelegate>
    {
        private readonly ISwDocument3D m_Doc3D;

        internal ConfigurationActivatedEventsHandler(SwDocument3D doc, ISwApplication app) : base(doc, app)
        {
            m_Doc3D = doc;
        }

        protected override void SubscribeAssemblyEvents(AssemblyDoc assm)
        {
            assm.ConfigurationChangeNotify += OnConfigurationChangeNotify;
        }
        
        protected override void SubscribePartEvents(PartDoc part)
        {
            part.ConfigurationChangeNotify += OnConfigurationChangeNotify;
        }

        protected override void UnsubscribeAssemblyEvents(AssemblyDoc assm)
        {
            assm.ConfigurationChangeNotify -= OnConfigurationChangeNotify;
        }

        protected override void UnsubscribePartEvents(PartDoc part)
        {
            part.ConfigurationChangeNotify -= OnConfigurationChangeNotify;
        }

        private int OnConfigurationChangeNotify(string configurationName, object obj, int objectType, int changeType)
        {
            const int POST_NOTIFICATION = 11;

            if (changeType == POST_NOTIFICATION)
            {
                Delegate?.Invoke(m_Doc3D, m_Doc3D.Configurations[configurationName]);
            }

            return HResult.S_OK;
        }
    }
}
