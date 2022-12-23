using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using QuikGraph;

namespace UtilExtensions;

public static class ArrayExtensions {
    public enum Axis {
        Horizontal,
        Vertical,
    }

    [Flags]
    public enum Directions {
        Origin = 1,
        N = 2,
        E = 4,
        S = 8,
        W = 16,
        NE = 32,
        SE = 64,
        SW = 128,
        NW = 256,
        Wrap = 512,
        Expand = 1024,

        Cardinal = N | E | S | W,
        Intermediate = NE | SE | SW | NW,
        All = Cardinal | Intermediate,
    }

    private static readonly (Directions, int, int)[] Offsets = {
        (Directions.N, -1, 0),
        (Directions.E, 0, 1),
        (Directions.S, 1, 0),
        (Directions.W, 0, -1),
        (Directions.NE, -1, 1),
        (Directions.SE, 1, 1),
        (Directions.SW, 1, -1),
        (Directions.NW, -1, -1),
    };

    public static int RowCount<T>(this T[,] arr) => arr.GetLength(0);
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

    public static bool TryGet<T>(this T[,] arr, int row, int col, [MaybeNullWhen(false)] out T value) {
        if (row < 0 || col < 0 || row >= arr.GetLength(0) || col >= arr.GetLength(1)) {
            value = default;
            return false;
        }

        value = arr[row, col];
        return true;
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

    public static T[] GetColumn<T>(this T[,] arr, int col) {
        int length = arr.GetLength(0);

        var result = new T[length];
        for (int i = 0; i < length; i++) {
            result[i] = arr[i, col];
        }

        return result;
    }

    public static void SetColumn<T>(this T[,] arr, int col, T[] data) {
        int cols = arr.ColumnCount();
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

    public static T[] GetRow<T>(this T[,] arr, int row) {
        int length = arr.GetLength(1);

        var result = new T[length];
        for (int i = 0; i < length; i++) {
            result[i] = arr[row, i];
        }

        return result;
    }

    public static void SetRow<T>(this T[,] arr, int row, T[] data) {
        int rows = arr.RowCount();
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

    public static void Fill<T>(this T[,] arr, T value) {
        int rows = arr.RowCount();
        int cols = arr.ColumnCount();
        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < cols; j++) {
                arr[i, j] = value;
            }
        }
    }

    public static T[,] Clone<T>(this T[,] arr, int row = 0, int col = 0, int rows = int.MaxValue, int cols = int.MaxValue) {
        if (row < 0 || col < 0) {
            throw new IndexOutOfRangeException("Index was out of range. Must be non-negative.");
        }
        if (rows < 0 || cols < 0) {
            throw new IndexOutOfRangeException("Size was out of range. Must be non-negative.");
        }
        rows = Math.Min(arr.RowCount() - row, rows);
        cols = Math.Min(arr.ColumnCount() - col, cols);

        var result = new T[rows, cols];
        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < cols; j++) {
                result[i, j] = arr[i + row, j + col];
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

    public static T[,] Extract<T>(this T[,] arr, int row, int col, int rowCount, int colCount) {
        var result = new T[rowCount, colCount];
        for (int i = row; i < row + rowCount; i++) {
            for (int j = col; j < col + colCount; j++) {
                result[i - row, j - col] = arr[i, j];
            }
        }

        return result;
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

    public static IEnumerable<(int, int)> Deltas(this Directions dir) {
        foreach ((Directions d, int dX, int dY) in Offsets) {
            if ((dir & d) == 0) continue;

            yield return (dX, dY);
        }
    }

    public static IEnumerable<(int, int)> Adjacent<T>(this T[,] arr, int row, int col, Directions dir) {
        if ((dir & Directions.Origin) != 0) {
            yield return (row, col);
        }

        int rows = arr.RowCount();
        int cols = arr.ColumnCount();
        bool wrap = (dir & Directions.Wrap) != 0;
        bool expand = (dir & Directions.Expand) != 0;
        foreach ((int dX, int dY) in dir.Deltas()) {
            int r = row + dX;
            int c = col + dY;
            if (wrap) {
                r = (r + rows) % rows;
                c = (c + cols) % cols;
            }

            if (arr.TryGet(r, c, out T? _) || expand) {
                yield return (r, c);
            }
        }
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
