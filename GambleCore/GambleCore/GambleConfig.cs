using GambleCore.Util;

namespace GambleCore
{
    public struct ConfigRange
    {
        public int Min;
        public int Max;

        /// <summary>
        /// Configurable range, [min, max)
        /// </summary>
        private ConfigRange(int min, int max)
        {
            Min = min;
            Max = max;
        }

        public static ConfigRange Inclusive(int min, int max) => new ConfigRange(min, max + 1);

        public static ConfigRange Exclusive(int min, int max) => new ConfigRange(min, max);

        public static ConfigRange Single(int value) => Inclusive(value, value);
    }

    internal static class IRngExtensions
    {
        public static int NextRange(this IRng rng, ConfigRange range) => rng.NextInt(range.Min, range.Max);
    }

    public class GambleConfig
    {
        /// <summary>
        /// Base range of wheel revolutions, applied to all wheels
        /// </summary>
        public ConfigRange FixedSpinAdjustment { get; set; } = ConfigRange.Inclusive(3, 5);

        /// <summary>
        /// Range of additional revolutions, applied per wheel
        /// </summary>
        public ConfigRange WheelSpinAdjustment { get; set; } = ConfigRange.Inclusive(2, 7);

        /// <summary>
        /// Minimum offset between each wheel's stopping position, per wheel
        /// </summary>
        public ConfigRange MinWheelOffset { get; set; } = ConfigRange.Single(3);
    }
}