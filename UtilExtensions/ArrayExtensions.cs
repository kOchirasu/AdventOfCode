using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        Cardinal = N | E | S | W,
        Intermediate = NE | SE | SW | NW,
        All = Cardinal | Intermediate,
    }

    private static readonly (Directions, int, int)[] Offsets = {
        (Directions.N, 0, -1),
        (Directions.E, 1, 0),
        (Directions.S, 0, 1),
        (Directions.W, -1, 0),
        (Directions.NE, 1, -1),
        (Directions.SE, 1, 1),
        (Directions.SW, -1, 1),
        (Directions.NW, -1, -1),
    };

    public static int Rows<T>(this T[,] arr) => arr.GetLength(0);
    public static int Columns<T>(this T[,] arr) => arr.GetLength(1);

    public static bool TryGet<T>(this T[] arr, int n, out T value) {
        if (n < 0 || n >= arr.Length) {
            value = default;
            return false;
        }

        value = arr[n];
        return true;
    }

    public static T GetOrDefault<T>(this T[] arr, int n, T @default = default) {
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

    public static T[] Shift<T>(this T[] arr, int n, T extend = default) {
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

    public static bool TryGet<T>(this T[,] arr, int row, int col, out T value) {
        if (row < 0 || col < 0 || row >= arr.GetLength(0) || col >= arr.GetLength(1)) {
            value = default;
            return false;
        }

        value = arr[row, col];
        return true;
    }

    public static T GetOrDefault<T>(this T[,] arr, int row, int col, T @default = default) {
        if (row < 0 || col < 0 || row >= arr.GetLength(0) || col >= arr.GetLength(1)) {
            return @default;
        }

        return arr[row, col];
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
        int cols = arr.Columns();
        if (col < 0 || col >= cols) {
            throw new IndexOutOfRangeException("Index was out of range. Must be non-negative and less than the size of the collection");
        }
        int rows = arr.Rows();
        if (rows != data.Length) {
            throw new ArgumentException($"SetColumn with invalid length: {data.Length} != {rows}");
        }

        for (int i = 0; i < rows; i++) {
            arr[i, col] = data[i];
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
        int rows = arr.Rows();
        if (row < 0 || row >= rows) {
            throw new IndexOutOfRangeException("Index was out of range. Must be non-negative and less than the size of the collection");
        }
        int cols = arr.Columns();
        if (cols != data.Length) {
            throw new ArgumentException($"SetColumn with invalid length: {data.Length} != {cols}");
        }

        for (int i = 0; i < cols; i++) {
            arr[row, i] = data[i];
        }
    }

    public static T[,] Clone<T>(this T[,] arr, int row, int col, int rows = int.MaxValue, int cols = int.MaxValue) {
        if (row < 0 || col < 0) {
            throw new IndexOutOfRangeException("Index was out of range. Must be non-negative.");
        }
        if (rows < 0 || cols < 0) {
            throw new IndexOutOfRangeException("Size was out of range. Must be non-negative.");
        }
        rows = Math.Min(arr.Rows() - row, rows);
        cols = Math.Min(arr.Columns() - col, cols);

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

        int rows = arr.Rows();
        int cols = arr.Columns();
        int iRows = insert.Rows();
        int iCols = insert.Columns();
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

    public static T[,] Rotate<T>(this T[,] arr, int n = 1) {
        n = (n + 4) % 4; // 0, 1, 2, 3 rotations only

        T[,] result = arr;
        while (n > 0) {
            int rows = result.Columns();
            int cols = result.Rows();
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
        int rows = arr.Rows();
        int cols = arr.Columns();
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

    public static IEnumerable<(int, int)> Adjacent<T>(this T[,] arr, int row, int col, Directions dir) {
        if ((dir & Directions.Origin) != 0) {
            yield return (row, col);
        }

        int rows = arr.Rows();
        int cols = arr.Columns();
        bool wrap = (dir & Directions.Wrap) != 0;
        foreach ((Directions d, int dX, int dY) in Offsets) {
            if ((dir & d) == 0) continue;

            int r = row + dX;
            int c = col + dY;
            if (wrap) {
                r = (r + rows) % rows;
                c = (c + cols) % cols;
            }

            if (arr.TryGet(r, c, out T _)) {
                yield return (r, c);
            }
        }
    }

    public static TR[,] Select<T, TR>(this T[,] items, Func<T, TR> f) {
        int rows = items.Rows();
        int cols = items.Columns();
        var result = new TR[rows, cols];
        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < cols; j++) {
                result[i, j] = f(items[i, j]);
            }
        }

        return result;
    }

    public static TR[,] Compose<T1, T2, TR>(this T1[,] arr1, T2[,] arr2, Func<T1, T2, TR> f) {
        int rows = arr1.Rows();
        int cols = arr1.Columns();
        if (rows != arr2.Rows() || cols != arr2.Columns()) {
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
        int rows = arr.Rows();
        int cols = arr.Columns();
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

    public static string PrettyString<T>(this IEnumerable<T> arr, string delimiter = " ") {
        return string.Join(delimiter, arr);
    }

    public static string PrettyString<T>(this T[,] arr, string delimiter = " ") {
        var builder = new StringBuilder();
        for (int i = 0; i < arr.Rows(); i++) {
            builder.AppendLine(string.Join(delimiter, arr.GetRow(i)));
        }

        return builder.ToString();
    }
}
