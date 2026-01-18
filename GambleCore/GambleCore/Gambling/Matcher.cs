using System.Collections.Generic;
using System.Linq;
using System.Text;
using GambleCore.Interface;
using GambleCore.Util;

namespace GambleCore.Gambling
{
    public struct MatchPosition
    {
        public int X;
        public int Y;
    }

    public struct Match
    {
        public struct Group
        {
            public ISymbol Symbol;
            public MatchPosition[] Positions;

            internal Group OffsetBy(int x, int y) => new Group
            {
                Symbol = Symbol,
                Positions = Positions.Select(p => new MatchPosition { X = p.X + x, Y = p.Y + y }).ToArray()
            };
        }

        public Group[] Groups;
    }

    public class Matcher
    {
        private readonly Array2D<ISymbol> _board;
        private int Width => _board.Width;
        private int Height => _board.Height;

        private bool MatchAt(ArrayView2D<ISymbol> board, Pattern pattern,
            out Match match)
        {
            match = default;
            var storedSymbols = new ISymbol[pattern.UsedSymbolCount];
            var hitPositions = new List<MatchPosition>[pattern.UsedSymbolCount];

            for (var y = 0; y < board.Height; ++y)
            {
                for (var x = 0; x < board.Width; ++x)
                {
                    var expected = pattern[x, y];
                    var actual = _board[x, y];
                    if (expected == 0) continue;
                    if (storedSymbols[expected] != null)
                    {
                        storedSymbols[expected] = actual;
                    } // we can't mismatch already, so only check on second symbol of group
                    else if (storedSymbols[expected] != actual)
                    {
                        // missed, bail out
                        return false;
                    }

                    hitPositions[expected].Add(new MatchPosition { X = x, Y = y });
                }
            }

            var groups = new List<Match.Group>();
            for (var i = 0; i < pattern.UsedSymbolCount; ++i)
            {
                if (storedSymbols[i] == null) continue;
                groups.Add(new Match.Group { Symbol = storedSymbols[i], Positions = hitPositions[i].ToArray() });
            }

            match = new Match { Groups = groups.ToArray() };

            return true;
        }

        internal Matcher(Array2D<ISymbol> board)
        {
            _board = board;
        }

        public Match[] Match(Pattern pattern)
        {
            var matches = new List<Match>();
            for (var y = 0; y <= Height - pattern.Height; ++y)
            for (var x = 0; x <= Width - pattern.Width; ++x)
            {
                var view = _board.View(x, y, pattern.Width, pattern.Height);
                if (MatchAt(view, pattern, out var match))
                {
                    matches.Add(new Match
                    {
                        Groups = match.Groups.Select(g => g.OffsetBy(x, y)).ToArray()
                    });
                }
            }

            return matches.ToArray();
        }

        public string BuildString()
        {
            return string.Join("\n", _board.Select(row => string.Join("", row.Select(x => x?.ToString() ?? " "))));
        }
    }
}