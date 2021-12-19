using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;

namespace Day19 {
    // https://adventofcode.com/
    public static class Program {
        public static void Main() {
            string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");
            var input = File.ReadAllLines(file);

            var scanners = new List<Scanner>();
            int i = 0;
            while (i < input.Length) {
                if (input[i].StartsWith("---")) {
                    i++;
                    var scanner = new Scanner();
                    scanners.Add(scanner);
                    while (i < input.Length && input[i] != string.Empty) {
                        int[] coords = input[i].Split(",").Select(int.Parse).ToArray();
                        scanner.AddBeacon(new Vector3(coords[0], coords[1], coords[2]));
                        i++;
                    }
                }

                i++;
            }

            Console.WriteLine(Part1(scanners));
            Console.WriteLine(Part2(scanners));
        }

        private static int Part1(List<Scanner> input) {
            HashSet<int> remaining = Enumerable.Range(1, input.Count - 1).ToHashSet();
            var done = new HashSet<int> { 0 };

            var allBeacons = new HashSet<Vector3>();
            AddAll(allBeacons, input[0].NormalizedBeacons());
            while (true) {
                remaining.RemoveWhere(e => done.Contains(e));
                if (remaining.Count == 0) {
                    break;
                }

                foreach (int n in remaining) {
                    if (done.Contains(n)) continue;

                    foreach (int set in done) {
                        if (input[set].Overlaps(input[n]) >= 12) {
                            AddAll(allBeacons, input[n].NormalizedBeacons());
                            done.Add(n);
                            break;
                        }
                    }
                }
            }

            return allBeacons.Count;
        }

        private static void AddAll(ISet<Vector3> set, IEnumerable<Vector3> add) {
            foreach (Vector3 beacon in add) {
                set.Add(beacon);
            }
        }

        private static int Part2(List<Scanner> input) {
            float maxDistance = 0;
            for (int i = 0; i < input.Count; i++) {
                for (int j = 0; j < input.Count; j++) {
                    if (i == j) continue;

                    Vector3 v1 = input[i].Offset();
                    Vector3 v2 = input[j].Offset();

                    float distance = Math.Abs(v1.X - v2.X) + Math.Abs(v1.Y - v2.Y) + Math.Abs(v1.Z - v2.Z);
                    maxDistance = MathF.Max(maxDistance, distance);
                }
            }

            return (int)MathF.Round(maxDistance);
        }
    }

    public class Scanner {
        private static readonly Matrix4x4[] Rotations = {
            Matrix4x4.Identity,
            Matrix4x4.CreateRotationX(MathF.PI / 2),
            Matrix4x4.CreateRotationX(MathF.PI),
            Matrix4x4.CreateRotationX((3 * MathF.PI) / 2),

            Matrix4x4.CreateRotationY(MathF.PI),
            Matrix4x4.CreateRotationY(MathF.PI) * Matrix4x4.CreateRotationX(MathF.PI / 2),
            Matrix4x4.CreateRotationY(MathF.PI) * Matrix4x4.CreateRotationX(MathF.PI),
            Matrix4x4.CreateRotationY(MathF.PI) * Matrix4x4.CreateRotationX((3 * MathF.PI) / 2),

            Matrix4x4.CreateRotationY(MathF.PI / 2),
            Matrix4x4.CreateRotationY(MathF.PI / 2) * Matrix4x4.CreateRotationZ(MathF.PI / 2),
            Matrix4x4.CreateRotationY(MathF.PI / 2) * Matrix4x4.CreateRotationZ(MathF.PI),
            Matrix4x4.CreateRotationY(MathF.PI / 2) * Matrix4x4.CreateRotationZ((3 * MathF.PI) / 2),

            Matrix4x4.CreateRotationY((3 * MathF.PI) / 2),
            Matrix4x4.CreateRotationY((3 * MathF.PI) / 2) * Matrix4x4.CreateRotationZ(MathF.PI / 2),
            Matrix4x4.CreateRotationY((3 * MathF.PI) / 2) * Matrix4x4.CreateRotationZ(MathF.PI),
            Matrix4x4.CreateRotationY((3 * MathF.PI) / 2) * Matrix4x4.CreateRotationZ((3 * MathF.PI) / 2),

            Matrix4x4.CreateRotationZ(MathF.PI / 2),
            Matrix4x4.CreateRotationZ(MathF.PI / 2) * Matrix4x4.CreateRotationY(MathF.PI / 2),
            Matrix4x4.CreateRotationZ(MathF.PI / 2) * Matrix4x4.CreateRotationY(MathF.PI),
            Matrix4x4.CreateRotationZ(MathF.PI / 2) * Matrix4x4.CreateRotationY((3 * MathF.PI) / 2),

            Matrix4x4.CreateRotationZ((3 * MathF.PI) / 2),
            Matrix4x4.CreateRotationZ((3 * MathF.PI) / 2) * Matrix4x4.CreateRotationY(MathF.PI / 2),
            Matrix4x4.CreateRotationZ((3 * MathF.PI) / 2) * Matrix4x4.CreateRotationY(MathF.PI),
            Matrix4x4.CreateRotationZ((3 * MathF.PI) / 2) * Matrix4x4.CreateRotationY((3 * MathF.PI) / 2),
        };

        private readonly List<Vector3> beacons;
        private readonly List<(Matrix4x4, Vector3)> transforms = new();

        public Scanner() {
            beacons = new List<Vector3>();
        }

        public Vector3 Offset() {
            Vector3 result = default;
            foreach ((Matrix4x4 rotate, Vector3 offset) in transforms) {
                result = Rotate(result, rotate) + offset;
            }

            return result;
        }

        public void AddBeacon(Vector3 beacon) {
            beacons.Add(beacon);
        }

        public IEnumerable<Vector3> NormalizedBeacons() {
            return beacons.Select(beacon => {
                foreach ((Matrix4x4 rotate, Vector3 offset) in transforms) {
                    beacon = Rotate(beacon, rotate) + offset;
                }

                return beacon;
            });
        }

        public int Overlaps(Scanner other) {
            foreach (Matrix4x4 rot in Rotations) {
                // rotate the beacons
                var rotBeacons = other.beacons.Select(b => Rotate(b, rot)).ToList();

                foreach (Vector3 beacon in beacons) {
                    // Try to offset any of the rotated beacons to align
                    foreach (Vector3 rotBeacon in rotBeacons) {
                        Vector3 offset = beacon - rotBeacon;
                        int intersectingCount = rotBeacons.Select(v => v + offset).Intersect(beacons).Count();
                        if (intersectingCount >= 12) {
                            other.transforms.Add((rot, offset));
                            other.transforms.AddRange(transforms);
                            return intersectingCount;
                        }
                    }
                }
            }

            return 0;
        }

        private static Vector3 Rotate(in Vector3 beacon, in Matrix4x4 rotation) {
            Vector3 t = Vector3.Transform(beacon, rotation);
            return new Vector3(MathF.Round(t.X), MathF.Round(t.Y), MathF.Round(t.Z));
        }
    }
}
