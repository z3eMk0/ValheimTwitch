using System;
using System.Collections.Generic;
using System.Linq;
using ValheimTwitch.Helpers;

namespace ValheimTwitch.Events
{
    class SupplyCartLootTables
    {
        public static LootTable GetTable(Player player, ItemType type)
        {
            if (type == ItemType.Food)
            {
                return FoodLootTable.GetTable(player);
            }
            if (type == ItemType.Mats)
            {
                return MatsLootTable.GetTable(player);
            }
            if (type == ItemType.Gems)
            {
                return GemsLootTable.GetTable(player);
            }
            // empty table
            return new LootTable();
        }

        private static bool PlayerKnows(Player player, string mat)
        {
            var knownMaterialField = player.GetType().GetField("m_knownMaterial",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            var knownMaterial = (HashSet<string>)knownMaterialField.GetValue(player);
            return knownMaterial.Contains(mat);
        }

        internal static bool PlayerKnowsMistMats(Player player)
        {
            return PlayerKnows(player, "Carapace") && PlayerKnows(player, "SoftTissue") && PlayerKnows(player, "BileBag");
        }
        internal static bool PlayerKnowsBlackMetal(Player player)
        {
            return PlayerKnows(player, "BlackMetal");
        }
        internal static bool PlayerKnowsSilver(Player player)
        {
            return PlayerKnows(player, "Silver");
        }
        internal static bool PlayerKnowsIron(Player player)
        {
            return PlayerKnows(player, "Iron");
        }
        internal static bool PlayerKnowsCopper(Player player)
        {
            return PlayerKnows(player, "Copper");
        }

        internal static void UpdateEntries(LootTable table, Dictionary<string, LootEntryData> entriesMap, Func<LootEntryData, LootDetails> selector)
        {
            Log.Info("table enries count " + table.Entries.Count());
            foreach (StackLootEntry entry in table.Entries)
            {
                var entryData = entriesMap[entry.Name];
                var details = selector(entryData);
                if (details != null)
                {
                    Log.Info($"Entry {entry.Name} details {details}");
                    entry.Chance = details.Chance;
                    entry.MinCount = details.MinCount;
                    entry.MaxCount = details.MaxCount;
                }
                else
                {
                    Log.Info($"Entry {entry.Name} no details");
                    entry.Chance = 0;
                }
                Log.Info("Entry after update " + entry);
            }
        }
    }
    class LootEntryData
    {
        public string Name;
        public ItemType Type;
        public LootDetails Stone;
        public LootDetails Copper;
        public LootDetails Iron;
        public LootDetails Silver;
        public LootDetails BlackMetal;
        public LootDetails Mist;
    }
    class LootDetails
    {
        public double Chance;
        public int MinCount;
        public int MaxCount;

        public override string ToString()
        {
            return $"LootDetails: Chance {Chance}, MinCount {MinCount}, MaxCount {MaxCount}";
        }
    }
    enum ItemType
    {
        None,
        Food,
        Mats,
        Gems
    }

