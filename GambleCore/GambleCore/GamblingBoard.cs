using System;
using System.Collections.Generic;
using System.Linq;
using GambleCore.Util;

namespace GambleCore
{
    public class GamblingBoard
    {
        private class WheelState
        {
            public readonly GamblingWheel Wheel;
            private int _currentSlot = 0;

            public WheelState(GamblingWheel wheel)
            {
                Wheel = wheel;
            }

            public GambleSymbol[] GetShownSymbols()
            {
                var symbols = new GambleSymbol[Wheel.ShownSymbols];
                for (var i = 0; i < symbols.Length; ++i)
                {
                    symbols[i] = Wheel.Symbols[(_currentSlot + i) % Wheel.Symbols.Length];
                }

                return symbols;
            }

            public void Spin()
            {
                if (_currentSlot == 0) _currentSlot = Wheel.Symbols.Length - 1;
                else _currentSlot--;
            }
        }

        public class Matcher
        {
            private readonly Array2D<GambleSymbol> _board;
            private int _width => _board.Width;
            private int _height => _board.Height;

            private bool MatchAt(ArrayView2D<GambleSymbol> board, GamblingPattern pattern,
                out GamblingPattern.GambleMatch match)
            {
                match = new GamblingPattern.GambleMatch
                {
                    Symbol = GambleSymbol.NoMatch
                };
                var positions = new List<GamblingPattern.Position>();

                for (var y = 0; y < board.Height; ++y)
                {
                    for (var x = 0; x < board.Width; ++x)
                    {
                        var expected = pattern[x, y];
                        var actual = _board[x, y];
                        if (expected == GambleSymbol.SelectIgnoredSymbol) continue;
                        if (match.Symbol == GambleSymbol.NoMatch)
                        {
                            // can't have blank symbols
                            if (actual == GambleSymbol.Blank) return false;
                            match.Symbol = actual;
                        }
                        else if (match.Symbol != actual)
                        {
                            return false;
                        }

                        positions.Add(new GamblingPattern.Position { X = x, Y = y });
                    }
                }

                match.Positions = positions.ToArray();
                return true;
            }

            internal Matcher(Array2D<GambleSymbol> board)
            {
                _board = board;
            }

            public GamblingPattern.GambleMatch[] Match(GamblingPattern pattern)
            {
                var matches = new List<GamblingPattern.GambleMatch>();
                for (var y = 0; y <= _height - pattern.Height; ++y)
                for (var x = 0; x <= _width - pattern.Width; ++x)
                {
                    var view = _board.View(x, y, pattern.Width, pattern.Height);
                    if (MatchAt(view, pattern, out var match))
                    {
                        // offset by origin
                        match.Positions = match.Positions
                            .Select(p => new GamblingPattern.Position { X = p.X + x, Y = p.Y + y }).ToArray();
                        matches.Add(match);
                    }
                }

                return matches.ToArray();
            }

            public string BuildBoardString() => string.Join("\n", _board.Select(x => string.Join(", ", x)));
        }

        private readonly List<WheelState> _wheels = new List<WheelState>();

        public void AddWheel(GamblingWheel wheel)
        {
            _wheels.Add(new WheelState(wheel));
        }

        public void SpinExcept(int offset)
        {
            for (var i = offset; i < _wheels.Count; ++i)
            {
                _wheels[i].Spin();
            }
        }

        public Matcher BuildCurrentMatcher()
        {
            var height = _wheels.Select(x => x.Wheel.ShownSymbols).Max();
            var width = _wheels.Count;
            var board = new Array2D<GambleSymbol>(width, height, GambleSymbol.Blank);

            for (var x = 0; x < width; ++x)
            {
                var wheel = _wheels[x];
                var shown = wheel.GetShownSymbols();
                for (var y = 0; y < shown.Length; ++y)
                {
                    board[x, y] = shown[y];
                }
            }

            return new Matcher(board);
        }

        public string BuildWheelStateString() => BuildCurrentMatcher().BuildBoardString();
    }
}