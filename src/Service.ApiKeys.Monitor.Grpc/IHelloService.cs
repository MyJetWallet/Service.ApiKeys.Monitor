using System.ServiceModel;
using System.Threading.Tasks;
using Service.ApiKeys.Monitor.Grpc.Models;

namespace Service.ApiKeys.Monitor.Grpc
{
    [ServiceContract]
    public interface IHelloService
    {
        [OperationContract]
        Task<HelloMessage> SayHelloAsync(HelloRequest request);
    }
}