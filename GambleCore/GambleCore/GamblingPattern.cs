using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GambleCore.Util;

namespace GambleCore
{
    public class GamblingPattern
    {
        private readonly Array2D<GambleSymbol> _pattern;

        public int Width => _pattern.Width;
        public int Height => _pattern.Height;

        public GambleSymbol this[int x, int y] => _pattern[x, y];

        public struct Position
        {
            public int X, Y;
        }

        public struct GambleMatch
        {
            public GambleSymbol Symbol;
            public Position[] Positions;
        }

        public GamblingPattern(GambleSymbol[,] pattern)
        {
            _pattern = new Array2D<GambleSymbol>(pattern);
        }

        public GamblingPattern(string pattern)
        {
            var reader = new StringReader(pattern);
            var gamblingLines = new List<GambleSymbol[]>();
            for (var line = reader.ReadLine(); line != null; line = reader.ReadLine())
            {
                gamblingLines.Add(
                    line.Select(x => x == ' ' ? GambleSymbol.Blank : GambleSymbol.SelectSymbol1).ToArray());
            }

            var width = gamblingLines.Max(x => x.Length);
            var height = gamblingLines.Count;
            _pattern = new Array2D<GambleSymbol>(width, height);
            for (var y = 0; y < height; ++y)
            for (var x = 0; x < width; ++x)
                _pattern[y, x] = gamblingLines[y][x];
        }

        public static readonly GamblingPattern PatternLine = new GamblingPattern(
            new[,]
            {
                {
                    GambleSymbol.SelectSymbol1, GambleSymbol.SelectSymbol1, GambleSymbol.SelectSymbol1
                },
            }
        );

        public static readonly GamblingPattern PatternVerticalLine = new GamblingPattern(
            new[,]
            {
                { GambleSymbol.SelectSymbol1 },
                { GambleSymbol.SelectSymbol1 },
                { GambleSymbol.SelectSymbol1 }
            });

        public static readonly GamblingPattern PatternDiagonal = new GamblingPattern(
            new[,]
            {
                {
                    GambleSymbol.SelectSymbol1, GambleSymbol.SelectIgnoredSymbol, GambleSymbol.SelectIgnoredSymbol,
                },
                {
                    GambleSymbol.SelectIgnoredSymbol, GambleSymbol.SelectSymbol1, GambleSymbol.SelectIgnoredSymbol,
                },
                {
                    GambleSymbol.SelectIgnoredSymbol, GambleSymbol.SelectIgnoredSymbol, GambleSymbol.SelectSymbol1,
                }
            }
        );
    }
}