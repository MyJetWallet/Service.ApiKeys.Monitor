using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.NoSql;
using MyJetWallet.Sdk.Service;
using MyJetWallet.Sdk.ServiceBus;
using Service.ApiKeys.Monitor.Jobs;

namespace Service.ApiKeys.Monitor
{
    public class ApplicationLifetimeManager : ApplicationLifetimeManagerBase
    {
        private readonly ILogger<ApplicationLifetimeManager> _logger;
        private readonly MyNoSqlClientLifeTime _noSqlTcpClient;
        private readonly MonitoringJob _monitoringJob;

        public ApplicationLifetimeManager(IHostApplicationLifetime appLifetime,
            ILogger<ApplicationLifetimeManager> logger,
            MyNoSqlClientLifeTime noSqlTcpClient,
            MonitoringJob monitoringJob) : base(
            appLifetime)
        {
            _logger = logger;
            _noSqlTcpClient = noSqlTcpClient;
            _monitoringJob = monitoringJob;
        }

        protected override void OnStarted()
        {
            _logger.LogInformation("OnStarted has been called");
            _noSqlTcpClient.Start();
            _monitoringJob.Start();
        }

        protected override void OnStopping()
        {
            _logger.LogInformation("OnStopping has been called");
            _noSqlTcpClient.Stop();
            _monitoringJob.Dispose();
        }

        protected override void OnStopped()
        {
            _logger.LogInformation("OnStopped has been called");
        }
    }
}
