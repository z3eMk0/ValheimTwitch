using System.Collections.Generic;
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
            var drops = GetDrops(type);
            var offset = data.Distance;

            var name = redemption.User.DisplayName;
            Log.Info($"Supplying {drops}");

            ConsoleUpdatePatch.AddAction(() => Prefab.SpawnSupplyCart(drops, offset, name));
        }

        private static IEnumerable<LootItem> GetDrops(string type)
        {
            if (type == "food")
            {
                return GetFood();
            }
            if (type == "mats")
            {
                return GetMats();
            }
            Log.Info($"Unsupported supply type \"type\"");
            return new List<LootItem>(0);
        }

        private static IEnumerable<LootItem> GetFood()
        {
            return GetFoodLootTable().GetLoot();
        }

        private static IEnumerable<LootItem> GetMats()
        {
            return GetMatsLootTable().GetLoot();
        }

        private static LootTable GetFoodLootTable()
        {
            return SupplyCartLootTables.GetTable(Player.m_localPlayer, ItemType.Food);
        }

        private static LootTable GetMatsLootTable()
        {
            var table = new LootTable
            {
                MinCount = 2,
                MaxCount = 5
            };

            //table.Entries.Add(new StackLootEntry
            //{
            //    Name = "FineWood",
            //    Chance = 5,
            //    MinCount = 4,
            //    MaxCount = 6
            //});

            //table.Entries.Add(new StackLootEntry
            //{
            //    Name = "RoundLog",
            //    Chance = 5,
            //    MinCount = 8,
            //    MaxCount = 12
            //});

            return table;
        }
    }
}
