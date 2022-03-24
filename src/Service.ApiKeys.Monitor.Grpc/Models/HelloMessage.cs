using System.Runtime.Serialization;
using Service.ApiKeys.Monitor.Domain.Models;

namespace Service.ApiKeys.Monitor.Grpc.Models
{
    [DataContract]
    public class HelloMessage : IHelloMessage
    {
        [DataMember(Order = 1)]
        public string Message { get; set; }
    }
}