using System;

namespace Discord.Core.Utils
{
    public class DiscordClientConfig
    {
        public string ApplicationId { get; set; }
        public string PublicKey { get; set; }
        public string BotToken { get; set; }
        public string ClientId { get; set; }
        public string ApiBaseUrl { get; set; }
        public TimeZoneInfo TargetTimezone { get; set; }
    }
}
