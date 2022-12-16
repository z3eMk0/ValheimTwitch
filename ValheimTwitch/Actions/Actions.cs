using System;
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
        internal static void RunAction(Redemption redemption, SettingsMessageData data)
        {
            try
            {
                var type = data.Action;

                Log.Info($"RunAction: {redemption.Reward.Id} -> type: {type}");

                switch (type)
                {
                    case 0:
                        break;
                    case 1:
                        RavenMessageAction.Run(redemption, data as RavenMessageData);
                        break;
                    case 2:
                        SpawnCreatureAction.Run(redemption, data as SpawnCreatureData);
                        break;
                    case 3:
                        HUDMessageAction.Run(redemption, data as HUDMessageData);
                        break;
                    case 4:
                        StartRandomEventAction.Run(redemption, data as RandomEventData);
                        break;
                    case 5:
                        ChangeEnvironmentAction.Run(redemption, data as EnvironmentData);
                        break;
                    case 6:
                        PlayerAction.Run(redemption, data as PlayerData);
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
    }
}
