using Autofac;
using MyJetWallet.Sdk.NoSql;
using Service.ApiKeys.Monitor.Jobs;
using Telegram.Bot;

namespace Service.ApiKeys.Monitor.Modules
{
    public class ServiceModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var telegramBot = new TelegramBotClient(Program.Settings.BotApiKey);
            var myNoSqlClient = builder.CreateNoSqlClient(() => Program.Settings.MyNoSqlReaderHostPort);

            builder.RegisterInstance(telegramBot)
                .As<ITelegramBotClient>()
                .SingleInstance();

            builder.RegisterMyNoSqlReader<ApiKeyRecordNoSql>(myNoSqlClient, ApiKeyRecordNoSql.TableName);

            builder
               .RegisterType<MonitoringJob>()
               .AsSelf()
               .AutoActivate()
               .SingleInstance();
        }
    }
}