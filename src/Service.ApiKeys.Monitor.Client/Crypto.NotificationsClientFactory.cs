using JetBrains.Annotations;
using MyJetWallet.Sdk.Grpc;
using Service.ApiKeys.Monitor.Grpc;

namespace Service.ApiKeys.Monitor.Client
{
    [UsedImplicitly]
    public class CryptoNotificationsClientFactory: MyGrpcClientFactory
    {
        public CryptoNotificationsClientFactory(string grpcServiceUrl) : base(grpcServiceUrl)
        {
        }

        public IHelloService GetHelloService() => CreateGrpcService<IHelloService>();
    }
}
