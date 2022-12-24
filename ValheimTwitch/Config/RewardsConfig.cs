using BepInEx.Configuration;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ValheimTwitch.Gui;
using ValheimTwitch.Twitch.API.Helix;

namespace ValheimTwitch.Config
{
    public class RewardsConfig
    {
        private string sectionName;
        private Dictionary<string, JToken> rewards = new Dictionary<string, JToken>();

        public RewardsConfig(string sectionName)
        {
            this.sectionName = sectionName;
        }

        public void Load()
        {
            var keys = GetConfigDefinitions();
            Dictionary<string, JToken> tokens = new Dictionary<string, JToken>(16); 
            foreach (var key in keys)
            {
                if (key.Section == sectionName)
                {
                    var entry = Plugin.Instance.Config.Bind(sectionName, key.Key, "");
                    if (entry.Value.Length > 0)
                    {
                        var value = JToken.Parse(entry.Value);
                        rewards[key.Key] = value;
                    }
                }
            }
        }

        public void Sync(List<Reward> twitchRewards)
        {
            foreach (var id in rewards.Keys)
            {
                var rewardExists = twitchRewards.Any(reward => reward.Id == id);
                if (!rewardExists)
                {
                    Delete(id, false);
                }
            }
            Save();
        }

        public static void Save()
        {
            Plugin.Instance.Config.Save();
        }

        public SettingsMessageData GetSettings(string key)
        {
            JToken data;
            if (rewards.TryGetValue(key, out data))
            {
                return Model.FromToken(data);
            }
            return null;
        }

        public void Set(string key, SettingsMessageData data, bool save = true)
        {
            rewards[key] = JToken.FromObject(data);

            var bind = Plugin.Instance.Config.Bind(sectionName, key, "");
            bind.Value = rewards[key].ToString(Newtonsoft.Json.Formatting.None);

            if (save)
                Save();
        }

        public void Delete(string key, bool save = true)
        {
            var configDefinition = new ConfigDefinition(sectionName, key);
            Plugin.Instance.Config.Remove(configDefinition);
            GetOrphanedEntries().Remove(configDefinition);

            if (save)
                Save();
        }

        private static List<ConfigDefinition> GetConfigDefinitions()
        {
            var keys = new List<ConfigDefinition>(GetOrphanedEntries().Keys);
            keys.AddRange(Plugin.Instance.Config.Keys);
            return keys;
        }

        private static Dictionary<ConfigDefinition, string> GetOrphanedEntries()
        {
            var entriesProperty = Plugin.Instance.Config.GetType().GetProperty("OrphanedEntries", BindingFlags.Instance | BindingFlags.NonPublic);
            return (Dictionary<ConfigDefinition, string>)entriesProperty.GetValue(Plugin.Instance.Config);
        }
    }
}
