using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GambleCore.Interface;
using GambleCore.Util;

namespace GambleCore.Gambling
{
    public class Pattern
    {
        private readonly Array2D<int> _pattern;
        public int UsedSymbolCount => _pattern.SelectMany(x => x).Max();

        public int Width => _pattern.Width;
        public int Height => _pattern.Height;

        public Pattern(int[,] pattern)
        {
            _pattern = new Array2D<int>(pattern);
        }

        public int this[int x, int y] => _pattern[y, x];

        private int CharToSymbol(char c)
        {
            if (c == ' ' || c == '_') return 0;
            if (c >= '1' && c <= '9') return c - '0';
            if (c == 'X' || c == 'x') return 1;
            throw new InvalidDataException($"Invalid symbol character '{c}'");
        }

        public Pattern(string pattern)
        {
            var reader = new StringReader(pattern);
            var gamblingLines = new List<int[]>();
            for (var line = reader.ReadLine(); line != null; line = reader.ReadLine())
            {
                gamblingLines.Add(
                    line.Select(CharToSymbol).ToArray());
            }

            var width = gamblingLines.Max(x => x.Length);
            var height = gamblingLines.Count;
            _pattern = new Array2D<int>(width, height);
            for (var y = 0; y < height; ++y)
            for (var x = 0; x < width; ++x)
                _pattern[y, x] = gamblingLines[y][x];
        }

        public static readonly Pattern PatternLine = new Pattern(
            new[,]
            {
                {
                    1, 1, 1
                },
            }
        );

        public static readonly Pattern PatternVerticalLine = new Pattern(
            new[,]
            {
                { 1 },
                { 1 },
                { 1 }
            });

        public static readonly Pattern PatternDiagonal = new Pattern(
            new[,]
            {
                {
                    1, 0, 0
                },
                {
                    0, 1, 0
                },
                {
                    0, 0, 1
                }
            }
        );
    }
}