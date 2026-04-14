// -*- coding: utf-8 -*-
// src/Base/Features/CustomFeature/Services/ICustomFeatureHandler.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义每个宏特征实例处理器的接口，负责初始化和卸载时的资源管理。
// 在宏特征创建或首次加载时调用Init，删除或模型关闭时调用Unload。
//*********************************************************************

using Xarial.XCad.Documents;
using Xarial.XCad.Features.CustomFeature.Enums;

namespace Xarial.XCad.Features.CustomFeature.Services
{
    //TODO: implement

    /// <summary>
    /// Handler of each macro feature
    /// 每个宏特征实例对应的处理器接口
    /// </summary>
    /// <remarks>The instance of the specific class will be created for each macro feature</remarks>
    public interface ICustomFeatureHandler
    {
        /// <summary>
        /// Called when macro feature is created or loaded first time
        /// 当宏特征首次创建或首次加载时调用
        /// </summary>
        /// <param name="app">Pointer to application</param>
        /// <param name="model">Pointer to model</param>
        /// <param name="feat">Pointer to feature</param>
        void Init(IXApplication app, IXDocument model, IXFeature feat);

        /// <summary>
        /// Called when macro feature is deleted or model is closed
        /// 当宏特征删除或模型关闭时调用
        /// </summary>
        /// <param name="reason">Reason of macro feature unload</param>
        void Unload(CustomFeatureUnloadReason_e reason);
    }
}