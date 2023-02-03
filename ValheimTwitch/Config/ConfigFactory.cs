using UnityEngine;

namespace ValheimTwitch.Config
{
    public class ConfigFactory
    {
        private const string SECTION_NAME = "General";
        public static IConfigProvider GetProvider()
        {
            var modeConfigEntry = Plugin.Instance.Config.Bind(SECTION_NAME, "mode", Mode.Live, "Sets the mod in a live or a test mode.");
            var enableRewardsAutomaticallyConfigEntry = Plugin.Instance.Config.Bind(SECTION_NAME, "enableRewardsAutomatically", false, "Enable all Valheim rewards automatically when a game character enters a game world.");

            var toggleSpawnRewardsKeyConfigEntry = Plugin.Instance.Config.Bind(SECTION_NAME, "toggleSpawnRewardsKey", KeyCode.None, "A key for disabling / enabling any of the spawn rewards. Useful when the character is in a dungeon or near a spawning point like bed.");

            // bind the config entries
            var testConfig = new TestConfig
            {
                EnableRewardsAutomatically = enableRewardsAutomaticallyConfigEntry.Value,
                ToggleSpawnRewardsKey = toggleSpawnRewardsKeyConfigEntry.Value
            };

            var liveConfig = new LiveConfig
            {
                EnableRewardsAutomatically = enableRewardsAutomaticallyConfigEntry.Value,
                ToggleSpawnRewardsKey = toggleSpawnRewardsKeyConfigEntry.Value
            };

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
