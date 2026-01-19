using GambleCore.Interface;
using GambleCore.Util;
using System.Collections.Generic;
using System.Linq;

namespace GambleCore.Gambling
{
    internal class Board : IGamblingBoard
    {
        private const string SubsystemName = "GambleCore.GamblingBoard";
        private IRng _stepRng;
        private ulong _seed;
        private readonly int _offset;
        private readonly GamblingController _parent;
        private GambleConfig Config => _parent.Config;

        private readonly List<IWheel> _wheels = new List<IWheel>();

        public int Height => _wheels.Max(x => x.Height);
        public int Width => _wheels.Count;

        internal Board(GamblingController parent, int offset = 0, ulong seed = 0)
        {
            _parent = parent;
            _offset = offset;
            Reseed(seed);
        }

        private void StepRng()
        {
            _seed = _stepRng.NextUInt();
        }

        public void AddWheel(IWheel wheel) => _wheels.Add(wheel);

        public void RemoveWheel(IWheel wheel) => _wheels.RemoveAll(x => x == wheel);

        public override string ToString() => BuildCurrentMatcher().BuildString();

        private Matcher BuildCurrentMatcher()
        {
            var board = new Array2D<ISymbol>(Width, Height, null);

            for (var x = 0; x < Width; ++x)
            {
                var wheel = _wheels[x];
                var yOffset = (Height - wheel.Height) / 2;
                var shown = wheel.ShownSymbols;
                for (var y = 0; y < shown.Length; ++y)
                {
                    board[x, y + yOffset] = shown[y];
                }
            }

            return new Matcher(board);
        }

        private void Reseed(ulong seed)
        {
            _stepRng = DeterministicRng.CreateStreamMultiseed(seed, SubsystemName, unchecked((ulong)_offset));
            _seed = _stepRng.NextUInt();
        }

        private IRng GetRngFor(string subsystem) =>
            DeterministicRng.CreateStream(_seed, $"{SubsystemName}.{subsystem}");

        public int[] GetRandomSteps()
        {
            var wheelRng = GetRngFor("RandomSteps");
            var offsets = _wheels.Select(x => x.GetRandomStepCount(wheelRng)).ToArray();

            var visualRng = GetRngFor("StepCounts");
            var fixedSpinAdjustment = visualRng.NextRange(Config.FixedSpinAdjustment);
            var wheelSpinAdjustment = offsets.Select(_ => visualRng.NextRange(Config.WheelSpinAdjustment)).ToArray();
            var wheelSecondaryAdjustment = offsets.Select(_ => visualRng.NextRange(Config.MinWheelOffset)).ToArray();

            var steps = offsets.Select((x, i) =>
                x + (fixedSpinAdjustment + wheelSpinAdjustment[i]) * _wheels[i].SymbolsPerRotation
            ).ToList();

            // fixup wheels to ensure they stop from left to right
            for (var i = 1; i < steps.Count; ++i)
            {
                var wheelSize = _wheels[i].SymbolsPerRotation;
                var secondaryOffset = wheelSecondaryAdjustment[i] * wheelSize;
                while (steps[i - 1] + secondaryOffset >= steps[i])
                {
                    steps[i] += secondaryOffset;
                }
            }

            return steps.ToArray();
        }

        public void PerformSteps(int[] steps)
        {
            for (var i = 0; i < steps.Length; ++i)
                _wheels[i].Step(steps[i]);
            StepRng();
        }
    }
}