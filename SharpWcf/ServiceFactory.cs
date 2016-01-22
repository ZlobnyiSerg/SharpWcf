using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using Common.Logging;
using SharpWcf.Configuration;

namespace SharpWcf
{
    public class ServiceFactory : WcfFactory
    {
        protected static readonly ILog Log = LogManager.GetLogger<ServicesConfiguration>();
        private readonly ServicesConfiguration _configuration;

        public ServiceFactory(ServicesConfiguration configuration)
        {
            _configuration = configuration;
        }        

        protected virtual ServiceHost HostConstructor(Type type, ServiceConfiguration config)
        {
            return new ServiceHost(type, GetServiceBaseAddresses(type, config).ToArray());
        }

        public ServiceHost CreateHost<TService>()
        {
            return CreateHost<ServiceHost, TService>();
        }

        public THost CreateHost<THost, TService>() where THost : ServiceHost
        {
            var config = _configuration.GetServiceConfiguration(typeof(TService));

            var host = (THost)HostConstructor(typeof (TService), config);

            Configure<THost, TService>(host, config);

            return host;
        }

        protected virtual void Configure<T, TService>(T host, ServiceConfiguration config) where T : ServiceHost
        {
            var implementedInterface = GetImplementedInterface(typeof (TService));
            Log.TraceFormat("Service '{0}' endpoints:", implementedInterface.Name);
            foreach (var endpoint in config.Endpoints)
            {
                var binding = CreateBindingObjectByName(endpoint.Binding, endpoint.BindingConfiguration);

                Uri address;
                if (endpoint.Address.Contains("*"))
                {
                    address = new Uri(endpoint.Address.Replace("*", TrimInterfaceName(implementedInterface.Name)),
                        UriKind.RelativeOrAbsolute);
                }
                else
                {
                    address = new Uri(endpoint.Address, UriKind.RelativeOrAbsolute);
                }

                ServiceEndpoint addedEndpoint;
                if (string.IsNullOrEmpty(endpoint.Contract))
                {
                    addedEndpoint = host.AddServiceEndpoint(implementedInterface, binding, address);
                }
                else
                {
                    addedEndpoint = host.AddServiceEndpoint(endpoint.Contract, binding, address);
                }

                Log.TraceFormat("\t{0}\n\t\t\tbehavior: {1};\n\t\t\tbinding: {2};\n\t\t\tbinding config: {3}", addedEndpoint.Address, config.Behavior, endpoint.Binding, endpoint.BindingConfiguration);

                if (!string.IsNullOrEmpty(endpoint.BehaviorConfiguration))
                {
                    ApplyEndpointBehavior(addedEndpoint, endpoint.BehaviorConfiguration);                    
                }
                
            }

            if (!string.IsNullOrEmpty(config.Behavior))
            {
                ApplyServiceBehavior(host, config.Behavior);
            }
        }

        private void ApplyEndpointBehavior(ServiceEndpoint addedEndpoint, string behavior)
        {
            if (_configuration.Behaviors.EndpointBehaviors == null)
                return;

            foreach (EndpointBehaviorElement endpointBehavior in _configuration.Behaviors.EndpointBehaviors)
            {
                if (endpointBehavior.Name == behavior)
                {
                    foreach (var bxe in endpointBehavior)
                    {
                        var createBeh = typeof(BehaviorExtensionElement).GetMethod("CreateBehavior",
                                BindingFlags.Instance | BindingFlags.NonPublic);
                        var b = (IEndpointBehavior)createBeh.Invoke(bxe, new object[0]);
                        if (!addedEndpoint.Behaviors.Contains(bxe.BehaviorType))
                        {
                            addedEndpoint.Behaviors.Add(b);
                        }
                    }
                    return;
                }
            }
        }
        
        private void ApplyServiceBehavior(ServiceHost host, string behavior)
        {
            if (_configuration.Behaviors.ServiceBehaviors != null)
            {
                foreach (ServiceBehaviorElement sbe in _configuration.Behaviors.ServiceBehaviors)
                {
                    if (sbe.Name == behavior)
                    {
                        foreach (var bxe in sbe)
                        {
                            var createBeh = typeof (BehaviorExtensionElement).GetMethod("CreateBehavior",
                                BindingFlags.Instance | BindingFlags.NonPublic);
                            var b = (IServiceBehavior) createBeh.Invoke(bxe, new object[0]);
                            if (!host.Description.Behaviors.Contains(bxe.BehaviorType))
                            {
                                host.Description.Behaviors.Add(b);
                            }
                        }
                        return;
                    }
                }
                throw new ApplicationException("Unable to find behavior with name " + behavior);
            }
        }

        protected IEnumerable<Uri> GetServiceBaseAddresses(Type serviceType, ServiceConfiguration config)
        {            
            if (config != null && config.BaseAddresses != null)
            {
                var @interface = TrimInterfaceName(GetImplementedInterface(serviceType).Name);

                return config.BaseAddresses.Select(ba =>
                {
                    if (ba.Contains("*"))
                        ba = ba.Replace("*", @interface);
                    return new Uri(ba, UriKind.RelativeOrAbsolute);
                }
                    );
            }
            return null;
        }        
    }
}