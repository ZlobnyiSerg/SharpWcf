using System;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using Common.Logging;
using SharpWcf.Configuration;

namespace SharpWcf
{
    public class ClientFactory : WcfFactory
    {
        protected static readonly ILog Log = LogManager.GetLogger<ClientFactory>();

        private readonly ClientsConfiguration _configuration;

        public ClientFactory(ClientsConfiguration configuration)
        {
            _configuration = configuration;
        }

        public ServiceEndpoint CreateEndpoint<TContract>() where TContract : class
        {
            var config = _configuration.GetClientConfiguration(typeof (TContract));
            var contractDescription = ContractDescription.GetContract(typeof(TContract));

            var endpoint = new ServiceEndpoint(contractDescription);
            ApplyBehavior(endpoint, config.Behavior);
            endpoint.Binding = CreateBindingObjectByName(config.Binding, config.BindingConfiguration);
            var addr = config.Address;
            if (addr.Contains("*"))
            {
                var iface = TrimInterfaceName(GetImplementedInterface(typeof (TContract)).Name);                
                addr = addr.Replace("*", iface);
            }
            if (!string.IsNullOrEmpty(config.DnsIdentity))
            {
                endpoint.Address = new EndpointAddress(new Uri(addr, UriKind.RelativeOrAbsolute), new DnsEndpointIdentity(config.DnsIdentity));
            }
            else
            {
                endpoint.Address = new EndpointAddress(new Uri(addr, UriKind.RelativeOrAbsolute));
            }
            Log.TraceFormat("Configuring service: {0} with address: {1}; binding {2}; binding config: {3}; behavior: {4} ", typeof(TContract).Name, endpoint.Address, config.Binding, config.BindingConfiguration, config.Behavior);
            return endpoint;            
        }        

        public TContract CreateClient<TContract>() where TContract : class
        {
            return new ChannelFactory<TContract>(CreateEndpoint<TContract>()).CreateChannel();
        }

        private void ApplyBehavior(ServiceEndpoint host, string behavior)
        {
            foreach (EndpointBehaviorElement sbe in _configuration.Behaviors)
            {
                if (sbe.Name == behavior)
                {
                    foreach (var bxe in sbe)
                    {
                        var createBeh = typeof (BehaviorExtensionElement).GetMethod("CreateBehavior",
                            BindingFlags.Instance | BindingFlags.NonPublic);
                        var b = (IEndpointBehavior) createBeh.Invoke(bxe, new object[0]);
                        if (!host.Behaviors.Contains(bxe.BehaviorType))
                        {
                            host.Behaviors.Add(b);
                        }
                    }
                    return;
                }
            }
            throw new ApplicationException("Unable to find behavior with name " + behavior);
        }
    }
}