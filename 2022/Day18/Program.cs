using UtilExtensions;

namespace Day18;

// https://adventofcode.com/
public static class Program {
    private record Cube(int X, int Y, int Z);

    public static void Main() {
        string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");

        var cubes = new HashSet<Cube>();
        foreach (string line in File.ReadAllLines(file)) {
            int[] splits = line.Split(",").Select(int.Parse).ToArray();
            cubes.Add(new Cube(splits[0], splits[1], splits[2]));
        }

        Console.WriteLine(Part1(cubes));
        Console.WriteLine(Part2(cubes));
    }

    private static int Part1(HashSet<Cube> cubes) {
        int max = cubes.SelectMany(cube => new[] {cube.X, cube.Y, cube.Z}).Max() + 2;
        bool[,,] grid = new bool[max, max, max];
        foreach (Cube cube in cubes) {
            grid[cube.X, cube.Y, cube.Z] = true;
        }

        int count = 0;
        foreach (Cube cube in cubes.SelectMany(cube => cube.Adjacent())) {
            if (!grid.TryGet(cube.X, cube.Y, cube.Z, out bool result) || !result) {
                count++;
            }
        }

        return count;
    }

    private static int Part2(HashSet<Cube> cubes) {
        int max = cubes.SelectMany(cube => new[] {cube.X, cube.Y, cube.Z}).Max() + 2;
        bool?[,,] grid = new bool?[max, max, max];
        foreach (Cube cube in cubes) {
            grid[cube.X, cube.Y, cube.Z] = true;
        }

        // Fill outside with false, we ignore all inner gaps (null).
        var queue = new Queue<Cube>();
        queue.Enqueue(new Cube(0, 0, 0));
        grid[0, 0, 0] = false;
        while (queue.TryDequeue(out Cube? outside)) {
            foreach (Cube cube in outside.Adjacent()) {
                if (grid.TryGet(cube.X, cube.Y, cube.Z, out bool? result) && result is null) {
                    grid[cube.X, cube.Y, cube.Z] = false;
                    queue.Enqueue(cube);
                }
            }
        }

        // At this point 'null' cubes are gaps in the lava droplet.
        int count = 0;
        foreach (Cube cube in cubes.SelectMany(cube => cube.Adjacent())) {
            if (!grid.TryGet(cube.X, cube.Y, cube.Z, out bool? result) || result is false) {
                count++;
            }
        }

        return count;
    }

    private static IEnumerable<Cube> Adjacent(this Cube cube) {
        yield return cube with { X = cube.X - 1 };
        yield return cube with { Y = cube.Y - 1 };
        yield return cube with { Z = cube.Z - 1 };

        yield return cube with { X = cube.X + 1 };
        yield return cube with { Y = cube.Y + 1 };
        yield return cube with { Z = cube.Z + 1 };
    }
}
