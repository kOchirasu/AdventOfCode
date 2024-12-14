﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using QuikGraph;

namespace UtilExtensions;

public static class ArrayExtensions {
    public enum Axis {
        Horizontal,
        Vertical,
    }

    public enum Direction {
        Origin = 1,
        N = 2,
        E = 4,
        S = 8,
        W = 16,
        NE = 32,
        SE = 64,
        SW = 128,
        NW = 256,
    }

    [Flags]
    public enum Directions {
        Origin = Direction.Origin,
        N = Direction.N,
        E = Direction.E,
        S = Direction.S,
        W = Direction.W,
        NE = Direction.NE,
        SE = Direction.SE,
        SW = Direction.SW,
        NW = Direction.NW,

        Cardinal = N | E | S | W,
        Intermediate = NE | SE | SW | NW,
        All = Cardinal | Intermediate,
    }

    [Flags]
    public enum AdjacencyOptions {
        None = 0,
        Wrap = 1,
        Expand = 2,
    }

    private static readonly Dictionary<Direction, (int, int)> Offsets = new() {
        {Direction.Origin, (0, 0)},
        {Direction.N, (-1, 0)},
        {Direction.E, (0, 1)},
        {Direction.S, (1, 0)},
        {Direction.W, (0, -1)},
        {Direction.NE, (-1, 1)},
        {Direction.SE, (1, 1)},
        {Direction.SW, (1, -1)},
        {Direction.NW, (-1, -1)},
    };

