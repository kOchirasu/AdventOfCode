using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using static UtilExtensions.ArrayExtensions;

namespace UtilExtensions;

public sealed class DynamicMatrix<T> : IEnumerable<T> {
    private Point offset;

    public T[,] Value { get; private set; }
    public readonly T Default;

    private readonly bool expandOnAccess;

    public int StartRow => -offset.Row;
    public int EndRow => Value.RowCount() - 1 - offset.Row;
    public int StartCol => -offset.Col;
    public int EndCol => Value.ColumnCount() - 1 - offset.Col;

    public DynamicMatrix(T[,] matrix, T @default = default(T), bool expandOnAccess = false) {
        Value = matrix;
        Default = @default;
        offset = new Point(0, 0);

        this.expandOnAccess = expandOnAccess;
    }

    public DynamicMatrix(int rows = 0, int cols = 0, T @default = default(T), bool expandOnAccess = false) : this(new T[rows, cols], @default, expandOnAccess) { }

    public static implicit operator DynamicMatrix<T>(T[,] matrix) {
        return new DynamicMatrix<T>(matrix);
    }

    public Point Normalize(int row, int col) => new(row + offset.Row, col + offset.Col);
    public Point Normalize(Point pos) => pos + offset;

    public T this[Point p] {
        get => this[p.Row, p.Col];
        set => this[p.Row, p.Col] = value;
    }

    public T this[int row, int col] {
        get {
            if (!expandOnAccess) {
                return Value[row + offset.Row, col + offset.Col];
            }

            Point normalize = EnsureSize(row, col);
            return Value[normalize.Row, normalize.Col];
        }
        set {
            Point normalize = EnsureSize(row, col);
            Value[normalize.Row, normalize.Col] = value;
        }
    }

    public T[] GetRow(int row) {
        return Value.GetRow(row + offset.Row);
    }

    public T[] GetColumn(int col) {
        return Value.GetColumn(col + offset.Col);
    }

    public DynamicMatrix<T> Copy() {
        return new DynamicMatrix<T>(Value.Copy(), Default, expandOnAccess) {
            offset = offset,
        };
    }

    public override string ToString() {
        return Value.PrettyString();
    }

    public string PrettyString(string delimiter = " ") {
        return Value.PrettyString(delimiter);
    }

    public IEnumerator<T> GetEnumerator() {
        return Value.ToEnumerable().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }

    private Point EnsureSize(int row, int col) {
        Point computed = Normalize(row, col);
        int rowCount = Value.RowCount();
        int colCount = Value.ColumnCount();

        if (computed.Row >= 0 && computed.Col >= 0 && computed.Row < rowCount && computed.Col < colCount) {
            return computed;
        }

        int expandRow = 0;
        var addOffset = new Point(0, 0);
        if (computed.Row < 0) {
            addOffset.Row = -computed.Row;
            expandRow = -computed.Row;
        } else if (computed.Row >= rowCount) {
            expandRow = computed.Row - rowCount + 1;
        }

        int expandCol = 0;
        if (computed.Col < 0) {
            addOffset.Col = -computed.Col;
            expandCol = -computed.Col;
        } else if (computed.Col >= colCount) {
            expandCol = computed.Col - colCount + 1;
        }

        var newMatrix = new T[rowCount + expandRow, colCount + expandCol];
        newMatrix.Fill(Default);
        offset += addOffset;

        newMatrix.Insert(Value, addOffset.Row, addOffset.Col);
        Value = newMatrix;
        return computed + addOffset;
    }

    public void Insert(T[,] insert, int row, int col) {
        Point normalize = EnsureSize(row, col);
        EnsureSize(row + insert.RowCount() - 1, col + insert.ColumnCount() - 1);
        Value.Insert(insert, normalize.Row, normalize.Col);
    }

    public void ConditionalInsert(T[,] insert, int row, int col, Func<T, bool> shouldInsert) {
        Point normalize = EnsureSize(row, col);
        EnsureSize(row + insert.RowCount() - 1, col + insert.ColumnCount() - 1);
        Value.ConditionalInsert(insert, normalize.Row, normalize.Col, shouldInsert);
    }

    public T[,] Extract(int row, int col, int rowCount, int colCount) {
        Point normalize = Normalize(row, col);
        return Value.Extract(normalize.Row, normalize.Col, rowCount, colCount);
    }

    public DynamicMatrix<T> Rotate(int n = 1) {
        n = (n + 4) % 4; // 0, 1, 2, 3 rotations only
        Point newOffset = offset;
        for (int i = 0; i < n; i++) {
            newOffset = new Point(newOffset.Col, -newOffset.Row);
        }

        T[,] rotated = Value.Rotate(n);
        int rowWrap = rotated.RowCount() - 1;
        int colWrap = rotated.ColumnCount() - 1;
        return new DynamicMatrix<T>(rotated, Default, expandOnAccess) {
            offset = new Point((newOffset.Row + rowWrap) % rowWrap, (newOffset.Col + colWrap) % colWrap),
        };
    }

    public DynamicMatrix<T> Reflect(Axis axis = Axis.Horizontal) {
        int offsetRow = axis switch {
            Axis.Horizontal => offset.Row,
            Axis.Vertical => EndRow,
            _ => throw new ArgumentException($"Invalid Axis: {axis}"),
        };
        int offsetCol = axis switch {
            Axis.Horizontal => EndCol,
            Axis.Vertical => offset.Col,
            _ => throw new ArgumentException($"Invalid Axis: {axis}"),
        };

        return new DynamicMatrix<T>(Value.Reflect(axis), Default, expandOnAccess) {
            offset = new Point(offsetRow, offsetCol),
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Point[] Adjacent(Point point, Directions dir, AdjacencyOptions options = AdjacencyOptions.None) {
        return Adjacent(point.Row, point.Col, dir, options);
    }

    // We intentionally force enumeration here because DynamicMatrix may grow while enumerating.
    public Point[] Adjacent(int row, int col, Directions dir, AdjacencyOptions options = AdjacencyOptions.None) {
        Point normalize = Normalize(row, col);
        return Value.Adjacent(normalize, dir, options)
            .Select(i => i - offset)
            .ToArray();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Point Adjacent(Point point, Direction dir, AdjacencyOptions options = AdjacencyOptions.None) {
        return Adjacent(point.Row, point.Col, dir, options);
    }

    public Point Adjacent(int row, int col, Direction dir, AdjacencyOptions options = AdjacencyOptions.None) {
        Point normalize = Normalize(row, col);
        Point result = Value.Adjacent(normalize, dir, options);
        return result - offset;
    }

    // We intentionally force enumeration here because DynamicMatrix may grow while enumerating.
    public Point[] Find(T value) {
        return Value.Find(value)
            .Select(i => i - offset)
            .ToArray();
    }

    // We intentionally force enumeration here because DynamicMatrix may grow while enumerating.
    public Point[] Find(Func<T, bool> predicate) {
        return Value.Find(predicate)
            .Select(i => i - offset)
            .ToArray();
    }
}
