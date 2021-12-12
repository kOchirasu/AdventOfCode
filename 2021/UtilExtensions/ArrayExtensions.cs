using System;
using System.Linq;
using System.Text;

namespace UtilExtensions {
    public static class ArrayExtensions {
        public enum Axis {
            Horizontal,
            Vertical,
        }
        
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
        
        public static T[] Shift<T>(this T[] arr, int n, T extend = default) {
            n %= arr.Length;
            (int src, int dst) shift = default;
            int fill = 0;
            switch (n) {
                case < 0: // Shift Left
                    n *= -1;
                    shift.src = n;
                    fill = n - 1;
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
                    wrap.dst = n - 1;
                    break;
                case > 0: // CircularShift Right
                    shift.dst = n;
                    wrap.src = n - 1;
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
