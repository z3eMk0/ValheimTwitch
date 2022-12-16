using BepInEx.Configuration;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Reflection;
using ValheimTwitch.Gui;
using ValheimTwitch.Twitch.API.Helix;

namespace ValheimTwitch
{
    static class RewardsConfig
    {
        private const string REWARDS_SECTION = "rewards";
        private static Dictionary<string, JToken> rewards = new Dictionary<string, JToken>();

        public static void Load()
        {
            Log.Info("RewardsConfig.Load()");
            var entriesProperty = Plugin.Instance.Config.GetType().GetProperty("OrphanedEntries", BindingFlags.Instance | BindingFlags.NonPublic);
            var entries = (Dictionary<ConfigDefinition, string>)entriesProperty.GetValue(Plugin.Instance.Config);
            var keys = new List<ConfigDefinition>(entries.Keys);
            keys.AddRange(Plugin.Instance.Config.Keys);
            foreach (var key in keys)
            {
                Log.Info($"Config entry iterated {key.Key}");
                if (key.Section == REWARDS_SECTION)
                {
                    var entry = Plugin.Instance.Config.Bind(REWARDS_SECTION, key.Key, "");
                    if (entry.Value.Length > 0)
                    {
                        Log.Info($"Config entry loading {key.Key} {entry.Value}");
                        var value = JToken.Parse(entry.Value);
                        rewards[key.Key] = value;
                        Log.Info($"Config entry loaded {key.Key} {value}");
                    }
                }
            }
        }

        public static void Sync(List<Reward> twitchRewards)
        {
            foreach (var twichReward in twitchRewards)
            {
                if (!rewards.ContainsKey(twichReward.Id))
                {
                    Delete(twichReward.Id, false);
                }
            }
            Save();
        }

        public static void Save()
        {
            foreach (var entry in rewards)
            {
                //ConfigEntry<string> configEntry;
                //if (!Plugin.Instance.Config.TryGetEntry(REWARDS_SECTION, entry.Key, out configEntry))
                //{
                //    Plugin.Instance.Config.AddSetting(REWARDS_SECTION, entry.Key, "");
                //}
                //Plugin.Instance.Config["rewards", entry.Key].BoxedValue = entry.Value.ToString(Newtonsoft.Json.Formatting.Indented);
                
                //var bind = Plugin.Instance.Config.Bind(
                //    REWARDS_SECTION, entry.Key, JToken.FromObject(new SettingsMessageData()).ToString(Newtonsoft.Json.Formatting.None));
                //bind.Value = entry.Value.ToString(Newtonsoft.Json.Formatting.None);
            }
            Plugin.Instance.Config.Save();
            //PluginConfig.SetObject("rewards", rewards);
        }

        public static JToken Get(string key)
        {
            JToken data;
            rewards.TryGetValue(key, out data);
            return data;
        }

        public static SettingsMessageData GetSettings(string key)
        {
            JToken data;
            if (rewards.TryGetValue(key, out data))
            {
                return Model.FromToken(data);
            }
            return null;
        }

        public static void Set(string key, SettingsMessageData data, bool save = true)
        {
            rewards[key] = JToken.FromObject(data);

            var bind = Plugin.Instance.Config.Bind(
                    REWARDS_SECTION, key, JToken.FromObject(new SettingsMessageData()).ToString(Newtonsoft.Json.Formatting.None));
            bind.Value = rewards[key].ToString(Newtonsoft.Json.Formatting.None);

            if (save)
                Save();
        }

        public static void Delete(string key, bool save = true)
        {
            Plugin.Instance.Config.Remove(new ConfigDefinition(REWARDS_SECTION, key));

            if (save)
                Save();
        }
    }
}
