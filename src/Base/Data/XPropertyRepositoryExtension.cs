// -*- coding: utf-8 -*-
// XPropertyRepositoryExtension.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 为 IXPropertyRepository 接口提供扩展方法，包括设置属性值和获取或预创建属性。
// 简化属性操作流程，自动处理属性创建和更新的逻辑。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Base;

namespace Xarial.XCad.Data
{
    /// <summary>
    /// Additional methods for <see cref="IXPropertyRepository"/>
    /// <see cref="IXPropertyRepository"/> 的扩展方法
    /// </summary>
    public static class XPropertyRepositoryExtension
    {
        /// <summary>
        /// Sets the value for this poperty
        /// 设置属性值
        /// </summary>
        /// <param name="prps">Repository</param>
        /// <param name="prpName">Name of the property</param>
        /// <param name="prpVal">Proeprty value</param>
        /// <remarks>This method will change the value of existing property or create new one if not exist</remarks>
        public static void Set(this IXPropertyRepository prps, string prpName, object prpVal)
        {
            var prp = prps.GetOrPreCreate(prpName);
            prp.Value = prpVal;
            if (!prp.Exists())
            {
                prps.Add(prp);
            }
        }

        /// <summary>
        /// Gets or pre creates property
        /// 获取属性或预创建属性
        /// </summary>
        /// <param name="prps">Repository</param>
        /// <param name="name">Name of the property</param>
        /// <returns>Existing proeprty or non-commited property</returns>
        public static IXProperty GetOrPreCreate(this IXPropertyRepository prps, string name) 
        {
            IXProperty prp;

            if (!prps.TryGet(name, out prp)) 
            {
                prp = prps.PreCreate();
                prp.Name = name;
            }

            return prp;
        }
    }
}
