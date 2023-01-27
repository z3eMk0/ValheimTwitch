using System.Collections.Generic;
using System.Threading.Tasks;
using ValheimTwitch.Gui;
using ValheimTwitch.Helpers;
using ValheimTwitch.Patches;
using ValheimTwitch.Twitch.PubSub.Messages;

namespace ValheimTwitch.Events
{
    internal class SupplyCartAction
    {
        internal static void Run(Redemption redemption, SupplyCartData data)
        {
            var type = data.Type;
            var offset = data.Distance;
            var count = data.Count;
            var interval = data.Interval;

            var name = redemption.User.DisplayName;

            if (Player.m_localPlayer != null)
            {
                Player.m_localPlayer.Message(MessageHud.MessageType.Center, $"A gift from {name}");
            }

            if (count == 1)
            {
                var drops = GetDrops(type);
                Log.Info($"Supplying {drops}");
                ConsoleUpdatePatch.AddAction(() => Prefab.SpawnSupplyCart(drops, offset));
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    Task.Delay(i * interval * 1000).ContinueWith(t =>
                    {
                        var drops = GetDrops(type);
                        Log.Info($"Supplying {drops}");
                        ConsoleUpdatePatch.AddAction(() => Prefab.SpawnSupplyCart(drops, offset));
                    });
                }
            }
        }

        private static IEnumerable<LootItem> GetDrops(string type)
        {
            ItemType itemType = ItemType.None;
            if (type == SupplyCartData.FOOD_TYPE)
            {
                itemType = ItemType.Food;
            }
            else if (type == SupplyCartData.MATS_TYPE)
            {
                itemType = ItemType.Mats;
            }
            else if (type == SupplyCartData.GEMS_TYPE)
            {
                itemType = ItemType.Gems;
            }
            return SupplyCartLootTables.GetTable(Player.m_localPlayer, itemType).GetLoot();
        }
    }
}
