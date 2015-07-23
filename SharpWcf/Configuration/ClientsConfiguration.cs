﻿using System;
using System.Configuration;
using System.Linq;
using System.ServiceModel.Configuration;

namespace SharpWcf.Configuration
{
    public class ClientsConfiguration
    {
        public ClientsConfiguration()
        {
            var appConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var serviceGroup = ServiceModelSectionGroup.GetSectionGroup(appConfig);
            if (serviceGroup != null && serviceGroup.Behaviors != null)
            {
                Behaviors = serviceGroup.Behaviors.EndpointBehaviors;
            }
        }

        public EndpointBehaviorElementCollection Behaviors { get; }
        public ClientConfiguration[] Clients { get; set; }

        /// <summary>
        ///     Load services configuration from JSON file
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static ClientsConfiguration LoadFromJson(string fileName)
        {
            return ConfigLoader.LoadConfig<ClientsConfiguration>(fileName);
        }

        public ClientConfiguration GetClientConfiguration(Type type)
        {
            var typeName = type.FullName;
            var baseConfig = Clients.FirstOrDefault(s => s.Types == null);
            var explicitConfig = Clients.FirstOrDefault(s => s.Types != null && s.Types.Contains(typeName));

            if (explicitConfig == null)
            {
                explicitConfig = baseConfig;
            }
            else if (baseConfig != null)
            {
                explicitConfig.Address = explicitConfig.Address ?? baseConfig.Address;
                explicitConfig.Behavior = explicitConfig.Behavior ?? baseConfig.Behavior;
                explicitConfig.Binding = explicitConfig.Binding ?? baseConfig.Binding;
                explicitConfig.BindingConfiguration = explicitConfig.BindingConfiguration ??
                                                      baseConfig.BindingConfiguration;
            }

            if (explicitConfig == null)
                throw new InvalidOperationException(string.Format(
                    "No configuration was found for client of type '{0}'", typeName));

            return explicitConfig;
        }
    }
}