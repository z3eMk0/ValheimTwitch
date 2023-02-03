using UnityEngine;
using ValheimTwitch.Twitch.Auth;

namespace ValheimTwitch.Config
{
    public interface IConfigProvider
    {
        RewardsConfig Rewards { get; }
        string HelixUrl { get; }
        Credentials Credentials { get; }
        bool EnableRewardsAutomatically { get; }
        KeyCode ToggleSpawnRewardsKey { get; }
    }
}
