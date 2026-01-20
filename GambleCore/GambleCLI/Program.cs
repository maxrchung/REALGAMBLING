using GambleCore;
using GambleCore.Gambling;
using GambleCore.Interface;


var controller = new GamblingController
{
    Config =
    {
        FixedSpinAdjustment = ConfigRange.Single(0),
        WheelSpinAdjustment = ConfigRange.Single(0),
        MinWheelOffset = ConfigRange.Single(7),
    }
};

var board = controller.CreateBoard(0);
var wheel1 = new TestWheel();
wheel1.AddSymbol(SymbolType.Cross, SymbolType.Cross, SymbolType.Circle, SymbolType.Square, SymbolType.Triangle);
board.AddWheel(wheel1);
var wheel2 = new TestWheel();
wheel2.SetHeight(1);
wheel2.AddSymbol(SymbolType.Circle, SymbolType.Triangle, SymbolType.Square);
board.AddWheel(wheel2);
var wheel3 = new TestWheel();
wheel3.SetHeight(5);
wheel3.AddSymbol(SymbolType.Triangle, SymbolType.Triangle, SymbolType.Triangle, SymbolType.Triangle,
    SymbolType.Triangle);
board.AddWheel(wheel3);

Console.WriteLine($"Initial state:\n{board}");
var steps = board.GetRandomSteps();
Console.WriteLine($"Rolled: {string.Join(',', steps)}");
board.PerformSteps(steps);
Console.WriteLine($"New state:\n{board}");

var matcher = board.BuildCurrentMatcher();
var matches = new List<Match>();
matches.AddRange(matcher.Match(Pattern.PatternLine));
matches.AddRange(matcher.Match(Pattern.PatternVerticalLine));
matches.AddRange(matcher.Match(Pattern.PatternDiagonal));
Console.WriteLine($"Matches: {string.Join(',', matches)}");

internal enum SymbolType
{
    Cross = 1,
    Circle = 2,
    Square = 3,
    Triangle = 4
}

internal class TestSymbol(SymbolType type) : ISymbol
{
    public readonly SymbolType Type = type;

    public override int GetHashCode()
    {
        return Type.GetHashCode();
    }

    public override string ToString()
    {
        return Type switch
        {
            SymbolType.Cross => "X",
            SymbolType.Circle => "O",
            SymbolType.Square => "S",
            SymbolType.Triangle => "T",
            _ => "?"
        };
    }
}

internal class TestWheel : AbstractWheel
{
    private int _height = 3;
    public override int Height => _height;

    protected override ISymbol[] Symbols => _symbols.ToArray();

    private readonly List<ISymbol> _symbols = [];

    public void AddSymbol(params SymbolType[] type) => _symbols.AddRange(type.Select(t => new TestSymbol(t)));
    public void SetHeight(int height) => _height = height;
}