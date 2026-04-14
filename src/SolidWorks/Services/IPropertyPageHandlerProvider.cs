// -*- coding: utf-8 -*-
// src/SolidWorks/Services/IPropertyPageHandlerProvider.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 定义属性页处理器提供程序接口，用于创建属性管理器页的处理器，支持自定义属性页的创建和COM可见性配置。
//*********************************************************************

using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.SolidWorks.UI.PropertyPage;

namespace Xarial.XCad.SolidWorks.Services
{
    /// <summary>
    /// Handler for the drag arrow when used with <see cref="Extensions.IXExtension.CreatePage{TData}(XCad.UI.PropertyPage.Delegates.CreateDynamicControlsDelegate)"/>
    /// </summary>
    public interface IPropertyPageHandlerProvider 
    {
        ///<summary> This function is called when new handler instance needs to be created</summary>
        ///<param name="handlerType">Type of the handler</param>
        /// <param name="app">Pointer to SOLIDWORKS application</param>
        /// <returns>Property page handler</returns>
        /// <remarks>The class must be com-visible. Provide new instance of the handler with each call</remarks>
        SwPropertyManagerPageHandler CreateHandler(ISwApplication app, Type handlerType);
    }

    internal class DataModelPropertyPageHandlerProvider : IPropertyPageHandlerProvider
    {
        public SwPropertyManagerPageHandler CreateHandler(ISwApplication app, Type handlerType)
        {
            if (handlerType.GetConstructor(Type.EmptyTypes) != null)
            {
                var handler = Activator.CreateInstance(handlerType);

                if (handler is SwPropertyManagerPageHandler)
                {
                    return (SwPropertyManagerPageHandler)handler;
                }
                else
                {
                    throw new InvalidCastException($"{handlerType.FullName} must be COM-visible and inherit {typeof(SwPropertyManagerPageHandler).FullName}");
                }
            }
            else 
            {
                throw new Exception($"{handlerType.FullName} must have a public parameterless constructor");
            }
        }
    }
}
