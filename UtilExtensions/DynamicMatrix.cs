using System.Collections;
using System.Collections.Generic;

namespace UtilExtensions;

public class DynamicMatrix<T> : IEnumerable<T> {
    private int originRow;
    private int originCol;

    public T[,] Value { get; private set; }
    public readonly T Default;

    private readonly bool expandOnAccess;

    public DynamicMatrix(T[,] matrix, T @default = default(T), bool expandOnAccess = false) {
        Value = matrix;
        Default = @default;
        originRow = 0;
        originCol = 0;

        this.expandOnAccess = expandOnAccess;
    }

    public DynamicMatrix(int rows = 0, int cols = 0, T @default = default(T), bool expandOnAccess = false) : this(new T[rows, cols], @default, expandOnAccess) { }

    public static implicit operator DynamicMatrix<T>(T[,] matrix) {
        return new DynamicMatrix<T>(matrix);
    }

    public T this[int row, int col] {
        get {
            if (!expandOnAccess) {
                return Value[row + originRow, col + originCol];
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
            originRow = originRow,
            originCol = originCol,
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
        int computedRow = row + originRow;
        int computedCol = col + originCol;
        int rowCount = Value.RowCount();
        int colCount = Value.ColumnCount();

        if (computedRow >= 0 && computedCol >= 0 && computedRow < rowCount && computedCol < colCount) {
            return (computedRow, computedCol);
        }

        int expandRow = 0;
        int offsetRow = 0;
        if (computedRow < 0) {
            offsetRow = -computedRow;
            expandRow = -computedRow;
        } else if (computedRow >= rowCount) {
            expandRow = computedRow - rowCount + 1;
        }

        int expandCol = 0;
        int offsetCol = 0;
        if (computedCol < 0) {
            offsetCol = -computedCol;
            expandCol = -computedCol;
        } else if (computedCol >= colCount) {
            expandCol = computedCol - colCount + 1;
        }

        var newMatrix = new T[rowCount + expandRow, colCount + expandCol];
        newMatrix.Fill(Default);
        originRow += offsetRow;
        originCol += offsetCol;

        newMatrix.Insert(Value, offsetRow, offsetCol);
        Value = newMatrix;
        return (computedRow + offsetRow, computedCol + offsetCol);
    }
}
