using MyNoSqlServer.Abstractions;
using System.Runtime.Serialization;

namespace Service.ApiKeys.Monitor.Jobs
{
    public class ApiKeyRecordNoSql : MyNoSqlDbEntity
    {
        public const string TableName = "myjetwallet-adminpanel-apikey-record";

        public ApiKeyRecord ApiKey { get; set; }
        public static string GeneratePartitionKey() => "ApiKey";
        public static string GenerateRowKey(string name) => $"{name}";

        public static ApiKeyRecordNoSql Create(ApiKeyRecord apiKey)
        {
            return new ApiKeyRecordNoSql
            {
                PartitionKey = GeneratePartitionKey(),
                RowKey = GenerateRowKey(apiKey.ApplicationName),
                ApiKey = apiKey
            };
        }
    }

    [DataContract]
    public class ApiKeyRecord
    {
        [DataMember(Order = 1)] public string ApiKeyId { get; set; }
        [DataMember(Order = 2)] public string EncryptionKeyId { get; set; }
        [DataMember(Order = 3)] public string ApplicationName { get; set; }
        [DataMember(Order = 4)] public string ApplicationUri { get; set; }
    }
}
