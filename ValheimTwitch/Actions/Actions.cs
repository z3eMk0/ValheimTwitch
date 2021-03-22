﻿using System.Collections.Generic;
using ValheimTwitch.Twitch.PubSub.Messages;

namespace ValheimTwitch.Events
{

    abstract public class Action
    {
        public virtual void Run(Redemption redemption)
        {
            Log.Info($"Run -> {GetType().Name}");
        }
    }

    public static class Actions
    {
        public enum Types : int
        {
            None = 0,
            SpawnHugin = 1,
            SpawnMunin = 2,
            SpawnTroll = 3,
            SpawnBoarHord = 4,
            PrintCenterMessage = 100,
            PrintTopLeftMessage = 101
        }
        
        private static Dictionary<int, string> names = new Dictionary<int, string>
        {
            { (int)Types.None, "None" },
            { (int)Types.SpawnHugin, "Spawn Hugin" },
            { (int)Types.SpawnMunin, "Spawn Munin" },
            { (int)Types.SpawnTroll, "Spawn troll" },
            { (int)Types.SpawnBoarHord, "Spawn boar hord" },
            { (int)Types.PrintCenterMessage, "Print center message" },
            { (int)Types.PrintTopLeftMessage, "Print top left message" }
        };

        private static Dictionary<int, Action> actions = new Dictionary<int, Action>
        {
            { (int)Types.None, new NoneAction() },
            { (int)Types.SpawnHugin, new SpawnHuginAction() },
            { (int)Types.SpawnMunin, new SpawnMuninAction() },
            { (int)Types.SpawnTroll, new SpawnTrollAction() },
            { (int)Types.SpawnBoarHord, new SpawnBoarHordAction() },
            { (int)Types.PrintCenterMessage, new PrintCenterMessageAction() },
            { (int)Types.PrintTopLeftMessage, new PrintTopLeftMessageAction() }
        };

        public static Dictionary<int, string> GetActionNames()
        {
            return names;
        }

        public static string GetActionName(Types type)
        {
            return names[(int)type];
        }

        public static string GetActionName(int type)
        {
            return names[type];
        }

        internal static void RunAction(Redemption redemption)
        {
            var type = PluginConfig.GetInt("reward-actions", redemption.Reward.Id);

            actions[type].Run(redemption);
        }
    }
}