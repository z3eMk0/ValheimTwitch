using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;
using ValheimTwitch.Helpers;
using ValheimTwitch.Twitch.PubSub.Messages;

// bats
// blobs
// foresttrolls
// skeletons
// surtlings
// wolves

// army_bonemass
// army_eikthyr
// army_goblin
// army_moder
// army_theelder
// army_seekers
// army_gjalls

// boss_bonemass
// boss_eikthyr
// boss_gdking
// boss_goblinking
// boss_moder
// boss_queen

namespace ValheimTwitch.Patches
{
    [HarmonyPatch(typeof(RandEventSystem))]
    public class RandomEventSystemStartPatch
    {
        [HarmonyReversePatch]
        [HarmonyPatch(typeof(RandEventSystem), "SetRandomEvent", new Type[] { typeof(RandomEvent), typeof(Vector3) })]
        public static void PublicSetRandomEvent(RandEventSystem instance, RandomEvent ev, Vector3 pos)
        {
            return;
        }

        public static RandomEvent GetEventByName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }

            foreach (RandomEvent randomEvent in RandEventSystem.instance.m_events)
            {
                if (randomEvent.m_name == name)
                {
                    return randomEvent.Clone();
                }
            }

            return null;
        }

        public static void StartEvent(Redemption redemption, string name, Vector3 position, int duration)
        {
            var randomEvent = GetEventByName(name);

            if (randomEvent == null)
                return;

            var user = redemption.User.DisplayName;
            var text = redemption.UserInput?.Truncate(79)??"";

            randomEvent.m_enabled = true;
            randomEvent.m_nearBaseOnly = false;
            randomEvent.m_duration = duration * 60f;
            randomEvent.m_pauseIfNoPlayerInArea = false;

            randomEvent.m_startMessage = GetEventStartMessage(name, user.ToUpper(), text);
            randomEvent.m_endMessage = $"You survived the raid of {user.ToUpper()}!\nSame player play again!";

            PublicSetRandomEvent(RandEventSystem.instance, randomEvent, position);
        }

        private static readonly Dictionary<string, string> eventLabels = new Dictionary<string, string>()
        {
            { "bats", "a horde of bats" },
            { "blobs", "a horde of blobs" },
            { "foresttrolls", "a horde of trolls" },
            { "skeletons", "a horde of skeletons" },
            { "surtlings", "a horde of surtlings" },
            { "wolves", "a horde of wolves" },
            { "army_bonemass", "the army of Bonemass" },
            { "army_eikthyr", "the army of Eikthyr" },
            { "army_goblin", "the army of Yagluth" },
            { "army_moder", "the army of Moder" },
            { "army_theelder", "the army of the Elder" },
            { "army_seekers", "an army of seekers" },
            { "army_gjall", "an army of gjalls" },
            { "boss_bonemass", "Bonemass itself" },
            { "boss_eikthyr", "Eikthyr itself" },
            { "boss_gdking", "the Elder himself" },
            { "boss_goblinking", "Yagluth himself" },
            { "boss_moder", "Moder herself" },
            { "boss_queen", "the Queen herself" },
        };

        private static string GetEventStartMessage(string eventName, string userName, string text)
        {
            string eventLabel;
            if (!eventLabels.TryGetValue(eventName, out eventLabel))
            {
                eventLabel = $"a horde of {eventName.ToUpper()}";
            }
            return $"{userName} is raiding you with {eventLabel}!\nRun for your life!\n{text}";
        }
    }
}

