using System;
using SharpWcf.Configuration;
using SharpWcf.Demo.Services;

namespace SharpWcf.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            var r = new ServiceFactory(ServicesConfiguration.LoadFromJson("services.config.json"));
            var host1 = r.CreateHost<ServiceA>();
            host1.Open();

            var host2 = r.CreateHost<ServiceB>();
            host2.Open();

            Console.WriteLine("Services are created and listening. Press ENTER to terminate...");
            Console.ReadLine();
            host1.Close();
            host2.Close();
        }
    }
}
