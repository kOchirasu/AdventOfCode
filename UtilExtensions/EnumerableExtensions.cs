using System;
using System.Collections.Generic;
using System.Linq;
using QuikGraph;

namespace UtilExtensions;

public static class EnumerableExtensions {
    public static IEnumerable<TResult> TrySelect<T, TResult>(this IEnumerable<T> source, TryFunc<T, TResult> parse) {
        foreach(T element in source) {
            if(parse(element, out TResult value )) {
                yield return value;
            }
        }
    }

    public static IEnumerable<T[]> Window<T>(this IEnumerable<T> enumerable, int size, int step = 1) {
        T[] arr = enumerable.ToArray();
        for (int i = 0; i < arr.Length - size; i += step) {
            var result = new T[size];
            Array.Copy(arr, i, result, 0, size);
            yield return result;
        }
    }

    public static T? ElementAtOrDefault<T>(this IList<T> list, int index, T? fallback = default) {
        if (list == null) {
            throw new ArgumentNullException(nameof(list));
        }

        if (index >= 0 && index < list.Count) {
            return list[index];
        }

        return fallback;
    }

    public static EHashSet<T> ToEHashSet<T>(this IEnumerable<T> enumerable) {
        return new EHashSet<T>(enumerable);
    }

    public static string PrettyString<T>(this IEnumerable<T> arr, string delimiter = " ") {
        return string.Join(delimiter, arr);
    }
}
