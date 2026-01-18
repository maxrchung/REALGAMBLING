using System;

namespace GambleCore.Util
{
    public interface IRng
    {
        uint NextUInt();

        /// <summary>
        /// Returns a random uint in [min, max)
        /// </summary>
        uint NextUInt(uint min, uint max);

        /// <summary>
        /// Returns a random int in [min, max)
        /// </summary>
        int NextInt(int min, int max);

        /// <summary>
        /// Returns a random float in [0,1)
        /// </summary>
        float NextFloat();
    }

    public struct Pcg32 : IRng
    {
        // PCG-XSH-RR 32 (LCG 64 -> output 32)
        private ulong _state;
        private readonly ulong _inc; // must be odd

        public Pcg32(ulong seed, ulong stream)
        {
            _state = 0;
            _inc = (stream << 1) | 1UL;
            NextUInt();
            _state = unchecked(_state + seed);
            NextUInt();
        }

        public uint NextUInt()
        {
            unchecked
            {
                var old = _state;
                _state = old * 6364136223846793005UL + _inc;
                var xorshifted = (uint)(((old >> 18) ^ old) >> 27);
                var rot = (int)(old >> 59);
                return RotateRight(xorshifted, rot);
            }
        }

        private uint NextUInt(uint max)
        {
            if (max <= 0) throw new ArgumentOutOfRangeException(nameof(max));
            var threshold = unchecked(0u - max) % max;

            while (true)
            {
                var r = NextUInt();
                if (r >= threshold)
                    return (r % max);
            }
        }

        public uint NextUInt(uint min, uint exclusiveMax) => NextUInt(exclusiveMax - min) + min;
        public int NextInt(int min, int exclusiveMax) => unchecked((int)NextUInt((uint)exclusiveMax - (uint)min) + min);

        public float NextFloat()
        {
            var r = NextUInt();
            var mantissa24 = r >> 8;
            return mantissa24 * (1.0f / 16777216.0f);
        }

        private static uint RotateRight(uint x, int r)
        {
            r &= 31;
            return (x >> r) | (x << ((32 - r) & 31));
        }
    }

    public static class DeterministicRng
    {
        private static ulong SplitMix64(ref ulong x)
        {
            unchecked
            {
                x += 0x9E3779B97F4A7C15UL;
                var z = x;
                z = (z ^ (z >> 30)) * 0xBF58476D1CE4E5B9UL;
                z = (z ^ (z >> 27)) * 0x94D049BB133111EBUL;
                return z ^ (z >> 31);
            }
        }

        private static ulong Fnv1a64(string s)
        {
            unchecked
            {
                const ulong offset = 1469598103934665603UL;
                const ulong prime = 1099511628211UL;
                var h = offset;
                foreach (var t in s)
                {
                    h ^= (byte)t;
                    h *= prime;
                    h ^= (byte)(t >> 8);
                    h *= prime;
                }

                return h;
            }
        }

        public static Pcg32 CreateStream(ulong masterSeed, string subsystemName)
        {
            var x = masterSeed;
            var seed = SplitMix64(ref x);
            var stream = SplitMix64(ref x) ^ Fnv1a64(subsystemName);
            return new Pcg32(seed, stream);
        }

        public static Pcg32 CreateStreamMultiseed(ulong masterSeed, string subsystemName, ulong subSeed)
        {
            var x = masterSeed;
            var y = subSeed;
            var seed = SplitMix64(ref x);
            var stream = SplitMix64(ref x) ^ Fnv1a64(subsystemName) ^ SplitMix64(ref y);
            return new Pcg32(seed, stream);
        }
    }
}