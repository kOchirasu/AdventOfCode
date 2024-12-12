using System;

namespace UtilExtensions;

public static class MathUtils {
    /// <summary>
    /// Computes the Least Common Multiple LCM of an array of numbers.
    /// </summary>
    /// <param name="nums">Array of numbers to compute</param>
    /// <returns>Least Common Multiple</returns>
    public static int Lcm(params int[] nums) {
        if (nums.Length <= 0) {
            return 0;
        }

        int lcm = nums[0];
        for (int i = 1; i < nums.Length; i++) {
            lcm = lcm * nums[i] / FindGcd(lcm, nums[i]);
        }

        return lcm;
    }

    /// <summary>
    /// Computes the Greatest Common Divisor (Factor) GCD/GCF of an array of numbers.
    /// </summary>
    /// <param name="nums">Array of numbers to compute</param>
    /// <returns>Greatest Common Divisor</returns>
    public static int Gcd(params int[] nums) {
        if (nums.Length <= 0) {
            return 0;
        }

        int gcf = nums[0];
        for (int i = 1; i < nums.Length; i++) {
            gcf = FindGcd(gcf, nums[i]);
        }

        return gcf;
    }

    // This helper function computes the greatest common divisor of two numbers using the Euclidean algorithm.
    private static int FindGcd(int a, int b) {
        while (b != 0) {
            (a, b) = (b, a % b);
        }
        return a;
    }

    public static int ManhattanDistance(this (int, int) p1, (int, int) p2) {
        return Math.Abs(p1.Item1 - p2.Item1) + Math.Abs(p1.Item2 - p2.Item2);
    }

    public static (int dX, int dY) ManhattanDelta(this (int, int) p1, (int, int) p2) {
        return (p1.Item1 - p2.Item1, p1.Item2 - p2.Item2);
    }
}
