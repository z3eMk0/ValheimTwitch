namespace ValheimTwitch.Config
{
    public class ConfigFactory
    {
        public static IConfigProvider GetProvider()
        {
            var modeConfigEntry = Plugin.Instance.Config.Bind("General", "mode", Mode.Live, "Sets the mod in a live or a test mode.");
            // bind the config entries
            var testConfig = new TestConfig();
            var liveConfig = new LiveConfig();
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
