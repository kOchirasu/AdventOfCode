using System;
using System.Collections;
using System.Collections.Generic;

namespace UtilExtensions;

public class DynamicMatrix<T> : IEnumerable<T> {
    private int offsetRow;
    private int offsetCol;

    public T[,] Value { get; private set; }
    public readonly T Default;

    private readonly bool expandOnAccess;

    public int OriginRow => -offsetRow;
    public int OriginCol => -offsetCol;

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

    public DynamicMatrix<T> Clone() {
        return new DynamicMatrix<T>(Value.Clone(0), Default, expandOnAccess) {
            offsetRow = offsetRow,
            offsetCol = offsetCol,
        };
    }

    public override string ToString() {
        return Value.PrettyString();
    }

    public IEnumerator<T> GetEnumerator() {
        return Value.ToEnumerable().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }

    private (int, int) EnsureSize(int row, int col) {
        int computedRow = row + offsetRow;
        int computedCol = col + offsetCol;
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
}
