using System;
using System.ServiceModel;
using SharpWcf.Demo.Contracts;

namespace SharpWcf.Demo.Services
{
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    public class ServiceA : IServiceA
    {
        public string Operation1()
        {
            return "Hello from service! "+DateTime.Now;
        }
    }
}