using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace ValheimTwitch.Gui
{
    public static class Model
    {
        public static SettingsMessageData FromToken(JToken token)
        {
            if (token != null && token["action"] != null)
            {
                var data = token.ToObject<SettingsMessageData>();
                switch (data.Action)
                {
                    case 0:
                        break;
                    case 1:
                        data = token.ToObject<RavenMessageData>();
                        break;
                    case 2:
                        data = token.ToObject<SpawnCreatureData>();
                        break;
                    case 3:
                        data = token.ToObject<HUDMessageData>();
                        break;
                    case 4:
                        data = token.ToObject<RandomEventData>();
                        break;
                    case 5:
                        data = token.ToObject<EnvironmentData>();
                        break;
                    case 6:
                        data = token.ToObject<PlayerData>();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                return data;
            }
            return null;
        }
    }
    
    public class RewardGridItem
    {
        public string id;
        public string title;
        public Color color;
        public Image image;
        public Texture2D imageTexture;
        public JToken data;
        public bool customReward;

        public RewardGridItem(string id, string title, Color color, Texture2D imageTexture, bool customReward, JToken data = null)
        {
            this.id = id;
            this.title = title;
            this.color = color;
            this.imageTexture = imageTexture;
            this.data = data;
            this.customReward = customReward;
        }
    }

    public class SettingsChangedArgs : EventArgs
    {
        public string RewardId { get; set; }
        public SettingsMessageData Data { get; set; }
    }

    public class SettingsMessageData
    {
        [JsonProperty("action")]
        public int Action = 0;
    }

    public class RavenMessageData : SettingsMessageData
    {
        public RavenMessageData()
        {
            this.Action = 1;
        }

        [JsonProperty("isMunin")]
        public bool IsMunin { set; get; }
    }

    public class SpawnCreatureData : SettingsMessageData
    {
        public SpawnCreatureData()
        {
            this.Action = 2;
        }

        [JsonProperty("creature")]
        public int Creature { set; get; }

        [JsonProperty("level")]
        public int Level { set; get; }

        [JsonProperty("count")]
        public int Count { set; get; }

        [JsonProperty("distance")]
        public int Distance { set; get; }

        [JsonProperty("tamed")]
        public bool Tamed { set; get; }
    }

    public class HUDMessageData : SettingsMessageData
    {
        public HUDMessageData()
        {
            this.Action = 3;
        }
        
        [JsonProperty("isCentered")]
        public bool IsCentered { set; get; }
    }

    public class RandomEventData : SettingsMessageData
    {
        public RandomEventData()
        {
            this.Action = 4;
        }
        [JsonProperty("eventName")]
        public string EventName { set; get; }
        [JsonProperty("distance")]
        public int Distance { get; set; }
        [JsonProperty("duration")]
        public int Duration { get; set; }
    }

    public class EnvironmentData : SettingsMessageData
    {
        public EnvironmentData()
        {
            this.Action = 5;
        }
        [JsonProperty("name")]
        public string Name { set; get; }
        [JsonProperty("duration")]
        public int Duration { get; set; }
    }

    public class PlayerData : SettingsMessageData
    {
        public PlayerData()
        {
            this.Action = 6;
        }
        [JsonProperty("name")]
        public string Name { set; get; }
    }

    public class KeyCodeArgs : EventArgs
    {
        public KeyCode Code { get; set; }
    }
}
