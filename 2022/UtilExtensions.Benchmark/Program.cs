using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace UtilExtensions.Benchmark;

public static class Program {
    public static void Main(string[] args) {
        var stringBenchmark = BenchmarkRunner.Run<Benchmark>();
        Console.WriteLine(stringBenchmark);
    }
}

public class Benchmark {
    [Params(100)]
    public int N;

    [Benchmark]
    public void Product() {
        int[] array1 = {1, 3, 5};
        int[] array2 = {2, 4, 6};
        int[] array3 = {7, 8};

        foreach (var entry in Itertools.Product(array1, array2, array3)) {
            // Console.WriteLine(entry.PrettyString());
        }
    }

    [Benchmark]
    public void Product2() {
        int[] array1 = {1, 3, 5};
        int[] array2 = {2, 4, 6};
        int[] array3 = {7, 8};
        //
        // foreach (var entry in Itertools.Product2(array1, array2, array3)) {
        //     // Console.WriteLine(entry.PrettyString());
        // }
    }
}
