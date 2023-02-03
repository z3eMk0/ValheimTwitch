using System.Collections.Generic;

namespace ValheimTwitch.Events
{
    public class SpawnControl
    {
        private static List<string> disabledRewards = new List<string>();
        public static void EnableSpawns()
        {
            foreach (string id in disabledRewards)
            {
                Plugin.Instance.twitchClient.ToggleReward(id, true);
            }
            disabledRewards.Clear();
        }

        public static void DisableSpawns()
        {
            var customRewards = Plugin.Instance.twitchClient.GetCustomRewards();
            disabledRewards = customRewards.Data.FindAll(reward =>
            {
                var rewardSettings = Plugin.Instance.configProvider.Rewards.GetSettings(reward.Id);
                return reward.IsEnabled && Actions.IsSpawn(rewardSettings.Action);
            }).ConvertAll(r => r.Id);
            foreach (string id in disabledRewards)
            {
                Plugin.Instance.twitchClient.ToggleReward(id, false);
            }
        }
    }
}
