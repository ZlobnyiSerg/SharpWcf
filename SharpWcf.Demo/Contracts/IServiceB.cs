using System.ServiceModel;

namespace SharpWcf.Demo.Contracts
{
    [ServiceContract]
    public interface IServiceB
    {
        [OperationContract]
        void Operation2();
    }
}