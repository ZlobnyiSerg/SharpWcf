using System;
using System.ServiceModel;
using SharpWcf.Configuration;

namespace SharpWcf
{
    public class ServiceFactory
    {
        private ServiceConfiguration _configuration;

        public ServiceFactory()
        {
            _configuration = new ServiceConfiguration();
        }

        public ServiceFactory LoadConfiguration(string configurationFileName, bool tryLoadLocalFile)
        {

            return this;
        }

        public T CreateHost<T>(Action<ServiceConfiguration> configurer) where T : ServiceHost, new()
        {
            var host = new T();
            var configuration = new ServiceConfiguration();
        }
    }
}