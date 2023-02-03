using System;
using System.Collections.Generic;
using ValheimTwitch.Gui;
using ValheimTwitch.Twitch.PubSub.Messages;

namespace ValheimTwitch.Events
{
    // 0 None
    // 1 RavenMessage
    // 2 SpawnCreature
    // 3 HUD message
    // 4 Start event
    // 5 Environement
    // 6 Player action (puke, heal, ...)

    public static class Actions
    {
        private static HashSet<string> spawnActions = new HashSet<string>
        {
            SpawnCreatureData.ACTION_TYPE,
            RandomEventData.ACTION_TYPE,
            SupplyCartData.ACTION_TYPE,
            MeteorDropsData.ACTION_TYPE,
        };
        internal static void RunAction(Redemption redemption, SettingsMessageData data)
        {
            try
            {
                var type = data.Action;

                Log.Info($"RunAction: {redemption.Reward.Id} -> type: {type}");

                switch (type)
                {
                    case SettingsMessageData.ACTION_TYPE:
                        break;
                    case RavenMessageData.ACTION_TYPE:
                        RavenMessageAction.Run(redemption, data as RavenMessageData);
                        break;
                    case SpawnCreatureData.ACTION_TYPE:
                        SpawnCreatureAction.Run(redemption, data as SpawnCreatureData);
                        break;
                    case HUDMessageData.ACTION_TYPE:
                        HUDMessageAction.Run(redemption, data as HUDMessageData);
                        break;
                    case RandomEventData.ACTION_TYPE:
                        StartRandomEventAction.Run(redemption, data as RandomEventData);
                        break;
                    case EnvironmentData.ACTION_TYPE:
                        ChangeEnvironmentAction.Run(redemption, data as EnvironmentData);
                        break;
                    case PlayerData.ACTION_TYPE:
                        PlayerAction.Run(redemption, data as PlayerData);
                        break;
                    case SupplyCartData.ACTION_TYPE:
                        SupplyCartAction.Run(redemption, data as SupplyCartData);
                        break;
                    case MeteorDropsData.ACTION_TYPE:
                        MeteorDropsAction.Run(redemption, data as MeteorDropsData);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (Exception ex)
            {
                Log.Error("RunAction Error >>> " + ex.ToString());
            }
        }

        public static bool IsSpawn(string action)
        {
            return spawnActions.Contains(action);
        }
    }
}
