using System.Collections.Generic;

namespace ValheimTwitch.Events
{
    public class SpawnCreatureSettings
    {

        private bool IsTameable(int index)
        {
            return !untameableCreatures.Contains(creatures[index]);
        }

        private bool IsTameable(string name)
        {
            return !untameableCreatures.Contains(name);
        }


        public static List<string> untameableCreatures = new List<string>
    {
        "Crow",
        "Deer",
        "Fish1",
        "Fish2",
        "Fish3",
        "Fish4_cave",
        "Gull",
        "Leviathan",
        "Odin",
        "Eikthyr",
        "gd_king",
        "Bonemass",
        "Dragon",
        "GoblinKing",
    };

        public static List<string> creatures = new List<string> {
        "Blob",
        "BlobElite",
        "Boar",
        "Crow",
        "Deathsquito",
        "Deer",
        "Draugr",
        "Draugr_Elite",
        "Draugr_Ranged",
        "Fenring",
        "FireFlies",
        "Fish1",
        "Fish2",
        "Fish3",
        "Ghost",
        "Goblin",
        "GoblinArcher",
        "GoblinBrute",
        "GoblinShaman",
        "Greydwarf",
        "Greydwarf_Elite",
        "Greydwarf_Shaman",
        "Greyling",
        "Hatchling",
        "Leech",
        "Leviathan",
        "Lox",
        "Neck",
        "Seagal",
        "Serpent",
        "Skeleton",
        "Skeleton_NoArcher",
        "Skeleton_Poison",
        "StoneGolem",
        "Surtling",
        "Troll",
        "Wolf",
        "Wraith",
        "Eikthyr",
        "gd_king",
        "Bonemass",
        "Dragon",
        "GoblinKing",
    };
    }
}
