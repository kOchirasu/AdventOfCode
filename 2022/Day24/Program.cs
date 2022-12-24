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

        (int, int) start = (0, 1);
        (int, int) end = (grid.RowCount() - 1, grid.ColumnCount() - 2);
        Console.WriteLine(Part1(start, end));
        Console.WriteLine(Part2(start, end));
    }

    private static int Part1((int, int) start, (int, int) end) {
        return Search(start, end, 0);
    }

    private static int Part2((int, int) start, (int, int) end) {
        int time = Search(start, end, 0);
        time = Search(end, start, time);
        return Search(start, end, time);
    }

    private static readonly List<Tile[,]> MinuteGrids = new();
    private static readonly Dictionary<Tile, Directions> Movements = new() {
        {Tile.North, Directions.N},
        {Tile.South, Directions.S},
        {Tile.East, Directions.E},
        {Tile.West, Directions.W},
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

                foreach ((Tile blizzard, Directions direction) in Movements) {
                    if ((tile & blizzard) != 0) {
                        (int r, int c) pos = (r, c);
                        do {
                            pos = prev.Adjacent(pos.r, pos.c, direction | Directions.Wrap).First();
                        } while (prev[pos.r, pos.c] == Tile.Wall);

                        next[pos.r, pos.c] |= blizzard;
                    }
                }
            }
        }

        MinuteGrids.Add(next);
        return MinuteGrids[minute];
    }

    private static int Search((int r, int c) start, (int r, int c) end, int minute) {
        var queue = new Queue<(int r, int c, int min)>();
        queue.Enqueue((start.r, start.c, minute));

        var duplicates = new HashSet<(int, int, int)>();
        while (queue.TryDequeue(out (int r, int c, int min) cur)) {
            if (cur.r == end.r && cur.c == end.c) {
                return cur.min;
            }

            if (duplicates.Contains(cur)) {
                continue;
            }
            duplicates.Add(cur);


            Tile[,] grid = GetGrid(cur.min + 1);
            if (grid[cur.r, cur.c] == Tile.Ground) {
                queue.Enqueue((cur.r, cur.c, cur.min + 1)); // Stay
            }

            foreach ((int r, int c) adj in grid.Adjacent(cur.r, cur.c, Directions.Cardinal)) {
                if (grid[adj.r, adj.c] == Tile.Ground) {
                    queue.Enqueue((adj.r, adj.c, cur.min + 1));
                }
            }
        }

        throw new ArgumentException("No path found");
    }
}
