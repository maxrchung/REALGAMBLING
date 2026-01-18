using System;
using System.Collections;
using System.Collections.Generic;

namespace GambleCore.Util
{
    public class ArrayView<T> : IEnumerable<T>
    {
        private readonly T[] _backing;
        private readonly int _offset;
        private readonly int _stride;

        public readonly int Length;

        public ArrayView(T[] backing, int offset, int length, int stride = 1)
        {
            _backing = backing;
            _offset = offset;
            _stride = stride;
            Length = length;
        }

        public T this[int index] => _backing[_offset + index * _stride];

        public IEnumerator<T> GetEnumerator()
        {
            for (var i = 0; i < Length; ++i)
                yield return this[i];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class Array2D<T> : IEnumerable<ArrayView<T>>
    {
        private readonly T[] _backing;
        public readonly int Width, Height;

        public Array2D(int width, int height)
        {
            _backing = new T[width * height];
            Width = width;
            Height = height;
        }

        public Array2D(int width, int height, T fillValue) : this(width, height) => Fill(fillValue);

        public Array2D(T[,] matrix)
        {
            Width = matrix.GetLength(1);
            Height = matrix.GetLength(0);
            _backing = new T[Width * Height];
            for (var y = 0; y < Height; ++y)
            for (var x = 0; x < Width; ++x)
                _backing[y * Width + x] = matrix[y, x];
        }

        public void Fill(T value)
        {
            for (var i = 0; i < _backing.Length; ++i) _backing[i] = value;
        }

        private int GetOffset(int x, int y) => y * Width + x;

        public T this[int x, int y]
        {
            get => _backing[GetOffset(x, y)];
            set => _backing[GetOffset(x, y)] = value;
        }

        public ArrayView<T> AtRow(int y) => new ArrayView<T>(_backing, GetOffset(0, y), Width);

        public ArrayView2D<T> View(int offsetX, int offsetY, int width = -1, int height = -1)
        {
            var actualWidth = width == -1 ? Width - offsetX : width;
            var actualHeight = height == -1 ? Height - offsetY : height;
            return new ArrayView2D<T>(_backing, offsetX, offsetY, actualWidth, actualHeight, Width);
        }

        public IEnumerator<ArrayView<T>> GetEnumerator()
        {
            for (var y = 0; y < Height; ++y)
                yield return AtRow(y);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class ArrayView2D<T> : IEnumerable<ArrayView<T>>
    {
        private readonly T[] _backing;
        private readonly int _offset;
        private readonly int _rowStride;

        public readonly int Width, Height;

        private int GetOffset(int x, int y) => _offset + y * _rowStride + x;

        public ArrayView2D(T[] backing, int offsetX, int offsetY, int width, int height, int rowStride = -1)
        {
            _backing = backing;
            _rowStride = rowStride >= 0 ? rowStride : width;
            _offset = offsetY * _rowStride + offsetX;
            Width = width;
            Height = height;
        }

        public ArrayView<T> AtRow(int y) => new ArrayView<T>(_backing, GetOffset(0, y), Width);

        public IEnumerator<ArrayView<T>> GetEnumerator()
        {
            for (var y = 0; y < Height; ++y)
                yield return AtRow(y);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public T this[int x, int y] => _backing[GetOffset(x, y)];
    }
}