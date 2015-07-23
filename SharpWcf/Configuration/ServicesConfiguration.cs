﻿using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.ServiceModel.Configuration;
using Newtonsoft.Json;

namespace SharpWcf.Configuration
{
    /// <summary>
    ///     ServiceHost configuration description
    /// </summary>
    public class ServicesConfiguration
    {
        private readonly ServiceBehaviorElementCollection _behaviors;
        public ServiceBehaviorElementCollection Behaviors
        {
            get { return _behaviors; }
        }

        public ServicesConfiguration()
        {
            var appConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var serviceGroup = ServiceModelSectionGroup.GetSectionGroup(appConfig);
            if (serviceGroup != null && serviceGroup.Behaviors != null)
            {
                _behaviors = serviceGroup.Behaviors.ServiceBehaviors;
            }
        }

        public ServiceConfiguration[] Services { get; set; }

        /// <summary>
        /// Load services configuration from JSON file
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static ServicesConfiguration LoadFromJson(string fileName)
        {
            var localFileName = fileName + ".local";

            if (File.Exists(localFileName))
            {                
                fileName = localFileName;
            }

            if (!File.Exists(fileName))
                throw new ApplicationException("Unable to find services configuration: " + fileName);

            var config = JsonConvert.DeserializeObject<ServicesConfiguration>(File.ReadAllText(fileName));

            return config;
        }

        public ServiceConfiguration GetServiceConfiguration(Type type)
        {
            var typeName = type.FullName;
            var baseConfig = Services.FirstOrDefault(s => s.Types == null);
            var explicitConfig = Services.FirstOrDefault(s => s.Types != null && s.Types.Contains(typeName));

            if (explicitConfig == null)
            {
                explicitConfig = baseConfig;
            }
            else if (baseConfig != null)
            {
                explicitConfig.BaseAddresses = explicitConfig.BaseAddresses ?? baseConfig.BaseAddresses;
                explicitConfig.Behavior = explicitConfig.Behavior ?? baseConfig.Behavior;
            }

            if (explicitConfig == null)
                throw new InvalidOperationException(string.Format(
                    "No configuration was found for service of type '{0}'", typeName));

            return explicitConfig;
        }
    }
}