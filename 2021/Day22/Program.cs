using System;
using System.IO;
using System.Linq;

namespace Day22 {
    // https://adventofcode.com/
    public static class Program {
        public static void Main() {
            Range ParseRange(string range) {
                var result = range.Substring(2).Split("..").Select(int.Parse).ToArray();
                return new Range(result[0], result[1]);
            }

            string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");
            var input = File.ReadAllLines(file).Select(row => {
                bool isOn;
                if (row.StartsWith("on")) {
                    row = row.Substring(3);
                    isOn = true;
                } else if (row.StartsWith("off")) {
                    row = row.Substring(4);
                    isOn = false;
                } else {
                    throw new ArgumentException($"Unexpected input row: {row}");
                }

                string[] ranges = row.Split(",");
                return new Cube(ParseRange(ranges[0]), ParseRange(ranges[1]), ParseRange(ranges[2]), isOn);
            }).ToArray();

            Console.WriteLine(Part1(input));
            Console.WriteLine(Part2(input));
        }

        private static int Part1(Cube[] input) {
            int[,,] on = new int[101, 101, 101];
            foreach (Cube range in input) {
                int xLo = Math.Max(range.XRange.Lo, -50);
                int xHi = Math.Min(range.XRange.Hi, 50);
                int yLo = Math.Max(range.YRange.Lo, -50);
                int yHi = Math.Min(range.YRange.Hi, 50);
                int zLo = Math.Max(range.ZRange.Lo, -50);
                int zHi = Math.Min(range.ZRange.Hi, 50);

                for (int x = xLo; x <= xHi; x++) {
                    for (int y = yLo; y <= yHi; y++) {
                        for (int z = zLo; z <= zHi; z++) {
                            on[x + 50, y + 50, z + 50] = range.IsOn ? 1 : 0;
                        }
                    }
                }
            }

            return on.Cast<int>().Sum();
        }

        private static long Part2(Cube[] input) {
            var maxCube = new Cube(Range.Max, Range.Max, Range.Max, false);
            return CountOn(input, input.Length - 1, maxCube);
        }

        private static long CountOn(Cube[] input, int i, Cube cube) {
            if (i < 0) {
                return 0;
            }

            Cube next = input[i];
            long totalOn = CountOn(input, i - 1, cube);
            if (next.TryIntersect(cube, out Cube intersect)) {
                // Remove any intersecting cubes that are on.
                totalOn -= CountOn(input, i - 1, intersect);

                // If overlapping on, add all overlapping cubes.
                if (next.IsOn) {
                    totalOn += intersect.Volume();
                }
            }

            return totalOn;
        }

        public record Range(int Lo, int Hi) {
            public static readonly Range Max = new Range(int.MinValue, int.MaxValue);

            public long Size() => Hi - Lo + 1;

            public Range Intersect(Range other) {
                int low = Math.Max(Lo, other.Lo);
                int high = Math.Min(Hi, other.Hi);

                return low >= high ? default : new Range(low, high);
            }
        }

        public class Cube {
            public bool IsOn;
            public Range XRange;
            public Range YRange;
            public Range ZRange;

            public long Volume() => XRange.Size() * YRange.Size() * ZRange.Size();

            public Cube(Range xRange, Range yRange, Range zRange, bool isOn) {
                XRange = xRange;
                YRange = yRange;
                ZRange = zRange;
                IsOn = isOn;
            }

            public bool TryIntersect(Cube other, out Cube intersect) {
                Range xIntersect = XRange.Intersect(other.XRange);
                Range yIntersect = YRange.Intersect(other.YRange);
                Range zIntersect = ZRange.Intersect(other.ZRange);
                if (xIntersect == default || yIntersect == default || zIntersect == default) {
                    intersect = default;
                    return false;
                }

                intersect = new Cube(xIntersect, yIntersect, zIntersect, other.IsOn);
                return true;
            }

            public override string ToString() {
                return $"{nameof(IsOn)}: {IsOn}, {nameof(XRange)}: {XRange}, {nameof(YRange)}: {YRange}, {nameof(ZRange)}: {ZRange}";
            }
        }
    }
}
