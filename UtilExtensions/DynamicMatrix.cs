using System;
using System.Collections;
using System.Collections.Generic;
using static UtilExtensions.ArrayExtensions;

namespace UtilExtensions;

public sealed class DynamicMatrix<T> : IEnumerable<T> {
    private int offsetRow;
    private int offsetCol;

    public T[,] Value { get; private set; }
    public readonly T Default;

    private readonly bool expandOnAccess;

    public int StartRow => -offsetRow;
    public int EndRow => Value.RowCount() - 1 - offsetRow;
    public int StartCol => -offsetCol;
    public int EndCol => Value.ColumnCount() - 1 - offsetCol;

    public DynamicMatrix(T[,] matrix, T @default = default(T), bool expandOnAccess = false) {
        Value = matrix;
        Default = @default;
        offsetRow = 0;
        offsetCol = 0;

        this.expandOnAccess = expandOnAccess;
    }

    public DynamicMatrix(int rows = 0, int cols = 0, T @default = default(T), bool expandOnAccess = false) : this(new T[rows, cols], @default, expandOnAccess) { }

    public static implicit operator DynamicMatrix<T>(T[,] matrix) {
        return new DynamicMatrix<T>(matrix);
    }

    public (int Row, int Col) Normalize(int row, int col) => (row + offsetRow, col + offsetCol);
    public (int Row, int Col) Normalize((int Row, int Col) pos) => (pos.Row + offsetRow, pos.Col + offsetCol);

    public T this[int row, int col] {
        get {
            if (!expandOnAccess) {
                return Value[row + offsetRow, col + offsetCol];
            }

            (int normalizeRow, int normalizeCol) = EnsureSize(row, col);
            return Value[normalizeRow, normalizeCol];
        }
        set {
            (int normalizeRow, int normalizeCol) = EnsureSize(row, col);
            Value[normalizeRow, normalizeCol] = value;
        }
    }

    public T[] GetRow(int row) {
        return Value.GetRow(row + offsetRow);
    }

    public T[] GetColumn(int col) {
        return Value.GetColumn(col + offsetCol);
    }

    public DynamicMatrix<T> Copy() {
        return new DynamicMatrix<T>(Value.Copy(), Default, expandOnAccess) {
            offsetRow = offsetRow,
            offsetCol = offsetCol,
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

    private (int, int) EnsureSize(int row, int col) {
        (int computedRow, int computedCol) = Normalize(row, col);
        int rowCount = Value.RowCount();
        int colCount = Value.ColumnCount();

        if (computedRow >= 0 && computedCol >= 0 && computedRow < rowCount && computedCol < colCount) {
            return (computedRow, computedCol);
        }

        int expandRow = 0;
        int addOffsetRow = 0;
        if (computedRow < 0) {
            addOffsetRow = -computedRow;
            expandRow = -computedRow;
        } else if (computedRow >= rowCount) {
            expandRow = computedRow - rowCount + 1;
        }

        int expandCol = 0;
        int addOffsetCol = 0;
        if (computedCol < 0) {
            addOffsetCol = -computedCol;
            expandCol = -computedCol;
        } else if (computedCol >= colCount) {
            expandCol = computedCol - colCount + 1;
        }

        var newMatrix = new T[rowCount + expandRow, colCount + expandCol];
        newMatrix.Fill(Default);
        offsetRow += addOffsetRow;
        offsetCol += addOffsetCol;

        newMatrix.Insert(Value, addOffsetRow, addOffsetCol);
        Value = newMatrix;
        return (computedRow + addOffsetRow, computedCol + addOffsetCol);
    }

    public void Insert(T[,] insert, int row, int col) {
        (int normalizeRow, int normalizeCol) = EnsureSize(row, col);
        EnsureSize(row + insert.RowCount() - 1, col + insert.ColumnCount() - 1);
        Value.Insert(insert, normalizeRow, normalizeCol);
    }

    public void ConditionalInsert(T[,] insert, int row, int col, Func<T, bool> shouldInsert) {
        (int normalizeRow, int normalizeCol) = EnsureSize(row, col);
        EnsureSize(row + insert.RowCount() - 1, col + insert.ColumnCount() - 1);
        Value.ConditionalInsert(insert, normalizeRow, normalizeCol, shouldInsert);
    }

    public T[,] Extract(int row, int col, int rowCount, int colCount) {
        (int normalizeRow, int normalizeCol) = Normalize(row, col);
        return Value.Extract(normalizeRow, normalizeCol, rowCount, colCount);
    }

    public DynamicMatrix<T> Rotate(int n = 1) {
        n = (n + 4) % 4; // 0, 1, 2, 3 rotations only
        (int newOffsetRow, int newOffsetCol) = (offsetRow, offsetCol);
        for (int i = 0; i < n; i++) {
            (newOffsetRow, newOffsetCol) = (newOffsetCol, -newOffsetRow);
        }

        T[,] rotated = Value.Rotate(n);
        int rowWrap = rotated.RowCount() - 1;
        int colWrap = rotated.ColumnCount() - 1;
        return new DynamicMatrix<T>(rotated, Default, expandOnAccess) {
            offsetRow = (newOffsetRow + rowWrap) % rowWrap,
            offsetCol = (newOffsetCol + colWrap) % colWrap,
        };
    }

    public DynamicMatrix<T> Reflect(Axis axis = Axis.Horizontal) {
        return new DynamicMatrix<T>(Value.Reflect(axis), Default, expandOnAccess) {
            offsetRow = axis switch {
                Axis.Horizontal => offsetRow,
                Axis.Vertical => Value.RowCount() - 1 - offsetRow,
                _ => throw new ArgumentException($"Invalid Axis: {axis}")
            },
            offsetCol = axis switch {
                Axis.Horizontal => Value.ColumnCount() - 1 - offsetCol,
                Axis.Vertical => offsetCol,
                _ => throw new ArgumentException($"Invalid Axis: {axis}")
            },
        };
    }

    public IEnumerable<(int, int)> Adjacent(int row, int col, Directions dir) {
        (int normalizeRow, int normalizeCol) = Normalize(row, col);
        foreach ((int r, int c) in Value.Adjacent(normalizeRow, normalizeCol, dir)) {
            yield return (r - offsetRow, c - offsetCol);
        }
    }

    public IEnumerable<(int Row, int Col)> Find(T value) {
        foreach ((int r, int c) in Value.Find(value)) {
            yield return (r - offsetRow, c - offsetCol);
        }
    }

    public IEnumerable<(int Row, int Col)> Find(Func<T, bool> predicate) {
        foreach ((int r, int c) in Value.Find(predicate)) {
            yield return (r - offsetRow, c - offsetCol);
        }
    }
}
