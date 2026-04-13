//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.XCad
{
    /// <summary>
    /// Represents the environment which can contain objects in different states (e.g. component, document, drawing view)
    /// 表示可包含不同状态对象的环境（如组件、文档、工程图视图）
    /// </summary>
    public interface IXObjectContainer
    {
        /// <summary>
        /// Converts pointer to object to the current environment container
        /// 将对象指针转换到当前环境容器
        /// </summary>
        /// <typeparam name="TSelObject">Type of object 对象类型</typeparam>
        /// <param name="obj">Pointer to object to convert 要转换的对象指针</param>
        /// <returns>Converted pointer 转换后的指针</returns>
        TSelObject ConvertObject<TSelObject>(TSelObject obj)
            where TSelObject : class, IXSelObject;
    }
}
