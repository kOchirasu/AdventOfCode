using System.Diagnostics;
using UtilExtensions;
using static UtilExtensions.ArrayExtensions;

namespace Day22;

// https://adventofcode.com/
public static class Program {
    private const int Dimensions = 50;

    public static void Main() {
        string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");

        var groups = File.ReadAllText(file).Groups();
        var parsed = groups[0]
            .StringList()
            .CharMatrix()
            .Select(c => c == '\0' ? ' ' : c);

        var path = new PathReader(groups[1]);
        Console.WriteLine(Part1(parsed, path));
        Console.WriteLine(Part2(parsed, path));
    }

    private static int Part1(char[,] grid, PathReader path) {
        path.Reset();
        (int row, int col) = (0, Array.IndexOf(grid.GetRow(0), '.'));
        var direction = Direction.E;

        while (path.Remaining > 0) {
            if (path.NextInt(out int distance)) {
                (int nextRow, int nextCol) = (row, col);
                for (int j = 0; j < distance; j++) {
                    do {
                        (nextRow, nextCol) = grid.Adjacent(nextRow, nextCol, direction, AdjacencyOptions.Wrap);
                    } while (grid[nextRow, nextCol] == ' ');

                    if (grid[nextRow, nextCol] == '#') {
                        break;
                    }
                    (row, col) = (nextRow, nextCol);
                }
            }

            if (path.NextRotation(out char rotation)) {
                direction = direction.Rotate(rotation == 'R' ? 90 : -90);
            }
        }

        return (row + 1) * 1000 + (col + 1) * 4 + direction switch {
            Direction.E => 0,
            Direction.S => 1,
            Direction.W => 2,
            Direction.N => 3,
            _ => throw new ArgumentException("Invalid direction"),
        };
    }

    private static int Part2(char[,] grid, PathReader path) {
        path.Reset();
        (int row, int col) = (0, Array.IndexOf(grid.GetRow(0), '.'));
        var direction = Direction.E;

        while (path.Remaining > 0) {
            if (path.NextInt(out int distance)) {
                (int nextRow, int nextCol) = (row, col);
                Direction nextDirection = direction;
                for (int j = 0; j < distance; j++) {
                    (int dR, int dC) = direction.Delta();
                    (nextRow, nextCol) = (nextRow + dR, nextCol + dC);
                    if (!grid.TryGet(nextRow, nextCol, out char val) || val != '.') {
                        switch (nextDirection) {
                            case Direction.N:
                            case Direction.S:
                                (nextRow, nextCol, nextDirection) = WrapRow(nextRow, nextCol, nextDirection);
                                break;
                            case Direction.E:
                            case Direction.W:
                                (nextRow, nextCol, nextDirection) = WrapCol(nextRow, nextCol, nextDirection);
                                break;
                        }
                    }

                    if (grid[nextRow, nextCol] == '#') {
                        break;
                    }

                    (row, col) = (nextRow, nextCol);
                    direction = nextDirection;
                }
                Debug.Assert(grid[row, col] is '.', $"Moved to invalid position: {(row, col)}");
            }

            if (path.NextRotation(out char rotation)) {
                direction = direction.Rotate(rotation == 'R' ? 1 : -1);
            }

            const int dim1 = Dimensions;
            const int dim2 = Dimensions * 2;
            const int dim3 = Dimensions * 3;
            const int dim4 = Dimensions * 4;
            (int, int, Direction) WrapRow(int r, int c, Direction d) {
                return c switch {
                    >= 0 and < dim1 => r switch {
                        < dim2 => (c + dim1, dim1, d.Rotate(90)),
                        >= dim4 => (0, c + dim2, d),
                        _ => (r, c, d),
                    },
                    >= dim1 and < dim2 => r switch {
                        >= dim3 => (c + dim2, dim1 - 1, d.Rotate(90)),
                        < 0 => (c + dim2, 0, d.Rotate(90)),
                        _ => (r, c, d),
                    },
                    >= dim2 and < dim3 => r switch {
                        >= dim1 => (c - dim1, dim2 - 1, d.Rotate(90)),
                        < 0 => (dim4 - 1, c - dim2, d),
                        _ => (r, c, d),
                    },
                    _ => (r, c, d),
                };
            }

            (int, int, Direction) WrapCol(int r, int c, Direction d) {
                return r switch {
                    >= 0 and < dim1 => c switch {
                        < dim1 => (dim3 - 1 - r, 0, d.Rotate(180)),
                        >= dim3 => (dim3 - 1 - r, dim2 - 1, d.Rotate(180)),
                        _ => (r, c, d),
                    },
                    >= dim1 and < dim2 => c switch {
                        >= dim2 => (dim1 - 1, r + dim1, d.Rotate(-90)),
                        < dim1 => (dim2, r - dim1, d.Rotate(-90)),
                        _ => (r, c, d),
                    },
                    >= dim2 and < dim3 => c switch {
                        >= dim2 => (dim3 - 1 - r, dim3 - 1, d.Rotate(180)),
                        < 0 => (dim3 - 1 - r, dim1, d.Rotate(180)),
                        _ => (r, c, d),
                    },
                    >= dim3 and < dim4 => c switch {
                        >= dim1 => (dim3 - 1, r - dim2, d.Rotate(-90)),
                        < 0 => (0, r - dim2, d.Rotate(-90)),
                        _ => (r, c, d),
                    },
                    _ => (r, c, d),
                };
            }
        }

        return (row + 1) * 1000 + (col + 1) * 4 + direction switch {
            Direction.E => 0,
            Direction.S => 1,
            Direction.W => 2,
            Direction.N => 3,
            _ => throw new ArgumentException("Invalid direction"),
        };
    }
}
