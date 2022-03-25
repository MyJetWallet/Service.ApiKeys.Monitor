using Autofac;
using Microsoft.Extensions.Logging;
using MyJetWallet.ApiSecurityManager.Autofac;
using MyJetWallet.Sdk.Service;
using MyJetWallet.Sdk.Service.Tools;
using MyNoSqlServer.Abstractions;
using Polly;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;

namespace Service.ApiKeys.Monitor.Jobs
{
    public class MonitoringJob : IStartable, IDisposable
    {
        private readonly ILogger<MonitoringJob> _logger;
        private readonly MyTaskTimer _timer;
        private readonly AsyncRetryPolicy _retryPolicy;
        private readonly IMyNoSqlServerDataReader<ApiKeyRecordNoSql> _reader;
        private readonly ITelegramBotClient _telegramBotClient;

        public MonitoringJob(
            ILogger<MonitoringJob> logger,
            IMyNoSqlServerDataReader<ApiKeyRecordNoSql> reader,
            ITelegramBotClient telegramBotClient)
        {
            _logger = logger;
            _reader = reader;
            _telegramBotClient = telegramBotClient;
            _timer = new MyTaskTimer(typeof(MonitoringJob), 
                TimeSpan.FromSeconds(Program.Settings.CheckPeriodInSeconds), 
                logger, 
                DoProcess);
            _retryPolicy = Policy
                          .Handle<Exception>()
                          .WaitAndRetryAsync(3, (i) => TimeSpan.FromMilliseconds(100 * (int)Math.Pow(2, i)));
        }

        private async Task DoProcess()
        {
            _logger.LogInformation("Start monitoring for all api keys");
            try
            {
                var all = _reader.Get();

                foreach (var item in all)
                {
                    _logger.LogInformation("Checking for: {item}", item.ToJson());

                    var factory = new ApiSecurityManagerClientFactory(item.ApiKey.ApplicationUri);
                    var apiKeyClient = factory.GetApiKeyService();
                    var encryptionKeyGrpcService = factory.GetEncryptionKeyGrpcService();
                    var isApiKeySet = false;
                    var isEncryptionKeySet = false;

                    await _retryPolicy.ExecuteAsync(async () =>
                    {
                        var apiKeys = await apiKeyClient.GetApiKeyIdsAsync(new MyJetWallet.ApiSecurityManager.Grpc.Models.GetApiKeyIdsRequest());

                        isApiKeySet = apiKeys?.Ids.Any(x => x == item.ApiKey.ApiKeyId) ?? false;
                    });

                    await _retryPolicy.ExecuteAsync(async () =>
                    {
                        var encryptionKeys = await encryptionKeyGrpcService.GetEncryptionKeyIdsAsync(new MyJetWallet.ApiSecurityManager.Grpc.Models.GetEncryptionKeyIdsRequest());

                        isEncryptionKeySet = encryptionKeys?.Ids?.Any(x => x == item.ApiKey.EncryptionKeyId) ?? false;
                    });

                    _logger.LogInformation("Checking for: {item}, isApiKeySet: {isApiKeySet}, isEncryptionKeySet: {isEncryptionKeySet}", 
                        item.ToJson(), isApiKeySet, isEncryptionKeySet);

                    if (isApiKeySet && !isEncryptionKeySet)
                        await _telegramBotClient
                            .SendTextMessageAsync(Program.Settings.TelegramChatId,
                            $"ENCRYPTION KEY {item.ApiKey.EncryptionKeyId} FOR APIKEY {item.ApiKey.ApiKeyId} IS NOT SET FOR {item.ApiKey.ApplicationName}, TEAM COME ON! AdminPanel, Security!");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "When MonitoringJob this error happened");
                throw;
            }

            _logger.LogInformation("Monitoring has been completed api keys");
        }

        public void Start()
        {
            _timer.Start();
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
