namespace Day7;

// https://adventofcode.com/
public static class Program {
    public static void Main() {
        string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");

        var equations = new List<(long, long[])>();
        foreach (string row in File.ReadAllLines(file)) {
            string[] parts = row.Split(": ");
            long target = long.Parse(parts[0]);
            long[] nums = parts[1].Split(" ").Select(long.Parse).ToArray();

            equations.Add((target, nums));
        }

        Console.WriteLine(Part1(equations));
        Console.WriteLine(Part2(equations));
    }

    private static bool Solve1(Span<long> nums, long target, long current) {
        if (nums.IsEmpty) {
            return current == target;
        }

        return Solve1(nums[1..], target, current + nums[0])
               || Solve1(nums[1..], target, current * nums[0]);
    }

    private static long Part1(List<(long Target, long[] Nums)> equations) {
        long result = 0;
        foreach ((long target, long[] nums) in equations) {
            if (Solve1(nums.AsSpan()[1..], target, nums[0])) {
                result += target;
            }
        }

        return result;
    }

    private static bool Solve2(Span<long> nums, long target, long current) {
        if (nums.IsEmpty) {
            return current == target;
        }

        return Solve2(nums[1..], target, current + nums[0])
               || Solve2(nums[1..], target, current * nums[0])
               || Solve2(nums[1..], target, long.Parse($"{current}{nums[0]}"));
    }

    private static long Part2(List<(long Target, long[] Nums)> equations) {
        long result = 0;
        foreach ((long target, long[] nums) in equations) {
            if (Solve2(nums.AsSpan()[1..], target, nums[0])) {
                result += target;
            }
        }

        return result;
    }
}
