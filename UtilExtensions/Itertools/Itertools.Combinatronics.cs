using System;
using System.Collections.Generic;
using System.Linq;

namespace UtilExtensions;

public static partial class Itertools {
    public static IEnumerable<T[]> Product<T>(params IList<T>[] iterables) {
        int i = 0;
        while (true) {
            var result = new T[iterables.Length];
            int j = i;
            for (int k = iterables.Length - 1; k >= 0; k--) {
                IList<T> iterable = iterables[k];
                result[k] = iterable[j % iterable.Count];
                j /= iterable.Count;
            }
            if (j > 0) {
                yield break;
            }
            yield return result;
            i++;
        }
    }

    public static IEnumerable<T[]> Product<T>(this IList<T> iterable, int repeat = 1) {
        int i = 0;
        while (true) {
            var result = new T[repeat];
            int j = i;
            for (int k = repeat - 1; k >= 0; k--) {
                result[k] = iterable[j % iterable.Count];
                j /= iterable.Count;
            }
            if (j > 0) {
                yield break;
            }
            yield return result;
            i++;
        }
    }

    public static IEnumerable<T[]> Permutations<T>(this IList<T> iterable, int length = 0) {
        length = length <= 0 ? iterable.Count : length;
        if (length > iterable.Count) {
            yield break;
        }

        yield return iterable.Take(length).ToArray();

        int[] indices = Enumerable.Range(0, iterable.Count).ToArray();
        int[] cycles = Enumerable.Range(iterable.Count - length + 1, length).Reverse().ToArray();
        while (true) {
            int i = length - 1;
            for (;i >= 0; i--) {
                cycles[i]--;
                if(cycles[i] == 0) {
                    int temp = indices[i];
                    Array.Copy(indices, i + 1, indices, i, indices.Length - i - 1);
                    indices[^1] = temp;
                    cycles[i] = iterable.Count - i;
                } else {
                    int j = indices.Length - cycles[i];
                    (indices[i], indices[j]) = (indices[j], indices[i]);

                    var result = new T[length];
                    for (int k = 0; k < length; k++) {
                        result[k] = iterable[indices[k]];
                    }
                    yield return result;
                    break;
                }
            }

            // for/else equivalent in python
            if (i < 0) {
                yield break;
            }
        }
    }

    public static IEnumerable<T[]> Combinations<T>(this IList<T> iterable, int length) {
        if (length > iterable.Count) {
            yield break;
        }

        yield return iterable.Take(length).ToArray();

        int delta = iterable.Count - length;
        int[] indices = Enumerable.Range(0, iterable.Count).ToArray();
        while (true) {
            int i = length - 1;
            for (;i >= 0; i--) {
                if (indices[i] != i + delta) {
                    break;
                }
            }

            // for/else equivalent in python
            if (i < 0) {
                yield break;
            }

            indices[i]++;
            for (int j = i + 1; j < length; j++) {
                indices[j] = indices[j - 1] + 1;
            }

            var result = new T[length];
            for (int k = 0; k < length; k++) {
                result[k] = iterable[indices[k]];
            }
            yield return result;
        }
    }

    public static IEnumerable<T[]> CombinationsWithReplacement<T>(this IList<T> iterable, int length) {
        if (iterable.Count == 0 && length == 0) {
            yield break;
        }

        yield return Enumerable.Repeat(iterable[0], length).ToArray();

        int[] indices = new int[length];
        while (true) {
            int i = length - 1;
            for (;i >= 0; i--) {
                if (indices[i] != iterable.Count - 1) {
                    break;
                }
            }

            // for/else equivalent in python
            if (i < 0) {
                yield break;
            }

            Array.Fill(indices, indices[i] + 1, i, length - i);
            var result = new T[length];
            for (int j = 0; j < length; j++) {
                result[j] = iterable[indices[j]];
            }
            yield return result;
        }
    }
}
