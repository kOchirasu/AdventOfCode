using UtilExtensions;
using static UtilExtensions.ArrayExtensions;

namespace Day24;

// https://adventofcode.com/
public static class Program {
    [Flags]
    private enum Tile {
        Ground = 0,
        North = 1,
        South = 2,
        East = 4,
        West = 8,
        Wall = 16,
    }

    public static void Main() {
        string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");

        Tile[,] grid = File.ReadAllLines(file).CharMatrix().Select(ch => ch switch {
            '#' => Tile.Wall,
            '.' => Tile.Ground,
            '^' => Tile.North,
            'v' => Tile.South,
            '>' => Tile.East,
            '<' => Tile.West,
            _ => throw new ArgumentOutOfRangeException(nameof(ch), ch, null),
        });

        MinuteGrids.Add(grid);

        var start = new Point(0, 1);
        var end = new Point(grid.RowCount() - 1, grid.ColumnCount() - 2);
        Console.WriteLine(Part1(start, end));
        Console.WriteLine(Part2(start, end));
    }

    private static int Part1(Point start, Point end) {
        return Search(start, end, 0);
    }

    private static int Part2(Point start, Point end) {
        int time = Search(start, end, 0);
        time = Search(end, start, time);
        return Search(start, end, time);
    }

    private static readonly List<Tile[,]> MinuteGrids = new();
    private static readonly Dictionary<Tile, Direction> Movements = new() {
        {Tile.North, Direction.N},
        {Tile.South, Direction.S},
        {Tile.East, Direction.E},
        {Tile.West, Direction.W},
    };
    private static Tile[,] GetGrid(int minute) {
        if (minute < MinuteGrids.Count) {
            return MinuteGrids[minute];
        }

        Tile[,] prev = GetGrid(minute - 1);
        // Create a blank grid with ground and walls only.
        Tile[,] next = prev.Select(ch => ch == Tile.Wall ? Tile.Wall : Tile.Ground);

        for (int r = 0; r < prev.RowCount(); r++) {
            for (int c = 0; c < prev.ColumnCount(); c++) {
                Tile tile = prev[r, c];
                if (tile is Tile.Ground or Tile.Wall) {
                    continue;
                }

                foreach ((Tile blizzard, Direction direction) in Movements) {
                    if ((tile & blizzard) != 0) {
                        var pos = new Point(r, c);
                        do {
                            pos = prev.Adjacent(pos, direction, AdjacencyOptions.Wrap);
                        } while (prev[pos.Row, pos.Col] == Tile.Wall);

                        next[pos.Row, pos.Col] |= blizzard;
                    }
                }
            }
        }

        MinuteGrids.Add(next);
        return MinuteGrids[minute];
    }

    private static int Search(Point start, Point end, int minute) {
        var queue = new Queue<(Point p, int min)>();
        queue.Enqueue((start, minute));

        var duplicates = new HashSet<(Point, int)>();
        while (queue.TryDequeue(out (Point p, int min) cur)) {
            if (cur.p == end) {
                return cur.min;
            }

            if (duplicates.Contains(cur)) {
                continue;
            }
            duplicates.Add(cur);


            Tile[,] grid = GetGrid(cur.min + 1);
            if (grid[cur.p.Row, cur.p.Col] == Tile.Ground) {
                queue.Enqueue((cur.p, cur.min + 1)); // Stay
            }

            foreach (Point adj in grid.Adjacent(cur.p, Directions.Cardinal)) {
                if (grid[adj.Row, adj.Col] == Tile.Ground) {
                    queue.Enqueue((adj, cur.min + 1));
                }
            }
        }

        throw new ArgumentException("No path found");
    }
}
