//*********************************************************************
//xCAD
//Copyright(C) 2024 Xarial Pty Limited
//Product URL: https://www.xcad.net
//License: https://xcad.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.XCad.Features.CustomFeature;

namespace Xarial.XCad.Toolkit.CustomFeature
{
    /// <summary>
    /// Manages the cache of custom feature servers
    /// <para>管理自定义特征定义服务实例的缓存。</para>
    /// </summary>
    public static class CustomFeatureDefinitionInstanceCache
    {
        private static Dictionary<Type, IXCustomFeatureDefinition> m_Instances
            = new Dictionary<Type, IXCustomFeatureDefinition>();

        /// <summary>
        /// Registers instance of the custom feature server
        /// <para>注册自定义特征定义服务实例。</para>
        /// </summary>
        /// <param name="inst">Instance to register<para>要注册的实例。</para></param>
        public static void RegisterInstance(IXCustomFeatureDefinition inst)
        {
            var type = inst.GetType();

            if (!m_Instances.ContainsKey(type))
            {
                m_Instances.Add(type, inst);
            }
        }

        /// <summary>
        /// Returns the instance of custom feature server
        /// <para>返回自定义特征定义服务实例。</para>
        /// </summary>
        /// <param name="defType">Type of custom feature definition<para>自定义特征定义类型。</para></param>
        /// <returns>Instance of the custom feature server<para>自定义特征定义服务实例。</para></returns>
        public static IXCustomFeatureDefinition GetInstance(Type defType)
        {
            if (!typeof(IXCustomFeatureDefinition).IsAssignableFrom(defType)) 
            {
                throw new InvalidCastException($"{defType.FullName} must implement {typeof(IXCustomFeatureDefinition).FullName}");
            }

            IXCustomFeatureDefinition inst;

            if (!m_Instances.TryGetValue(defType, out inst))
            {
                //TODO: validate that default constructor is available

                inst = (IXCustomFeatureDefinition)Activator.CreateInstance(defType);
                RegisterInstance(inst);
            }

            return inst;
        }
    }
}
