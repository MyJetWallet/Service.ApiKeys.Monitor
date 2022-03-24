using MyJetWallet.Sdk.Service;
using MyYamlParser;
using Telegram.Bot.Types;

namespace Service.ApiKeys.Monitor.Settings
{
    public class SettingsModel
    {
        [YamlProperty("ApiKeysMonitor.SeqServiceUrl")]
        public string SeqServiceUrl { get; set; }

        [YamlProperty("ApiKeysMonitor.ZipkinUrl")]
        public string ZipkinUrl { get; set; }

        [YamlProperty("ApiKeysMonitor.ElkLogs")]
        public LogElkSettings ElkLogs { get; set; }

        [YamlProperty("ApiKeysMonitor.BotApiKey")]
        public string BotApiKey { get; set; }

        [YamlProperty("ApiKeysMonitor.MyNoSqlReaderHostPort")]
        public string MyNoSqlReaderHostPort { get; set; }

        [YamlProperty("ApiKeysMonitor.TelegramChatId")]
        public string TelegramChatId { get; set; }

        [YamlProperty("ApiKeysMonitor.CheckPeriodInSeconds")]
        public int CheckPeriodInSeconds { get; set; }
    }
}