    static class FoodLootTable
    {
        private static readonly LootTable table;
        private static readonly Dictionary<string, LootEntryData> entriesMap;
        private static readonly List<LootEntryData> entries = new List<LootEntryData>()
        {
            new LootEntryData()
            {
                Name = "RawMeat", Type = ItemType.Food,
                Stone = new LootDetails() { Chance = 5, MinCount = 3, MaxCount = 5 },
            },
            new LootEntryData()
            {
                Name = "DeerMeat", Type = ItemType.Food,
                Stone = new LootDetails() { Chance = 5, MinCount = 3, MaxCount = 5 },
                Copper = new LootDetails() { Chance = 5, MinCount = 3, MaxCount = 5 },
            },
            new LootEntryData()
            {
                Name = "Bloodbag", Type = ItemType.Food,
                Iron = new LootDetails() { Chance = 5, MinCount = 3, MaxCount = 5 },
            },
            new LootEntryData()
            {
                Name = "WolfMeat", Type = ItemType.Food,
                Silver = new LootDetails() { Chance = 5, MinCount = 3, MaxCount = 5 },
            },
            new LootEntryData()
            {
                Name = "LoxMeat", Type = ItemType.Food,
                BlackMetal = new LootDetails() { Chance = 5, MinCount = 3, MaxCount = 5 },
            },
            new LootEntryData()
            {
                Name = "BugMeat", Type = ItemType.Food,
                Mist = new LootDetails() { Chance = 5, MinCount = 3, MaxCount = 5 },
            }
        };
        static FoodLootTable()
        {
            table = new LootTable
            {
                MinCount = 2,
                MaxCount = 4,
                Entries = entries.Select(entry => new StackLootEntry()
                {
                    Name = entry.Name,
                    Chance = 0,
                    MinCount = 0,
                    MaxCount = 0
                }).ToArray()
            };
            entriesMap = new Dictionary<string, LootEntryData>(entries.Count);
            foreach (var entry in entries)
            {
                Log.Info("Adding loot entry " + entry.Name);
                entriesMap.Add(entry.Name, entry);
            }
        }
        // TODO ip: a better approach is to update the loot table only when the player learns a key material (iron, silver, etc)
        internal static LootTable GetTable(Player player)
        {
            if (SupplyCartLootTables.PlayerKnowsMistMats(player))
            {
                Log.Info("updating entries for mist age");
                SupplyCartLootTables.UpdateEntries(table, entriesMap, item => item.Mist);
            }
            else if (SupplyCartLootTables.PlayerKnowsBlackMetal(player))
            {
                Log.Info("updating entries for black metal age");
                SupplyCartLootTables.UpdateEntries(table, entriesMap, item => item.BlackMetal);
            }
            else if (SupplyCartLootTables.PlayerKnowsSilver(player))
            {
                Log.Info("updating entries for silver age");
                SupplyCartLootTables.UpdateEntries(table, entriesMap, item => item.Silver);
            }
            else if (SupplyCartLootTables.PlayerKnowsIron(player))
            {
                Log.Info("updating entries for iron age");
                SupplyCartLootTables.UpdateEntries(table, entriesMap, item => item.Iron);
            }
            else if (SupplyCartLootTables.PlayerKnowsCopper(player))
            {
                Log.Info("updating entries for copper age");
                SupplyCartLootTables.UpdateEntries(table, entriesMap, item => item.Copper);
            }
            else
            {
                Log.Info("updating entries for stone age");
                SupplyCartLootTables.UpdateEntries(table, entriesMap, item => item.Stone);
            }
            return table;
        }
    }

