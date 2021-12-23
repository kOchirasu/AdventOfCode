using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using UtilExtensions;

namespace Day23 {
    // https://adventofcode.com/
    public static class Program {
        public static void Main() {
            string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");
            var input = File.ReadAllLines(file)
                .SelectMany(row => row.ToCharArray())
                .Where(char.IsLetter)
                .Select(c => c - 'A')
                .ToArray();

            List<int[]> rows = new() {
                input.Take(4).ToArray(),
                new []{3, 1, 0, 2},
                new []{3, 2, 1, 0},
                input.Skip(4).Take(4).ToArray()
            };

            int[,] part1 = new[] {
                rows[0],
                rows[3],
            }.UnJagged().Rotate();
            Console.WriteLine(Solve(part1));

            int[,] part2 = new[] {
                rows[0],
                rows[1],
                rows[2],
                rows[3],
            }.UnJagged().Rotate();
            Console.WriteLine(Solve(part2));
        }

        private static int Solve(int[,] rooms) {
            int[] hallway = new int[11];
            Array.Fill(hallway, GameState.EMPTY);

            int bestScore = int.MaxValue;
            var queue = new Queue<GameState>();
            queue.Enqueue(new GameState(hallway, rooms, 0));

            while (queue.TryDequeue(out GameState next)) {
                next.Simplify();
                if (next.Score > bestScore) {
                    continue;
                }
                if (next.Done()) {
                    bestScore = Math.Min(next.Score, bestScore);
                    continue;
                }

                foreach (GameState branch in next.MoveToHallway()) {
                    queue.Enqueue(branch);
                }
            }

            return bestScore;
        }

        public class GameState {
            public const int EMPTY = 8;

            private static readonly int[] RoomIndex = { 2, 4, 6, 8 };
            private static readonly int[] HallwayIndex = { 0, 1, 3, 5, 7, 9, 10 };

            private readonly int[] hallway;
            private readonly int[,] rooms;
            private GameState prev;

            public int Score { get; private set; }

            public GameState(int[] hallway, int[,] rooms, int score) {
                this.hallway = (int[])hallway.Clone();
                this.rooms = (int[,])rooms.Clone();
                Score = score;
            }

            // Any amphipods in hallway or top of room can be moved to free rooms
            public void Simplify() {
                while (HallwayToRoom() || RoomToRoom()) { }
            }

            public IEnumerable<GameState> MoveToHallway() {
                for (int i = 3; i >= 0; i--) {
                    // Nothing to move out of this room.
                    if (rooms.GetRow(i).All(a => a == i || a == EMPTY)) {
                        continue;
                    }

                    foreach (int hIndex in HallwayIndex) {
                        if (hallway[hIndex] != EMPTY) {
                            continue;
                        }
                        if (!CanMoveHallway(RoomIndex[i], hIndex)) {
                            continue;
                        }

                        int steps = RemoveFromRoom(i, out int removed);
                        hallway[hIndex] = removed;
                        int cost = (steps + Math.Abs(hIndex - RoomIndex[i])) * (int)Math.Pow(10, removed);
                        yield return new GameState(hallway, rooms, Score + cost) { prev = this };
                        // restore mutated state
                        rooms[i, rooms.Columns() - steps] = hallway[hIndex];
                        hallway[hIndex] = EMPTY;
                    }
                }
            }

            private bool HallwayToRoom() {
                bool mutated = false;
                for (int i = 3; i >= 0; i--) {
                    int space = RoomSpace(i);
                    if (space <= 0) continue;

                    foreach (int hIndex in HallwayIndex) {
                        int type = hallway[hIndex];
                        if (type == EMPTY || type != i) continue;

                        if (CanMoveHallway(hIndex, RoomIndex[type])) {
                            hallway[hIndex] = EMPTY;
                            int steps = AddToRoom(i);
                            Score += (Math.Abs(hIndex - RoomIndex[type]) + steps) * (int)Math.Pow(10, type);
                            mutated = true;

                            if (--space <= 0) break;
                        }
                    }
                }

                return mutated;
            }

            private bool RoomToRoom() {
                bool mutated = false;
                for (int i = 3; i >= 0; i--) {
                    int space = RoomSpace(i);
                    if (space <= 0) continue;

                    for (int j = 0; j < 4 && space > 0; j++) {
                        if (i == j) continue; // move to self

                        if (PeekRoom(j) == i && CanMoveHallway(RoomIndex[j], RoomIndex[i])) {
                            int steps = RemoveFromRoom(j, out int removed) + AddToRoom(i);

                            Debug.Assert(removed == i);
                            Score += (Math.Abs(RoomIndex[j] - RoomIndex[i]) + steps) * (int)Math.Pow(10, i);

                            mutated = true;
                            space--;
                            j--; // Search same room again in case there are more cases to move.
                        }
                    }
                }

                return mutated;
            }

            private int RoomSpace(int roomNumber) {
                int space = rooms.Columns();
                for (int i = 0; i < rooms.Columns(); i++) {
                    if (rooms[roomNumber, i] == roomNumber) {
                        space--;
                    }
                    // Assume all remaining are empty
                    if (rooms[roomNumber, i] == EMPTY) {
                        break;
                    }
                    if (rooms[roomNumber, i] != roomNumber) {
                        return 0;
                    }
                }

                return space;
            }

            private int PeekRoom(int roomNumber) {
                for (int i = rooms.Columns() - 1; i >= 0; i--) {
                    if (rooms[roomNumber, i] == EMPTY) continue;

                    return rooms[roomNumber, i];
                }

                return EMPTY;
            }

            private int RemoveFromRoom(int roomNumber, out int type) {
                for (int i = rooms.Columns() - 1; i >= 0; i--) {
                    if (rooms[roomNumber, i] == EMPTY) continue;

                    type = rooms[roomNumber, i];
                    rooms[roomNumber, i] = EMPTY;
                    return rooms.Columns() - i;
                }

                throw new InvalidOperationException("Cannot remove from empty room");
            }

            private int AddToRoom(int roomNumber) {
                for (int i = 0; i < rooms.Columns(); i++) {
                    if (rooms[roomNumber, i] == EMPTY) {
                        rooms[roomNumber, i] = roomNumber;
                        return rooms.Columns() - i;
                    }

                    Debug.Assert(rooms[roomNumber, i] == roomNumber);
                }

                throw new InvalidOperationException("Cannot add to a full room");
            }

            private bool CanMoveHallway(int from, int to, bool inclusive = false) {
                int step = from > to ? -1 : 1;
                from = inclusive ? from : from + step;
                for (int i = from; i != to; i += step) {
                    if (hallway[i] != EMPTY) {
                        return false;
                    }
                }

                return true;
            }

            public bool Done() {
                for (int i = 0; i < rooms.Rows(); i++) {
                    for (int j = 0; j < rooms.Columns(); j++) {
                        if (rooms[i, j] != i) {
                            return false;
                        }
                    }
                }

                return true;
            }

            public void PrintChain() {
                GameState current = this;
                do {
                    Console.WriteLine(current);
                    current = current.prev;
                } while (current != null);
            }

            public override string ToString() {
                var builder = new StringBuilder();
                builder.Append(string.Join("", hallway));
                builder.Replace("8", "_");
                builder.Append($" : {Score}");
                builder.AppendLine();
                builder.Append("  ");
                builder.Append(rooms.Rotate(-1).PrettyString("|").Replace("\n", "\n  ").Replace("8", "_"));
                return builder.ToString();
            }
        }
    }
}
