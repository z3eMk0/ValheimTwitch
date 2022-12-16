using ValheimTwitch.Gui;
using ValheimTwitch.Twitch.PubSub.Messages;

namespace ValheimTwitch.Events
{
    public class HUDMessageAction
    {
        public static void PlayerMessage(string message, bool center = true)
        {
            var messageType = center ? MessageHud.MessageType.Center : MessageHud.MessageType.TopLeft;

            if (Player.m_localPlayer != null)
            {
                Player.m_localPlayer.Message(messageType, message);
            }
        }

        public static void Run(Redemption redemption, HUDMessageData data)
        {
            var isCentered = data.IsCentered;
            var user = redemption.User.DisplayName;
            var text = redemption.UserInput ?? "";

            PlayerMessage($"<{user}> {text}", isCentered);
        }
    }
}