    private static readonly Direction[] Rotations = {
        Direction.N, Direction.NE, Direction.E, Direction.SE,
        Direction.S, Direction.SW, Direction.W, Direction.NW,
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int RowCount<T>(this T[,] arr) => arr.GetLength(0);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ColumnCount<T>(this T[,] arr) => arr.GetLength(1);

    public static bool TryGet<T>(this T[] arr, int n, [MaybeNullWhen(false)] out T value) {
        if (n < 0 || n >= arr.Length) {
            value = default(T);
            return false;
        }

        value = arr[n];
        return true;
    }

    public static T GetOrDefault<T>(this T[] arr, int n, T @default = default(T)) {
        if (n < 0 || n >= arr.Length) {
            return @default;
        }

        return arr[n];
    }

    public static T[] Append<T>(this T[] arr, T[] other) {
        var result = new T[arr.Length + other.Length];
        arr.CopyTo(result, 0);
        other.CopyTo(result, arr.Length);
        return result;
    }

    public static T[] Append<T>(this T[] arr, T other) {
        var result = new T[arr.Length + 1];
        arr.CopyTo(result, 0);
        result[arr.Length] = other;
        return result;
    }

    public static T[] Shift<T>(this T[] arr, int n, T extend = default(T)) {
        n %= arr.Length;
        (int src, int dst) shift = default;
        int fill = 0;
        switch (n) {
            case < 0: // Shift Left
                n *= -1;
                shift.src = n;
                fill = arr.Length - n;
                break;
            case > 0: // Shift Right
                shift.dst = n;
                fill = 0;
                break;
        }

        var result = new T[arr.Length];
        Array.Copy(arr, shift.src, result, shift.dst, arr.Length - n);
        if (extend != null) {
            Array.Fill(result, extend, fill, n);
        }

        return result;
    }

    public static T[] CircularShift<T>(this T[] arr, int n) {
        n %= arr.Length;
        (int src, int dst) shift = default;
        (int src, int dst) wrap = default;
        switch (n) {
            case < 0: // CircularShift Left
                n *= -1;
                shift.src = n;
                wrap.dst = arr.Length - n;
                break;
            case > 0: // CircularShift Right
                shift.dst = n;
                wrap.src = arr.Length - n;
                break;
        }

        var result = new T[arr.Length];
        Array.Copy(arr, shift.src, result, shift.dst, arr.Length - n);
        Array.Copy(arr, wrap.src, result, wrap.dst, n);

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGet<T>(this T[,] arr, (int Row, int Col) point, [NotNullWhen(true)] out T value) {
        return arr.TryGet(point.Row, point.Col, out value);
    }

    public static bool TryGet<T>(this T[,] arr, int row, int col, [NotNullWhen(true)] out T value) {
        if (row < 0 || col < 0 || row >= arr.GetLength(0) || col >= arr.GetLength(1)) {
            value = default;
            return false;
        }

        value = arr[row, col];
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T GetOrDefault<T>(this T[,] arr, (int Row, int Col) point, T @default = default(T)) {
        return arr.GetOrDefault<T>(point.Row, point.Col, @default);
    }

    public static T GetOrDefault<T>(this T[,] arr, int row, int col, T @default = default(T)) {
        if (row < 0 || col < 0 || row >= arr.GetLength(0) || col >= arr.GetLength(1)) {
            return @default;
        }

        return arr[row, col];
    }

    public static bool TryGet<T>(this T[,,] arr, int row, int col, int dep, [MaybeNullWhen(false)] out T value) {
        if (row < 0 || col < 0 || dep < 0 || row >= arr.GetLength(0) || col >= arr.GetLength(1) || dep >= arr.GetLength(2)) {
            value = default;
            return false;
        }

        value = arr[row, col, dep];
        return true;
    }

    public static T GetOrDefault<T>(this T[,,] arr, int row, int col, int dep, T @default = default(T)) {
        if (row < 0 || col < 0 || dep < 0 || row >= arr.GetLength(0) || col >= arr.GetLength(1) || dep >= arr.GetLength(2)) {
            return @default;
        }

        return arr[row, col, dep];
    }

     public static T[] GetColumn<T>(this T[,] arr, Index index) {
        int rows = arr.RowCount();
        int cols = arr.ColumnCount();
        int col = index.GetOffset(cols);
        if (col < 0 || col >= cols) {
            throw new IndexOutOfRangeException("Index was out of range. Must be non-negative and less than the size of the collection");
        }

        var result = new T[rows];
        for (int i = 0; i < rows; i++) {
            result[i] = arr[i, col];
        }

        return result;
    }

    public static void SetColumn<T>(this T[,] arr, Index index, T[] data) {
        int cols = arr.ColumnCount();
        int col = index.GetOffset(cols);
        if (col < 0 || col >= cols) {
            throw new IndexOutOfRangeException("Index was out of range. Must be non-negative and less than the size of the collection");
        }
        int rows = arr.RowCount();
        if (rows != data.Length) {
            throw new ArgumentException($"SetColumn with invalid length: {data.Length} != {rows}");
        }

        for (int i = 0; i < rows; i++) {
            arr[i, col] = data[i];
        }
    }

    public static IEnumerable<T[]> Columns<T>(this T[,] arr) {
        int count = arr.ColumnCount();
        for (int i = 0; i < count; i++) {
            yield return arr.GetColumn(i);
        }
    }

    public static T[] GetRow<T>(this T[,] arr, Index index) {
        int rows = arr.RowCount();
        int cols = arr.ColumnCount();
        int row = index.GetOffset(rows);
        if (row < 0 || row >= rows) {
            throw new IndexOutOfRangeException("Index was out of range. Must be non-negative and less than the size of the collection");
        }

        var result = new T[cols];
        for (int i = 0; i < cols; i++) {
            result[i] = arr[row, i];
        }

        return result;
    }

    public static void SetRow<T>(this T[,] arr, Index index, T[] data) {
        int rows = arr.RowCount();
        int row = index.GetOffset(rows);
        if (row < 0 || row >= rows) {
            throw new IndexOutOfRangeException("Index was out of range. Must be non-negative and less than the size of the collection");
        }
        int cols = arr.ColumnCount();
        if (cols != data.Length) {
            throw new ArgumentException($"SetColumn with invalid length: {data.Length} != {cols}");
        }

        for (int i = 0; i < cols; i++) {
            arr[row, i] = data[i];
        }
    }

    public static IEnumerable<T[]> Rows<T>(this T[,] arr) {
        int count = arr.RowCount();
        for (int i = 0; i < count; i++) {
            yield return arr.GetRow(i);
        }
    }

    public static IEnumerable<T> Convert<T>(this IEnumerable<string> arr) where T : IConvertible {
        return arr.Select(i => (T) System.Convert.ChangeType(i, typeof(T)));
    }

    public static T[,] Convert<T>(this string[,] arr) where T : IConvertible {
        return arr.Select(i => (T) System.Convert.ChangeType(i, typeof(T)));
    }

    public static void Fill<T>(this T[,] arr, T value) {
        int rows = arr.RowCount();
        int cols = arr.ColumnCount();
        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < cols; j++) {
                arr[i, j] = value;
            }
        }
    }

    public static T[] Copy<T>(this T[] arr) {
        var result = new T[arr.Length];
        Array.Copy(arr, result, arr.Length);

        return result;
    }

    public static T[,] Copy<T>(this T[,] arr) {
        int rows = arr.RowCount();
        int cols = arr.ColumnCount();
        var result = new T[rows, cols];
        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < cols; j++) {
                result[i, j] = arr[i, j];
            }
        }

