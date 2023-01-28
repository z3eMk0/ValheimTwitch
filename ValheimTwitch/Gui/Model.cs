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
                var action = token["action"].ToString();
                switch (action)
                {
                    case SettingsMessageData.ACTION_TYPE:
                        return new SettingsMessageData();
                    case RavenMessageData.ACTION_TYPE:
                        return token.ToObject<RavenMessageData>();
                    case SpawnCreatureData.ACTION_TYPE:
                        return token.ToObject<SpawnCreatureData>();
                    case HUDMessageData.ACTION_TYPE:
                        return token.ToObject<HUDMessageData>();
                    case RandomEventData.ACTION_TYPE:
                        return token.ToObject<RandomEventData>();
                    case EnvironmentData.ACTION_TYPE:
                        return token.ToObject<EnvironmentData>();
                    case PlayerData.ACTION_TYPE:
                        return token.ToObject<PlayerData>();
                    case SupplyCartData.ACTION_TYPE:
                        return token.ToObject<SupplyCartData>();
                    case MeteorDropsData.ACTION_TYPE:
                        return token.ToObject<MeteorDropsData>();
                    default:
                        throw new ArgumentOutOfRangeException();
                }
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
        public const string ACTION_TYPE = "none";
        [JsonProperty("action")]
        public virtual string Action
        {
            get
            {
                return ACTION_TYPE;
            }
        }
    }

    public class RavenMessageData : SettingsMessageData
    {
        public new const string ACTION_TYPE = "raven";
        public override string Action
        {
            get
            {
                return ACTION_TYPE;
            }
        }

        [JsonProperty("isMunin")]
        public bool IsMunin { set; get; }
    }

    public class SpawnCreatureData : SettingsMessageData
    {
        public new const string ACTION_TYPE = "spawn";
        public override string Action
        {
            get
            {
                return ACTION_TYPE;
            }
        }

        [JsonProperty("creature")]
        public string Creature { set; get; }

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
        public new const string ACTION_TYPE = "message";
        public override string Action
        {
            get
            {
                return ACTION_TYPE;
            }
        }

        [JsonProperty("isCentered")]
        public bool IsCentered { set; get; }
    }

    public class RandomEventData : SettingsMessageData
    {
        public new const string ACTION_TYPE = "event";
        public override string Action
        {
            get
            {
                return ACTION_TYPE;
            }
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
        public new const string ACTION_TYPE = "environment";
        public override string Action
        {
            get
            {
                return ACTION_TYPE;
            }
        }
        [JsonProperty("name")]
        public string Name { set; get; }
        [JsonProperty("duration")]
        public int Duration { get; set; }
    }

    public class PlayerData : SettingsMessageData
    {
        public new const string ACTION_TYPE = "player";
        public override string Action
        {
            get
            {
                return ACTION_TYPE;
            }
        }
        [JsonProperty("name")]
        public string Name { set; get; }
    }

    public class SupplyCartData : SettingsMessageData
    {
        public new const string ACTION_TYPE = "supply";
        public const string FOOD_TYPE = "food";
        public const string MATS_TYPE = "mats";
        public const string GEMS_TYPE = "gems";
        public override string Action
        {
            get
            {
                return ACTION_TYPE;
            }
        }

        [JsonProperty("type")]
        public string Type { set; get; }

        [JsonProperty("distance")]
        public int Distance { set; get; }
        [JsonProperty("count")]
        public int Count { set; get; }
        [JsonProperty("interval")]
        public int Interval { set; get; }
    }

    public class MeteorDropsData : SettingsMessageData
    {
        public new const string ACTION_TYPE = "meteor";
        public override string Action
        {
            get
            {
                return ACTION_TYPE;
            }
        }
        [JsonProperty("type")]
        public string Type { set; get; }

        [JsonProperty("distance")]
        public int Distance { set; get; }
        [JsonProperty("count")]
        public int Count { set; get; }
        [JsonProperty("interval")]
        public int Interval { set; get; }
    }

    public class KeyCodeArgs : EventArgs
    {
        public KeyCode Code { get; set; }
    }
}
