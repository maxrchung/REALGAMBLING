using System.Diagnostics;
using System.Linq;
using GambleCore.Interface;
using GambleCore.Util;

namespace GambleCore.Gambling
{
    public abstract class AbstractWheel : IWheel
    {
        public virtual int Height { get; }
        protected virtual ISymbol[] Symbols { get; }

        protected virtual int[] SymbolWeights => Enumerable.Repeat(1, Symbols.Length).ToArray();

        public int SymbolsPerRotation => Symbols.Length;

        private int _currentSlot;

        public void Step(int count = 1)
        {
            _currentSlot = (_currentSlot + count) % Symbols.Length;
        }

        public ISymbol[] ShownSymbols => GetShownSymbols();

        private ISymbol[] GetShownSymbols()
        {
            var symbols = new ISymbol[Height];
            for (var i = 0; i < symbols.Length; ++i)
            {
                symbols[symbols.Length - i - 1] = Symbols[(_currentSlot + i) % Symbols.Length];
            }

            return symbols;
        }

        public ISymbol GetRandomSymbol(IRng rng)
        {
            var totalWeight = SymbolWeights.Sum();
            var roll = rng.NextInt(0, totalWeight);
            for (var i = 0; i < Symbols.Length; ++i)
            {
                if (roll < SymbolWeights[i]) return Symbols[i];
                roll -= SymbolWeights[i];
            }

            return null;
        }

        public int GetRandomStepCount(IRng rng)
        {
            var totalWeight = SymbolWeights.Sum();
            var roll = rng.NextInt(0, totalWeight);
            for (var i = 0; i < Symbols.Length; ++i)
            {
                if (roll < SymbolWeights[i]) return i;
                roll -= SymbolWeights[i];
            }

            return 0;
        }

        private bool FindSymbol(ISymbol symbol, out int slot)
        {
            for (var i = 0; i < SymbolsPerRotation; ++i)
                if (Symbols[i] == symbol)
                {
                    slot = i;
                    return true;
                }

            slot = -1;
            return false;
        }

        private int StepsUntil(ISymbol symbol)
        {
            if (!FindSymbol(symbol, out var slot)) return -1;
            return slot - _currentSlot;
        }

        public int StepsUntilSymbolCenter(ISymbol symbol)
        {
            var startSteps = StepsUntil(symbol);
            if (startSteps < 0) return -1;
            var stepCount = (startSteps - (Height / 2)) % Symbols.Length;
            if (stepCount < 0) stepCount += Symbols.Length;
            return stepCount;
        }
    }

    public class NullWheel : AbstractWheel
    {
        public override int Height => 0;
        protected override ISymbol[] Symbols => new ISymbol[] { };
    }
}