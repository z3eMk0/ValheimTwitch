using System;

namespace ValheimTwitch.Helpers
{
    public static class RandomGen
    {
        private static readonly Random random = new Random();
        public static double GetDouble(double max)
        {
            if (max < 0)
            {
                throw new ArgumentOutOfRangeException("max", "max argument should be greater than or equal to zero");
            }
            return random.NextDouble() * max;
        }
        public static double GetDouble(double min, double max)
        {
            if (min < 0)
            {
                throw new ArgumentOutOfRangeException("min", "min argument should be greater than or equal to zero");
            }
            if (max < min)
            {
                throw new ArgumentOutOfRangeException("max", "max argument cannot be less than min argument");
            }
            return min + random.NextDouble() * (max - min);
        }
        public static int GetInt(int max)
        {
            return random.Next(max);
        }
        public static int GetInt(int min, int max)
        {
            return random.Next(min, max);
        }
    }
}