        return result;
    }

    public static void Insert<T>(this T[,] arr, T[,] insert, int row, int col, bool strictBounds = true) {
        if (strictBounds && (row < 0 || col < 0)) {
            throw new IndexOutOfRangeException("Index was out of range. Must be non-negative");
        }

        int rows = arr.RowCount();
        int cols = arr.ColumnCount();
        int iRows = insert.RowCount();
        int iCols = insert.ColumnCount();
        if (strictBounds && (row + iRows > rows || col + iCols > cols)) {
            throw new IndexOutOfRangeException($"Cannot insert ({iRows}, {iCols}) into ({rows}, {cols}) at ({row}, {col}).");
        }

        for (int i = 0; i < iRows; i++) {
            for (int j = 0; j < iCols; j++) {
                if (!strictBounds) {
                    if (row + i < 0 || row + i >= rows) continue;
                    if (col + j < 0 || col + j >= cols) continue;
                }

                arr[row + i, col + j] = insert[i, j];
            }
        }
    }

    public static void ConditionalInsert<T>(this T[,] arr, T[,] insert, int row, int col, Func<T, bool> shouldInsert, bool strictBounds = true)  {
        if (strictBounds && (row < 0 || col < 0)) {
            throw new IndexOutOfRangeException("Index was out of range. Must be non-negative");
        }

        int rows = arr.RowCount();
        int cols = arr.ColumnCount();
        int iRows = insert.RowCount();
        int iCols = insert.ColumnCount();
        if (strictBounds && (row + iRows > rows || col + iCols > cols)) {
            throw new IndexOutOfRangeException($"Cannot insert ({iRows}, {iCols}) into ({rows}, {cols}) at ({row}, {col}).");
        }

        for (int i = 0; i < iRows; i++) {
            for (int j = 0; j < iCols; j++) {
                if (!strictBounds) {
                    if (row + i < 0 || row + i >= rows) continue;
                    if (col + j < 0 || col + j >= cols) continue;
                }

                if (shouldInsert(insert[i, j])) {
                    arr[row + i, col + j] = insert[i, j];
                }
            }
        }
    }

    public static T[,] Extract<T>(this T[,] arr, int row, int col, int rowCount = int.MaxValue, int colCount = int.MaxValue) {
        if (row < 0 || col < 0) {
            throw new IndexOutOfRangeException("Index was out of range. Must be non-negative.");
        }
        if (rowCount < 0 || colCount < 0) {
            throw new IndexOutOfRangeException("Size was out of range. Must be non-negative.");
        }
        rowCount = Math.Min(arr.RowCount() - row, rowCount);
        colCount = Math.Min(arr.ColumnCount() - col, colCount);

        var result = new T[rowCount, colCount];
        for (int i = row; i < row + rowCount; i++) {
            for (int j = col; j < col + colCount; j++) {
                result[i - row, j - col] = arr[i, j];
            }
        }

        return result;
    }

    public static T[,] Extract<T>(this T[,] arr, Range rows, Range cols) {
        (int row, int rowCount) = rows.GetOffsetAndLength(arr.RowCount());
        (int col, int colCount) = cols.GetOffsetAndLength(arr.ColumnCount());
        return arr.Extract(row, col, rowCount, colCount);
    }

    public static IEnumerable<T[]> ExtractLine<T>(this T[,] arr, int row, int col, Directions dirs, int length) {
        foreach (Direction dir in dirs.Enumerate()) {
            var result = arr.ExtractLine(row, col, dir, length);
            if (result.Length > 0) {
                yield return result;
            }
        }
    }

    public static T[] ExtractLine<T>(this T[,] arr, int row, int col, Direction dir, int length) {
        if (row < 0 || col < 0 || dir == Direction.Origin || length == 0) {
            return Array.Empty<T>();
        }

        (int dX, int dY) = dir.Delta();
        var result = new T[length];
        bool ok = true;
        for (int i = 0; i < length; i++) {
            if (!arr.TryGet(row + dX * i, col + dY * i, out T? value)) {
                ok = false;
                break;
            }

            result[i] = value;
        }

        return ok ? result : Array.Empty<T>();
    }

    public static T[,] Rotate<T>(this T[,] arr, int n = 1) {
        n = (n + 4) % 4; // 0, 1, 2, 3 rotations only

        T[,] result = arr;
        while (n > 0) {
            int rows = result.ColumnCount();
            int cols = result.RowCount();
            var next = new T[rows, cols];
            for (int i = 0; i < rows; i++) {
                for (int j = 0; j < cols; j++) {
                    next[i, j] = result[cols - j - 1, i];
                }
            }

            result = next;
            n--;
        }

        return result;
    }

    public static T[,] Reflect<T>(this T[,] arr, Axis axis = Axis.Horizontal) {
        int rows = arr.RowCount();
        int cols = arr.ColumnCount();
        var result = new T[rows, cols];
        switch (axis) {
            case Axis.Horizontal:
                for (int i = 0; i < rows; i++) {
                    for (int j = 0; j < cols; j++) {
                        result[i, j] = arr[i, cols - j - 1];
                    }
                }

                return result;
            case Axis.Vertical:
                for (int i = 0; i < rows; i++) {
                    for (int j = 0; j < cols; j++) {
                        result[i, j] = arr[rows - i - 1, j];
                    }
                }

                return result;
            default:
                throw new ArgumentException($"Invalid axis: {axis}");
        }
    }

    public static IEnumerable<Direction> Enumerate(this Directions dirs) {
        foreach (Direction dir in Offsets.Keys) {
            if ((dirs & (Directions)dir) == (Directions)dir) {
                yield return dir;
            }
        }
    }

    public static IEnumerable<(int, int)> Deltas(this Directions dir) {
        return dir.Enumerate()
            .Select(d => Offsets[d]);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static (int, int) Delta(this Direction dir) {
        return Offsets[dir];
    }

    public static Directions Rotate(this Directions dir, int degrees) {
        return dir.Enumerate()
            .Select(d => (Directions) d.Rotate(degrees))
            .Aggregate((a, b) => a | b);
    }

    public static Direction Rotate(this Direction dir, int degrees) {
        if (degrees % 45 != 0) {
            throw new ArgumentException("Rotation must be a multiple of 45");
        }
        if (dir == Direction.Origin) {
            return Direction.Origin;
        }

        int index = Array.IndexOf(Rotations, dir);
        if (index == -1) {
            throw new ArgumentException($"Invalid direction: {dir}");
        }

        int steps = (degrees % 360 + 360) / 45;
        return Rotations[(index + steps) % Rotations.Length];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<(int Row, int Col)> Adjacent<T>(this T[,] arr, (int Row, int Col) point, Directions dir, AdjacencyOptions options = AdjacencyOptions.None) {
        return arr.Adjacent(point.Row, point.Col, dir, options);
    }

    public static IEnumerable<(int Row, int Col)> Adjacent<T>(this T[,] arr, int row, int col, Directions dirs, AdjacencyOptions options = AdjacencyOptions.None) {
        foreach (Direction dir in dirs.Enumerate()) {
            (int Row, int Col) adjacent = arr.Adjacent(row, col, dir, options);
            if (adjacent != (-1, -1)) {
                yield return adjacent;
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static (int Row, int Col) Adjacent<T>(this T[,] arr, (int Row, int Col) point, Direction dir, AdjacencyOptions options = AdjacencyOptions.None) {
        return arr.Adjacent(point.Row, point.Col, dir, options);
    }

    public static (int Row, int Col) Adjacent<T>(this T[,] arr, int row, int col, Direction dir, AdjacencyOptions options = AdjacencyOptions.None) {
        (int dX, int dY) = dir.Delta();
        int r = row + dX;
        int c = col + dY;
        if ((options & AdjacencyOptions.Wrap) == AdjacencyOptions.Wrap) {
            int rows = arr.RowCount();
            r = (r + rows) % rows;

            int cols = arr.ColumnCount();
            c = (c + cols) % cols;
        }

        if (arr.TryGet(r, c, out T? _) || (options & AdjacencyOptions.Expand) == AdjacencyOptions.Expand) {
            return (r, c);
        }

        return (-1, -1);
    }

    public static TR[,] Select<T, TR>(this T[,] items, Func<T, TR> f) {
        int rows = items.RowCount();
        int cols = items.ColumnCount();
        var result = new TR[rows, cols];
        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < cols; j++) {
                result[i, j] = f(items[i, j]);
            }
        }

        return result;
    }

    public static IEnumerable<T> Flatten<T>(this T[,] items) {
        return items.Cast<T>();
    }

    public static TR[,] Compose<T1, T2, TR>(this T1[,] arr1, T2[,] arr2, Func<T1, T2, TR> f) {
        int rows = arr1.RowCount();
        int cols = arr1.ColumnCount();
        if (rows != arr2.RowCount() || cols != arr2.ColumnCount()) {
            throw new ArgumentException("Cannot compose arrays of different dimensions.");
        }

        var result = new TR[rows, cols];
        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < cols; j++) {
                result[i, j] = f(arr1[i, j], arr2[i, j]);
            }
        }

        return result;
    }

    public static T[,] Resize<T>(this T[,] arr, uint addRows, uint addCols) {
        int rows = arr.RowCount();
        int cols = arr.ColumnCount();
        var result = new T[rows + addRows, cols + addCols];
        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < cols; j++) {
                result[i, j] = arr[i, j];
            }
        }

        return result;
    }

    public static T[][] Jagged<T>(this T[,] arr) {
        int cols = arr.GetLength(0);
        var result = new T[cols][];
        for (int i = 0; i < cols; i++) {
            result[i] = arr.GetRow(i);
        }

        return result;
    }

    public static T[,] UnJagged<T>(this T[][] arr, bool expand = false) {
        int rows = arr.Length;
        int cols = arr[0].Length;
        if (expand) {
            cols = arr.Select(row => row.Length).Max();
        }

        var result = new T[rows, cols];
        for (int i = 0; i < rows; i++) {
            if (!expand && arr[i].Length != cols) {
                throw new IndexOutOfRangeException("Jagged array cannot be converted without expanding.");
            }
            for (int j = 0; j < cols && j < arr[i].Length; j++) {
                result[i, j] = arr[i][j];
            }
        }

        return result;
    }

    public static BidirectionalGraph<(int R, int C), TaggedEdge<(int, int), (T, T)>> AsGraph<T>(this T[,] arr) {
        var graph = new BidirectionalGraph<(int, int), TaggedEdge<(int, int), (T, T)>>();

        int rows = arr.RowCount();
        int cols = arr.ColumnCount();
        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < cols; j++) {
                graph.AddVertex((i, j));
            }
        }

        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < cols; j++) {
                foreach ((int r, int c) in arr.Adjacent(i, j, Directions.Cardinal)) {
                    graph.AddEdge(new TaggedEdge<(int, int), (T, T)>((i, j), (r, c), (arr[i, j], arr[r, c])));
                }
            }
        }

        return graph;
    }

    public static AdjacencyGraph<(int R, int C), TaggedEdge<(int, int), (T, T)>> AsGraph<T>(this T[,] arr, Func<TaggedEdge<(int, int), (T, T)>, bool> addEdgeFunc) {
        var graph = new AdjacencyGraph<(int, int), TaggedEdge<(int, int), (T, T)>>();

        int rows = arr.RowCount();
        int cols = arr.ColumnCount();
        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < cols; j++) {
                graph.AddVertex((i, j));
            }
        }

        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < cols; j++) {
                foreach ((int r, int c) in arr.Adjacent(i, j, Directions.Cardinal)) {
                    var edge = new TaggedEdge<(int, int), (T, T)>((i, j), (r, c), (arr[i, j], arr[r, c]));
                    if (addEdgeFunc(edge)) {
                        graph.AddEdge(edge);
                    }
                }
            }
        }

        return graph;
    }

    public static string PrettyString<T>(this T[,] arr, string delimiter = " ") {
        var builder = new StringBuilder();
        for (int i = 0; i < arr.RowCount(); i++) {
            builder.AppendLine(string.Join(delimiter, arr.GetRow(i)));
        }

        return builder.ToString();
    }

    public static IEnumerable<T> ToEnumerable<T>(this T[,] arr) {
        foreach (T item in arr) {
            yield return item;
        }
    }

    public static int Sum(this int[,] items) {
        int rows = items.RowCount();
        int cols = items.ColumnCount();
        int sum = 0;
        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < cols; j++) {
                sum += items[i, j];
            }
        }

        return sum;
    }

    public static IEnumerable<T> Where<T>(this T[,] arr, Func<T, bool> predicate) {
        int rows = arr.RowCount();
        int cols = arr.ColumnCount();
        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < cols; j++) {
                if (predicate(arr[i, j])) {
                    yield return arr[i, j];
                }
            }
        }
    }

    public static IEnumerable<(int Row, int Col)> Find<T>(this T[,] arr, T value) {
        int rows = arr.RowCount();
        int cols = arr.ColumnCount();
        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < cols; j++) {
                if (EqualityComparer<T>.Default.Equals(arr[i, j], value)) {
                    yield return (i, j);
                }
            }
        }
    }

    public static IEnumerable<(int Row, int Col)> Find<T>(this T[,] arr, Func<T, bool> predicate) {
        int rows = arr.RowCount();
        int cols = arr.ColumnCount();
        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < cols; j++) {
                if (predicate(arr[i, j])) {
                    yield return (i, j);
                }
            }
        }
    }
}
