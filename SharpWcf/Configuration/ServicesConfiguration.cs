using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel.Configuration;
using System.Web;

namespace SharpWcf.Configuration
{
    /// <summary>
    ///     ServiceHost configuration description
    /// </summary>
    public class ServicesConfiguration
    {
        private readonly BehaviorsSection _behaviors;
        public BehaviorsSection Behaviors
        {
            get { return _behaviors; }
        }

        public ServicesConfiguration()
        {
            System.Configuration.Configuration appConfig;
            if (HttpContext.Current != null)
                appConfig = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
            else
                appConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var serviceGroup = ServiceModelSectionGroup.GetSectionGroup(appConfig);
            if (serviceGroup != null)
            {
                _behaviors = serviceGroup.Behaviors;
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
            return ConfigLoader.LoadConfig<ServicesConfiguration>(fileName);
        }

        public ServiceConfiguration GetServiceConfiguration(Type type)
        {
            Trace.WriteLine($"Resolving configuration for '{type}'");
            var typeName = type.FullName;
            var baseConfig = Services.FirstOrDefault(s => s.Types == null);
            var explicitConfig = Services.FirstOrDefault(s => s.Types != null && s.Types.Contains(typeName));

            if (explicitConfig == null)
            {
                explicitConfig = baseConfig;
                Trace.WriteLine("No explicit configuration found for this type of service, using base config");
            }
            else if (baseConfig != null)
            {
                explicitConfig.BaseAddresses = explicitConfig.BaseAddresses ?? baseConfig.BaseAddresses;
                explicitConfig.Behavior = explicitConfig.Behavior ?? baseConfig.Behavior;
                explicitConfig.Endpoints = explicitConfig.Endpoints ?? baseConfig.Endpoints;
            }

            if (explicitConfig == null)
                throw new InvalidOperationException(string.Format(
                    "No configuration was found for service of type '{0}'", typeName));

            Trace.WriteLine("Base addresses: " + string.Join(",", explicitConfig.BaseAddresses));
            Trace.WriteLine("Behavior: " + explicitConfig.Behavior);
            Trace.WriteLine("Endpoints: " + string.Join(",", explicitConfig.Endpoints.Select(e=>e.Address)));

            return explicitConfig;
        }
    }
}