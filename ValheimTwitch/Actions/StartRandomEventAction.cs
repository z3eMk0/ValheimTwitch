using UnityEngine;
using ValheimTwitch.Gui;
using ValheimTwitch.Patches;
using ValheimTwitch.Twitch.PubSub.Messages;

namespace ValheimTwitch.Events
{
    internal class StartRandomEventAction
    {
        internal static void Run(Redemption redemption, RandomEventData data)
        {
            ConsoleUpdatePatch.AddAction(() => StartRandomEvent(redemption, data));
        }

        private static void StartRandomEvent(Redemption redemption, RandomEventData data)
        {
            var eventName = data.EventName;
            var distance = data.Distance;
            var duration = data.Duration;

            Vector3 b = Random.insideUnitSphere * distance;
            var position = Player.m_localPlayer.transform.position + Player.m_localPlayer.transform.forward * 2f + Vector3.up + b;

            RandomEventSystemStartPatch.StartEvent(redemption, eventName, position, duration);
        }
    }
}