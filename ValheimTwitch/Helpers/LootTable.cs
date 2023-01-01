using System.Collections.Generic;

namespace ValheimTwitch.Helpers
{
    public class LootTable
    {
        public int MinCount { get; set; }
        public int MaxCount { get; set; }
        public IEnumerable<ILootEntry> Entries { get; set; }
        public ICollection<LootItem> GetLoot()
        {
            var allChances = 0d;
            foreach (var entry in Entries)
            {
                Log.Info("Calculating chance " + entry);
                if (entry.Chance < 0)
                {
                    Log.Info($"Entry {entry} has negative chance of {entry.Chance}");
                }
                else
                {
                    allChances += entry.Chance;
                }
            }
            int count = RandomGen.GetInt(MinCount, MaxCount + 1);
            List<LootItem> result = new List<LootItem>(count);
            if (allChances > 0)
            {
                while (count > 0)
                {
                    var chance = RandomGen.GetDouble(allChances);
                    double accumulatedChance = 0;
                    foreach (var entry in Entries)
                    {
                        if (entry.Chance > 0)
                        {
                            accumulatedChance += entry.Chance;
                            if (accumulatedChance > chance)
                            {
                                var loot = entry.Loot;
                                result.AddRange(loot);
                                count -= loot.Count;
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                Log.Info("All loot entries have chances of 0 so returning empty loot");
            }
            return result;
        }
        public LootTable()
        {
            this.Entries = new List<ILootEntry>();
        }
    }
    public interface ILootEntry
    {
        double Chance { get; }

        ICollection<LootItem> Loot { get; }
    }

    public class StackLootEntry : ILootEntry
    {
        public double Chance { get; set; }
        public int MinCount { get; set; }
        public int MaxCount { get; set; }
        public string Name { get; set; }

        public ICollection<LootItem> Loot
        {
            get
            {
                var count = RandomGen.GetInt(MinCount, MaxCount + 1);
                return new List<LootItem>(1)
                {
                    new LootItem(Name, count, 0, string.Empty)
                };
            }
        }

        public override string ToString()
        {
            return $"StackLootEntry: Name {Name}, Chance {Chance}, MinCount {MinCount}, MaxCount {MaxCount}";
        }
    }

    public struct LootItem
    {
        public string Name;
        public int Stack;
        public int Quality;
        public string CrafterName;
        public LootItem(string name, int stack, int quality, string crafterName)
        {
            this.Name = name;
            this.Stack = stack;
            this.Quality = quality;
            this.CrafterName = crafterName;
        }

        public override string ToString()
        {
            return $"LootItem: Name {Name}, Stack {Stack}, Quality {Quality}, Crafter name {CrafterName}";
        }
    }
}
