//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using Xarial.XCad.Utils.PageBuilder.Base.Attributes;

namespace Xarial.XCad.Utils.PageBuilder.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    /// <summary>
    /// Declares constructor as default for specified data type.
    /// <para>将构造器声明为指定数据类型的默认构造器。</para>
    /// </summary>
    public class DefaultTypeAttribute : Attribute, IDefaultTypeAttribute
    {
        /// <summary>
        /// Gets associated data type.
        /// <para>获取关联的数据类型。</para>
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// Initializes attribute with target data type.
        /// <para>使用目标数据类型初始化该特性。</para>
        /// </summary>
        public DefaultTypeAttribute(Type type)
        {
            Type = type;
        }
    }
}