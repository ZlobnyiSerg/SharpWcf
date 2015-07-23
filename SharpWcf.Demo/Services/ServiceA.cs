using System;
using SharpWcf.Demo.Contracts;

namespace SharpWcf.Demo.Services
{
    public class ServiceA : IServiceA
    {
        public string Operation1()
        {
            return "Hello from service! "+DateTime.Now;
        }
    }
}