    static class MatsLootTable
    {
        private static readonly LootTable table;
        private static readonly Dictionary<string, LootEntryData> entriesMap;
        private static readonly List<LootEntryData> entries = new List<LootEntryData>()
        {
#region Stone age
            new LootEntryData()
            {
                Name = "Stone", Type = ItemType.Mats,
                Stone = new LootDetails() { Chance = 5, MinCount = 8, MaxCount = 13 },
            },
            new LootEntryData()
            {
                Name = "Wood", Type = ItemType.Mats,
                Stone = new LootDetails() { Chance = 5, MinCount = 8, MaxCount = 13 },
            },
            new LootEntryData()
            {
                Name = "RoundLog", Type = ItemType.Mats,
                Stone = new LootDetails() { Chance = 5, MinCount = 3, MaxCount = 6 },
            },
            new LootEntryData()
            {
                Name = "FineWood", Type = ItemType.Mats,
                Stone = new LootDetails() { Chance = 5, MinCount = 3, MaxCount = 6 },
            },
            new LootEntryData()
            {
                Name = "Flint", Type = ItemType.Mats,
                Stone = new LootDetails() { Chance = 5, MinCount = 4, MaxCount = 7 },
            },
            new LootEntryData()
            {
                Name = "Resin", Type = ItemType.Mats,
                Stone = new LootDetails() { Chance = 5, MinCount = 4, MaxCount = 7 },
            },
#endregion

            new LootEntryData()
            {
                Name = "CopperOre", Type = ItemType.Mats,
                Copper = new LootDetails() { Chance = 5, MinCount = 3, MaxCount = 5 },
            },
            new LootEntryData()
            {
                Name = "TinOre", Type = ItemType.Mats,
                Copper = new LootDetails() { Chance = 5, MinCount = 3, MaxCount = 5 },
            },
            new LootEntryData()
            {
                Name = "IronScrap", Type = ItemType.Mats,
                Iron = new LootDetails() { Chance = 5, MinCount = 3, MaxCount = 5 },
            },
            new LootEntryData()
            {
                Name = "SilverOre", Type = ItemType.Mats,
                Silver = new LootDetails() { Chance = 5, MinCount = 3, MaxCount = 5 },
            },
            new LootEntryData()
            {
                Name = "BlackMetalScrap", Type = ItemType.Mats,
                BlackMetal = new LootDetails() { Chance = 5, MinCount = 3, MaxCount = 5 },
            },
            new LootEntryData()
            {
                Name = "Carapace", Type = ItemType.Mats,
                Mist = new LootDetails() { Chance = 5, MinCount = 4, MaxCount = 6 },
            }
        };
        static MatsLootTable()
        {
            table = new LootTable
            {
                MinCount = 2,
                MaxCount = 5,
                Entries = entries.Select(entry => new StackLootEntry()
                {
                    Name = entry.Name,
                    Chance = 0,
                    MinCount = 0,
                    MaxCount = 0
                }).ToArray()
            };
            entriesMap = new Dictionary<string, LootEntryData>(entries.Count);
            foreach (var entry in entries)
            {
                Log.Info("Adding loot entry " + entry.Name);
                entriesMap.Add(entry.Name, entry);
            }
        }
        // TODO ip: a better approach is to update the loot table only when the player learns a key material (iron, silver, etc)
        internal static LootTable GetTable(Player player)
        {
            if (SupplyCartLootTables.PlayerKnowsMistMats(player))
            {
                Log.Info("updating entries for mist age");
                SupplyCartLootTables.UpdateEntries(table, entriesMap, item => item.Mist);
            }
            else if (SupplyCartLootTables.PlayerKnowsBlackMetal(player))
            {
                Log.Info("updating entries for black metal age");
                SupplyCartLootTables.UpdateEntries(table, entriesMap, item => item.BlackMetal);
            }
            else if (SupplyCartLootTables.PlayerKnowsSilver(player))
            {
                Log.Info("updating entries for silver age");
                SupplyCartLootTables.UpdateEntries(table, entriesMap, item => item.Silver);
            }
            else if (SupplyCartLootTables.PlayerKnowsIron(player))
            {
                Log.Info("updating entries for iron age");
                SupplyCartLootTables.UpdateEntries(table, entriesMap, item => item.Iron);
            }
            else if (SupplyCartLootTables.PlayerKnowsCopper(player))
            {
                Log.Info("updating entries for copper age");
                SupplyCartLootTables.UpdateEntries(table, entriesMap, item => item.Copper);
            }
            else
            {
                Log.Info("updating entries for stone age");
                SupplyCartLootTables.UpdateEntries(table, entriesMap, item => item.Stone);
            }
            return table;
        }
    }

