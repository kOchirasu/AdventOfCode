using System;
using System.Linq;
using System.Text;

namespace UtilExtensions {
    public static class ArrayExtensions {
        public static int Rows<T>(this T[,] arr) => arr.GetLength(0);
        public static int Columns<T>(this T[,] arr) => arr.GetLength(1);
        
        public static bool TryGet<T>(this T[,] arr, int row, int col, out T value) {
            if (row < 0 || col < 0 || row >= arr.GetLength(0) || col >= arr.GetLength(1)) {
                value = default;
                return false;
            }

            value = arr[row, col];
            return true;
        }

        public static T[] GetColumn<T>(this T[,] arr, int col) {
            int length = arr.GetLength(0);
            
            var result = new T[length]; 
            for (int i = 0; i < length; i++) {
                result[i] = arr[i, col];
            }

            return result;
        }

        public static T[] GetRow<T>(this T[,] arr, int row) {
            int length = arr.GetLength(1);
            
            var result = new T[length]; 
            for (int i = 0; i < length; i++) {
                result[i] = arr[row, i];
            }

            return result;
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

        public static string PrettyString<T>(this T[,] arr, string delimiter = " ") {
            var builder = new StringBuilder();
            for (int i = 0; i < arr.Rows(); i++) {
                builder.AppendLine(string.Join(delimiter, arr.GetRow(i)));
            }

            return builder.ToString();
        }
    }
}
