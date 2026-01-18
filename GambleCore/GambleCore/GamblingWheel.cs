namespace GambleCore
{
    public class GamblingWheel
    {
        public int ShownSymbols { get; }
        public GambleSymbol[] Symbols { get; }

        public GamblingWheel(GambleSymbol[] symbols, int shownSymbols = 3)
        {
            Symbols = symbols;
            ShownSymbols = shownSymbols;
        }

        public static readonly GamblingWheel TestWheel = new GamblingWheel(new[]
        {
            GambleSymbol.Symbol1, GambleSymbol.Symbol2, GambleSymbol.Symbol3, GambleSymbol.Symbol4, GambleSymbol.Symbol5
        });
    }
}