using System.Threading.Tasks;
using ValheimTwitch.Gui;
using ValheimTwitch.Helpers;
using ValheimTwitch.Patches;
using ValheimTwitch.Twitch.PubSub.Messages;

namespace ValheimTwitch.Events
{
    internal class MeteorDropsAction
    {
        internal static void Run(Redemption redemption, MeteorDropsData data)
        {
            var type = data.Type;
            var offset = data.Distance;
            var count = data.Count;
            var interval = data.Interval;

            var name = redemption.User.DisplayName;

            if (Player.m_localPlayer != null)
            {
                Player.m_localPlayer.Message(MessageHud.MessageType.Center, $"Enjoy the shower from {name}");
            }

            if (count == 1)
            {
                ConsoleUpdatePatch.AddAction(() => Prefab.SpawnFallingDrop(type, offset));
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    Task.Delay(i * interval).ContinueWith(t =>
                    {
                        ConsoleUpdatePatch.AddAction(() => Prefab.SpawnFallingDrop(type, offset));
                    });
                }
            }
        }
    }
}
