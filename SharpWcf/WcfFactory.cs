using System;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace SharpWcf
{
    public class WcfFactory
    {
        protected Binding CreateBindingObjectByName(string bindingType, string bindingConfigName)
        {
            var type = Type.GetType(bindingType, true, true);

            if (type == null)
                throw new InvalidOperationException("Unable to instantiate type: " + bindingType);

            return (Binding)Activator.CreateInstance(type, bindingConfigName);
        }

        protected virtual Type GetImplementedInterface(Type type)
        {
            if (type.IsInterface &&
                type.GetCustomAttributes(typeof (ServiceContractAttribute), false).Any())
                return type;

            var implementedInterface =
                type.GetInterfaces()
                    .FirstOrDefault(i => i.GetCustomAttributes(typeof(ServiceContractAttribute), true).Any());
            if (implementedInterface == null)
                throw new ArgumentException(string.Format("Service {0} does not implements any interface marked with ServiceContract", type));
            return implementedInterface;
        }
    }
}