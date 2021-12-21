using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UtilExtensions;

namespace Day21 {
    // https://adventofcode.com/
    public static class Program {
        private static readonly DefaultDictionary<int, int> Branches;
        static Program() {
            Branches = new DefaultDictionary<int, int>();
            for (int i = 1; i <= 3; i++) {
                for (int j = 1; j <= 3; j++) {
                    for (int k = 1; k <= 3; k++) {
                        Branches[i + j + k] += 1;
                    }
                }
            }
        }

        public static void Main() {
            string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");
            var input = File.ReadAllLines(file).Select(row => row[^1] - '0').ToArray();

            Console.WriteLine(Part1(input[0], input[1]));
            Console.WriteLine(Part2(input[0], input[1]));
        }

        private static int Part1(int p1, int p2) {
            p1--;
            p2--;
            int p1Score = 0;
            int p2Score = 0;

            int rolls = 0;
            int current = 2;
            while (p1Score < 1000 && p2Score < 1000) {
                if (current % 2 == 0) {
                    p1 =  (p1 + current * 3) % 10;
                    p1Score += p1 + 1;
                } else {
                    p2 =  (p2 + current * 3) % 10;
                    p2Score += p2 + 1;
                }

                current += 3;
                rolls += 3;
            }

            return Math.Min(p1Score, p2Score) * rolls;
        }

        private static long Part2(int p1, int p2) {
            p1--;
            p2--;

            long p1Win = 0;
            long p2Win = 0;

            var queue = new Queue<GameState>();
            queue.Enqueue(new GameState(p1, p2));
            while (queue.TryDequeue(out GameState next)) {
                foreach (GameState state in next.Roll()) {
                    if (state.P1Score >= 21) {
                        p1Win += state.Count;
                    } else if (state.P2Score >= 21) {
                        p2Win += state.Count;
                    } else {
                        queue.Enqueue(state);
                    }
                }
            }

            return Math.Max(p1Win, p2Win);
        }

        private struct GameState {
            public long Count;
            public int P1Score;
            public int P2Score;
            private int p1;
            private int p2;
            private bool turn; // false = p1, true = p2

            public GameState(int p1, int p2) {
                Count = 1;
                P1Score = 0;
                P2Score = 0;
                this.p1 = p1;
                this.p2 = p2;
                turn = false;
            }

            public IEnumerable<GameState> Roll() {
                foreach ((int roll, int count) in Branches) {
                    GameState next = this; // Copy this state.
                    next.Count *= count;
                    if (!turn) {
                        next.p1 =  (next.p1 + roll) % 10;
                        next.P1Score += next.p1 + 1;
                    } else {
                        next.p2 =  (next.p2 + roll) % 10;
                        next.P2Score += next.p2 + 1;
                    }

                    next.turn = !turn;
                    yield return next;
                }
            }
        }
    }
}
