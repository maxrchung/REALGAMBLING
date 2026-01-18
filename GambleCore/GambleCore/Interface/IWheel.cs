using GambleCore.Util;

namespace GambleCore.Interface
{
    public interface IWheel
    {
        int Height { get; }

        int SymbolsPerRotation { get; }

        ISymbol[] ShownSymbols { get; }

        void Step(int count = 1);

        int GetRandomStepCount(IRng rng);
    }
}