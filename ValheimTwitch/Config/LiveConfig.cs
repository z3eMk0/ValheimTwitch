using Newtonsoft.Json.Linq;
using ValheimTwitch.Twitch.Auth;

namespace ValheimTwitch.Config
{
    class LiveConfig : IConfigProvider
    {
        private const string HELIX_URL = "https://api.twitch.tv/helix";
        // the client of Skarab42
        private const string TWITCH_APP_CLIENT_ID = "5b9v1vm0jv7kx9afpmz0ylb3lp7k9w";
        private readonly RewardsConfig rewards;

        public RewardsConfig Rewards
        {
            get
            {
                return rewards;
            }
        }

        public string HelixUrl
        {
            get => HELIX_URL;
        }

        public Credentials Credentials
        {
            get;
            private set;
        }
        
        public bool EnableRewardsAutomatically
        {
            get;
            set;
        }
        
        public LiveConfig()
        {
            var token = PluginConfig.GetObject("twitchAuthToken");
            if (token != null)
            {
                JToken accessToken;
                JToken refreshToken;

                token.TryGetValue("accessToken", out accessToken);
                token.TryGetValue("refreshToken", out refreshToken);
                Credentials = new Credentials(TWITCH_APP_CLIENT_ID, accessToken.Value<string>(), refreshToken.Value<string>());
            }
            else
            {
                Credentials = new Credentials(TWITCH_APP_CLIENT_ID, string.Empty, string.Empty);
            }

            rewards = new RewardsConfig("Rewards");
            rewards.Load();
        }
    }
}