    static class GemsLootTable
    {
        private static readonly LootTable table;
        private static readonly Dictionary<string, LootEntryData> entriesMap;
        private static readonly List<LootEntryData> entries = new List<LootEntryData>()
        {
            new LootEntryData()
            {
                Name = "Coins", Type = ItemType.Gems,
                Stone = new LootDetails() { Chance = 5, MinCount = 11, MaxCount = 20 },
                Copper = new LootDetails() { Chance = 5, MinCount = 21, MaxCount = 30 },
                Iron = new LootDetails() { Chance = 5, MinCount = 31, MaxCount = 40 },
                Silver = new LootDetails() { Chance = 5, MinCount = 41, MaxCount = 50 },
                BlackMetal = new LootDetails() { Chance = 5, MinCount = 51, MaxCount = 60 },
                Mist = new LootDetails() { Chance = 5, MinCount = 61, MaxCount = 120 },
            },
            new LootEntryData()
            {
                Name = "Amber", Type = ItemType.Gems,
                Stone = new LootDetails() { Chance = 5, MinCount = 1, MaxCount = 3 },
                Copper = new LootDetails() { Chance = 5, MinCount = 3, MaxCount = 6 },
                Iron = new LootDetails() { Chance = 5, MinCount = 6, MaxCount = 9 },
                Silver = new LootDetails() { Chance = 5, MinCount = 6, MaxCount = 11 },
                BlackMetal = new LootDetails() { Chance = 5, MinCount = 6, MaxCount = 12 },
                Mist = new LootDetails() { Chance = 5, MinCount = 6, MaxCount = 13 },
            },
            new LootEntryData()
            {
                Name = "AmberPearl", Type = ItemType.Gems,
                Stone = new LootDetails() { Chance = 5, MinCount = 1, MaxCount = 2 },
                Copper = new LootDetails() { Chance = 5, MinCount = 2, MaxCount = 4 },
                Iron = new LootDetails() { Chance = 5, MinCount = 2, MaxCount = 5 },
                Silver = new LootDetails() { Chance = 5, MinCount = 3, MaxCount = 6 },
                BlackMetal = new LootDetails() { Chance = 5, MinCount = 3, MaxCount = 7 },
                Mist = new LootDetails() { Chance = 5, MinCount = 3, MaxCount = 8 },
            },
            new LootEntryData()
            {
                Name = "Ruby", Type = ItemType.Gems,
                Stone = new LootDetails() { Chance = 5, MinCount = 1, MaxCount = 2 },
                Copper = new LootDetails() { Chance = 5, MinCount = 2, MaxCount = 3 },
                Iron = new LootDetails() { Chance = 5, MinCount = 2, MaxCount = 4 },
                Silver = new LootDetails() { Chance = 5, MinCount = 3, MaxCount = 5 },
                BlackMetal = new LootDetails() { Chance = 5, MinCount = 3, MaxCount = 6 },
                Mist = new LootDetails() { Chance = 5, MinCount = 3, MaxCount = 7 },
            },
            new LootEntryData()
            {
                Name = "SilverNecklace", Type = ItemType.Gems,
                Silver = new LootDetails() { Chance = 5, MinCount = 1, MaxCount = 3 },
                BlackMetal = new LootDetails() { Chance = 5, MinCount = 2, MaxCount = 4 },
                Mist = new LootDetails() { Chance = 5, MinCount = 3, MaxCount = 5 },
            },
        };
        static GemsLootTable()
        {
            table = new LootTable
            {
                MinCount = 1,
                MaxCount = 3,
                Entries = entries.Select(entry => new StackLootEntry()
                {
                    Name = entry.Name,
                    Chance = 0,
                    MinCount = 0,
                    MaxCount = 0
                }).ToArray()
            };
            entriesMap = new Dictionary<string, LootEntryData>(entries.Count);
            foreach (var entry in entries)
            {
                Log.Info("Adding loot entry " + entry.Name);
                entriesMap.Add(entry.Name, entry);
            }
        }
        // TODO ip: a better approach is to update the loot table only when the player learns a key material (iron, silver, etc)
        internal static LootTable GetTable(Player player)
        {
            if (SupplyCartLootTables.PlayerKnowsMistMats(player))
            {
                Log.Info("updating entries for mist age");
                SupplyCartLootTables.UpdateEntries(table, entriesMap, item => item.Mist);
            }
            else if (SupplyCartLootTables.PlayerKnowsBlackMetal(player))
            {
                Log.Info("updating entries for black metal age");
                SupplyCartLootTables.UpdateEntries(table, entriesMap, item => item.BlackMetal);
            }
            else if (SupplyCartLootTables.PlayerKnowsSilver(player))
            {
                Log.Info("updating entries for silver age");
                SupplyCartLootTables.UpdateEntries(table, entriesMap, item => item.Silver);
            }
            else if (SupplyCartLootTables.PlayerKnowsIron(player))
            {
                Log.Info("updating entries for iron age");
                SupplyCartLootTables.UpdateEntries(table, entriesMap, item => item.Iron);
            }
            else if (SupplyCartLootTables.PlayerKnowsCopper(player))
            {
                Log.Info("updating entries for copper age");
                SupplyCartLootTables.UpdateEntries(table, entriesMap, item => item.Copper);
            }
            else
            {
                Log.Info("updating entries for stone age");
                SupplyCartLootTables.UpdateEntries(table, entriesMap, item => item.Stone);
            }
            return table;
        }
    }
}
