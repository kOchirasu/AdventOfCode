using System.Numerics;
using UtilExtensions;

namespace Day8;

// https://adventofcode.com/
public static class Program {
    public static void Main() {
        string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");

        Vector3[] input = File.ReadAllText(file).ExtractAll<int>(@"(\d+),(\d+),(\d+)")
            .Select(x => new Vector3(x[0], x[1], x[2]))
            .ToArray();

        Console.WriteLine(Part1(input));
        Console.WriteLine(Part2(input));
    }

    private static bool MergeJunctions(List<HashSet<Vector3>> list, in Vector3 a, in Vector3 b) {
        int indexA = -1;
        int indexB = -1;
        for (int i = 0; i < list.Count; i++) {
            if (list[i].Contains(a)) {
                indexA = i;
            }
            if (list[i].Contains(b)) {
                indexB = i;
            }

            if (indexA >= 0 && indexB >= 0) {
                break;
            }
        }

        if (indexA == indexB) {
            return false;
        }

        // Ensure indexB > indexA so removal is safe.
        if (indexB < indexA) {
            (indexA, indexB) = (indexB, indexA);
        }

        HashSet<Vector3> removed = list[indexB];
        list.RemoveAt(indexB);
        list[indexA].UnionWith(removed);
        return true;
    }

    private static int Part1(Vector3[] input) {
        var list = new List<HashSet<Vector3>>();
        foreach (Vector3 junction in input) {
            var add = new HashSet<Vector3> {junction};
            list.Add(add);
        }

        foreach (Vector3[] combo in input.Combinations(2).OrderBy(x => Vector3.Distance(x[0], x[1])).Take(1000)) {
            MergeJunctions(list, combo[0], combo[1]);
        }

        return list.Select(s => s.Count)
            .OrderByDescending(n => n)
            .Take(3)
            .Aggregate((a, b) => a * b);
    }

    private static long Part2(Vector3[] input) {
        var list = new List<HashSet<Vector3>>();
        foreach (Vector3 junction in input) {
            var add = new HashSet<Vector3> {junction};
            list.Add(add);
        }

        foreach (Vector3[] combo in input.Combinations(2).OrderBy(x => Vector3.Distance(x[0], x[1]))) {
            MergeJunctions(list, combo[0], combo[1]);

            if (list.Count == 1) {
                return (long) combo[0].X * (long) combo[1].X;
            }
        }

        throw new InvalidOperationException("Graph was never fully merged.");
    }
}
