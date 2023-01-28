namespace ValheimTwitch.Config
{
    public class ConfigFactory
    {
        private const string SECTION_NAME = "General";
        public static IConfigProvider GetProvider()
        {
            var modeConfigEntry = Plugin.Instance.Config.Bind(SECTION_NAME, "mode", Mode.Live, "Sets the mod in a live or a test mode.");
            var enableRewardsAutomaticallyConfigEntry = Plugin.Instance.Config.Bind(SECTION_NAME, "enableRewardsAutomatically", false, "Enable all Valheim rewards automatically when a game character enters a game world.");
            // bind the config entries
            var testConfig = new TestConfig();
            testConfig.EnableRewardsAutomatically = enableRewardsAutomaticallyConfigEntry.Value;
            var liveConfig = new LiveConfig();
            liveConfig.EnableRewardsAutomatically = enableRewardsAutomaticallyConfigEntry.Value;
            
            if (modeConfigEntry.Value == Mode.Test)
            {
                return testConfig;
            }
            return liveConfig;
        }

        public enum Mode
        {
            Live,
            Test
        }
    }
}
