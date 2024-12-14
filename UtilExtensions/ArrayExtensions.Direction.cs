using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace UtilExtensions;

public static partial class ArrayExtensions {
    public enum Direction {
        Origin = 1,
        N = 2,
        E = 4,
        S = 8,
        W = 16,
        NE = 32,
        SE = 64,
        SW = 128,
        NW = 256,
    }

    [Flags]
    public enum Directions {
        Origin = Direction.Origin,
        N = Direction.N,
        E = Direction.E,
        S = Direction.S,
        W = Direction.W,
        NE = Direction.NE,
        SE = Direction.SE,
        SW = Direction.SW,
        NW = Direction.NW,

        Cardinal = N | E | S | W,
        Intermediate = NE | SE | SW | NW,
        All = Cardinal | Intermediate,
    }

    private static readonly Dictionary<Direction, (int, int)> Offsets = new() {
        {Direction.Origin, (0, 0)},
        {Direction.N, (-1, 0)},
        {Direction.E, (0, 1)},
        {Direction.S, (1, 0)},
        {Direction.W, (0, -1)},
        {Direction.NE, (-1, 1)},
        {Direction.SE, (1, 1)},
        {Direction.SW, (1, -1)},
        {Direction.NW, (-1, -1)},
    };

    private static readonly Direction[] Rotations = {
        Direction.N, Direction.NE, Direction.E, Direction.SE,
        Direction.S, Direction.SW, Direction.W, Direction.NW,
    };

    public static IEnumerable<Direction> Enumerate(this Directions dirs) {
        foreach (Direction dir in Offsets.Keys) {
            if ((dirs & (Directions)dir) == (Directions)dir) {
                yield return dir;
            }
        }
    }

    public static IEnumerable<(int, int)> Deltas(this Directions dir) {
        return dir.Enumerate()
            .Select(d => Offsets[d]);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static (int, int) Delta(this Direction dir) {
        return Offsets[dir];
    }

    public static Directions Rotate(this Directions dir, int degrees) {
        return dir.Enumerate()
            .Select(d => (Directions) d.Rotate(degrees))
            .Aggregate((a, b) => a | b);
    }

    public static Direction Rotate(this Direction dir, int degrees) {
        if (degrees % 45 != 0) {
            throw new ArgumentException("Rotation must be a multiple of 45");
        }
        if (dir == Direction.Origin) {
            return Direction.Origin;
        }

        int index = Array.IndexOf(Rotations, dir);
        if (index == -1) {
            throw new ArgumentException($"Invalid direction: {dir}");
        }

        int steps = (degrees % 360 + 360) / 45;
        return Rotations[(index + steps) % Rotations.Length];
    }
}
