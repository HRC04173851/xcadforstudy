// -*- coding: utf-8 -*-
// src/Toolkit/Reflection/ObjectExtension.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件提供内部对象转换辅助方法。
// 将装箱值转换为页面构建器或控件绑定所需的目标类型。
// 支持可转换类型和枚举类型的自动转换。
//*********************************************************************

using System;

namespace Xarial.XCad.Utils.Reflection
{
    /// <summary>
    /// Provides internal object conversion helper methods.
    /// <para>提供内部对象转换辅助方法。</para>
    /// </summary>
    internal static class ObjectExtension
    {
        /// <summary>
        /// Converts boxed value to target type used by page builder/control binding.
        /// <para>将装箱值转换为页面构建器或控件绑定所需的目标类型。</para>
        /// </summary>
        internal static object Cast(this object value, Type type)
        {
            object destVal = null;

            if (value != null)
            {
                if (!type.IsAssignableFrom(value.GetType()))
                {
                    try
                    {
                        if (typeof(IConvertible).IsAssignableFrom(type))
                        {
                            destVal = Convert.ChangeType(value, type);
                        }
                        else if (type.IsEnum) 
                        {
                            destVal = Enum.Parse(type, value?.ToString());
                        }
                    }
                    catch
                    {
                        throw new InvalidCastException(
                            $"Specified constructor for {type.Name} type is invalid as value cannot be cast from {value.GetType().Name}");
                    }
                }
                else
                {
                    //TODO: change this - validate that cast is possible otherwise throw exception
                    destVal = value;
                }
            }
            else 
            {
                if (type.IsValueType)
                {
                    return Activator.CreateInstance(type);
                }
            }

            return destVal;
        }
    }
}