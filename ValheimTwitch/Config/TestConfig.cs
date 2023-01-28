using UnityEngine;
using ValheimTwitch.Twitch.Auth;

namespace ValheimTwitch.Config
{
    class TestConfig : IConfigProvider
    {
        private const string SECTION_NAME = "Test";
        private readonly RewardsConfig rewards;

        public RewardsConfig Rewards
        {
            get
            {
                return rewards;
            }
        }

        public string HelixUrl { get; private set; }

        public Credentials Credentials { get; private set; }

        public bool EnableRewardsAutomatically { get; set; }

        public KeyCode IgnoreRewardsKey { get; set; }

        public TestConfig()
        {
            var clientIdConfigEntry = Plugin.Instance.Config.Bind(SECTION_NAME, "clientId", "", "Id of the Twitch mock client.");
            var clientId = clientIdConfigEntry.Value;
            var accessTokenConfigEntry = Plugin.Instance.Config.Bind(SECTION_NAME, "accessToken", "", "Access token of the Twitch mock user.");
            var accessToken = accessTokenConfigEntry.Value;
            // the mock-api does not provide refresh tokens
            var refreshToken = string.Empty;
            Credentials = new Credentials(clientId, accessToken, refreshToken);

            var helixUrlConfigEntry = Plugin.Instance.Config.Bind(SECTION_NAME, "mockServerUrl", "", "URL of the Twitch mock server.");
            HelixUrl = helixUrlConfigEntry.Value;

            rewards = new RewardsConfig("TestRewards");
            rewards.Load();
        }
    }
}
