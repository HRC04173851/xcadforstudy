// -*- coding: utf-8 -*-
// src/Toolkit/Utils/FaultObjectBuilder.cs
//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************
// 说明：
// 本文件实现故障对象工厂类 FaultObjectFactory。
// 为指定 xCAD 接口类型动态创建故障对象实例。
// 当调用任何方法时抛出不支持异常，用于占位或错误处理。
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Xarial.XCad.Exceptions;

namespace Xarial.XCad.Toolkit.Utils
{
    /// <summary>
    /// Creates fault object of the specified type
    /// <para>为指定接口类型创建故障对象（调用时抛出不支持异常）。</para>
    /// </summary>
    public class FaultObjectFactory
    {
        private readonly AssemblyBuilder m_AssmBuilder;
        private readonly ModuleBuilder m_ModuleBuilder;

        private readonly Dictionary<Type, Type> m_Cache;

        /// <summary>
        /// Initializes dynamic fault object factory.
        /// <para>初始化动态故障对象工厂。</para>
        /// </summary>
        public FaultObjectFactory() 
        {
            m_AssmBuilder = AssemblyBuilder.DefineDynamicAssembly(
                    new AssemblyName(Guid.NewGuid().ToString()), AssemblyBuilderAccess.Run);

            m_ModuleBuilder = m_AssmBuilder.DefineDynamicModule(Guid.NewGuid().ToString());

            m_Cache = new Dictionary<Type, Type>();
        }

        public T CreateFaultObject<T>()
            where T : IXObject 
            => (T)CreateFaultObject(typeof(T));

        /// <summary>
        /// Creates fault object for specified xCAD interface type.
        /// <para>为指定 xCAD 接口类型创建故障对象实例。</para>
        /// </summary>
        public IFaultObject CreateFaultObject(Type type)
        {
            if (!m_Cache.TryGetValue(type, out Type impType))
            {
                if (!type.IsInterface)
                {
                    throw new NotSupportedException($"Only interfaces are supported");
                }

                if (!typeof(IXObject).IsAssignableFrom(type))
                {
                    throw new NotSupportedException($"Only interfaces derived from {nameof(IXObject)} are supported");
                }

                var typeBuilder = m_ModuleBuilder.DefineType
                    (type.Name + "Fault", TypeAttributes.Class | TypeAttributes.Public);

                typeBuilder.AddInterfaceImplementation(type);
                typeBuilder.AddInterfaceImplementation(typeof(IFaultObject));

                ImplementAllMethods(type, typeBuilder, new List<Type>(), new List<MethodInfo>());

                impType = typeBuilder.CreateType();

                m_Cache.Add(type, impType);
            }

            return (IFaultObject)Activator.CreateInstance(impType);
        }

        private void ImplementAllMethods(Type type, TypeBuilder typeBuilder, List<Type> processedInterfaces, List<MethodInfo> processedMethods)
        {
            if (!processedInterfaces.Contains(type))
            {
                processedInterfaces.Add(type);

                foreach (var method in type.GetMethods())
                {
                    ImplementMethod(method, typeBuilder, processedMethods);
                }

                foreach (var subInterface in type.GetInterfaces())
                {
                    ImplementAllMethods(subInterface, typeBuilder, processedInterfaces, processedMethods);
                }
            }
        }

        private void ImplementMethod(MethodInfo methodInfo, TypeBuilder typeBuilder, List<MethodInfo> processedMethods)
        {
            if (!processedMethods.Contains(methodInfo))
            {
                var returnType = methodInfo.ReturnType;

                var paramTypes = new List<Type>();

                foreach (var parameterInfo in methodInfo.GetParameters())
                {
                    paramTypes.Add(parameterInfo.ParameterType);
                }

                var methodBuilder = typeBuilder.DefineMethod
                    (methodInfo.Name, MethodAttributes.Public |
                    MethodAttributes.Virtual, returnType, paramTypes.ToArray());

                var ilGenerator = methodBuilder.GetILGenerator();

                ilGenerator.ThrowException(typeof(FaultObjectNotSupportedException));

                typeBuilder.DefineMethodOverride(methodBuilder, methodInfo);

                processedMethods.Add(methodInfo);
            }
        }
    }
}
