using ValheimTwitch.Gui;
using ValheimTwitch.Helpers;
using ValheimTwitch.Patches;
using ValheimTwitch.Twitch.PubSub.Messages;

namespace ValheimTwitch.Events
{
    internal class SpawnCreatureAction
    {
        internal static void Run(Redemption redemption, SpawnCreatureData data)
        {
            var creature = data.Creature;
            var level = data.Level + 1;
            var count = data.Count;
            var offset = data.Distance;
            var tamed = data.Tamed;

            var name = redemption.User.DisplayName;

            for (int i = 0; i < count; i++)
            {
                ConsoleUpdatePatch.AddAction(() => Prefab.Spawn(creature, level, offset, tamed, name));
            }
        }
    }
}