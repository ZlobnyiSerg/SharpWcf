using System.ServiceModel;

namespace SharpWcf.Demo.Contracts
{
    [ServiceContract]
    public interface IServiceA
    {
        [OperationContract]
        void Operation1();
    }
}