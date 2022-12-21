using ValheimTwitch.Gui;
using ValheimTwitch.Patches;
using ValheimTwitch.Twitch.PubSub.Messages;

namespace ValheimTwitch.Events
{
    internal class RavenMessageAction
    {
        internal static void Run(Redemption redemption, RavenMessageData data)
        {
            RavenPatch.Message(redemption.User.DisplayName, redemption.UserInput?? "Caw! Caw!", data.IsMunin);
        }
    }
}