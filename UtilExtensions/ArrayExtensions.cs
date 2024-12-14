using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace UtilExtensions;

public static partial class ArrayExtensions {
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

    public static IEnumerable<T> Convert<T>(this IEnumerable<string> arr) where T : IConvertible {
        return arr.Select(i => (T) System.Convert.ChangeType(i, typeof(T)));
    }

    public static T[] Copy<T>(this T[] arr) {
        var result = new T[arr.Length];
        Array.Copy(arr, result, arr.Length);

        return result;
    }
}
