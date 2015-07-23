using System;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using SharpWcf.Configuration;

namespace SharpWcf
{
    public class ClientFactory : WcfFactory
    {
        private readonly ClientsConfiguration _configuration;

        public ClientFactory(ClientsConfiguration configuration)
        {
            _configuration = configuration;
        }

        public TContract CreateChannel<TContract>() where TContract : class
        {
            var config = _configuration.GetClientConfiguration(typeof (TContract));
            var contractDescription = ContractDescription.GetContract(typeof(TContract));

            var endpoint = new ServiceEndpoint(contractDescription);
            ApplyBehavior(endpoint, config.Behavior);
            endpoint.Binding = CreateBindingObjectByName(config.Binding, config.BindingConfiguration);
            var addr = config.Address;
            if (addr.EndsWith("*"))
            {
                var iface = GetImplementedInterface(typeof (TContract)).Name.TrimStart('I');
                addr = addr.TrimEnd('*') + iface;
            }
            if (!string.IsNullOrEmpty(config.DnsIdentity))
            {
                endpoint.Address = new EndpointAddress(new Uri(addr, UriKind.RelativeOrAbsolute), new DnsEndpointIdentity(config.DnsIdentity));
            }
            else
            {
                endpoint.Address = new EndpointAddress(new Uri(addr, UriKind.RelativeOrAbsolute));
            }

            var factory = new ChannelFactory<TContract>(endpoint);
            
            return factory.CreateChannel();
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