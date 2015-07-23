using System.ServiceModel;

namespace SharpWcf.Demo.Contracts
{
    [ServiceContract]
    public interface IServiceA
    {
        [OperationContract]
        string Operation1();
    }
}