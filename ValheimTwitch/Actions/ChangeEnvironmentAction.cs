using System;
using System.Threading.Tasks;
using ValheimTwitch.Gui;
using ValheimTwitch.Helpers;
using ValheimTwitch.Twitch.PubSub.Messages;

namespace ValheimTwitch.Events
{
    internal class ChangeEnvironmentAction
    {
        internal static void Run(Redemption redemption, EnvironmentData data)
        {
            var name = data.Name;
            var duration = data.Duration;

            if (name == "Reset to default")
                name = "";

            if (EnvMan.instance == null)
                return;

            //Log.Info($"Env -> {name} {duration}");

            EnvMan.instance.m_debugEnv = name;

            try
            {
                if (Player.m_localPlayer != null)
                {
                    var type = MessageHud.MessageType.Center;
                    var user = redemption.User.DisplayName;
                    var text = redemption.UserInput?.Truncate(79)??"";
                    var msg1 = $"{user} says \"There is no bad weather, {name} incoming.\"";
                    var msg2 = $"It's time to catch a rest, {user} calls the weather spirits to calm them down.";
                    var msg = name.Length > 0 ? msg1 : msg2;

                    Player.m_localPlayer.Message(type, $"{msg}\n{text}");
                }

                if (duration > 0)
                {
                    Task.Delay(duration * 60000).ContinueWith(t => EnvMan.instance.m_debugEnv = "");
                }
            }
            catch (Exception ex)
            {
                Log.Warning(ex.StackTrace);
            }
        }

        public static void ListEnvironments()
        {
            var list = "";
            foreach(var env in EnvMan.instance.m_environments)
            {
                list += env.m_name + ",";
            }
            Log.Info("Environment list: " + list);
        }
    }
}