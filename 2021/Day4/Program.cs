using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day4 {
    // https://adventofcode.com/
    public static class Program {
        public static void Main() {
            string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");
            string[] input = File.ReadAllLines(file);

            int[] nums = input[0].Split(",").Select(int.Parse).ToArray();
            var boards = new List<Board>();
            for (int i = 1; i < input.Length; i+=6) {
                var board = new Board();
                for (int j = 1; j <= 5; j++) {
                    int[] next = input[i + j].Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
                    for (int k = 0; k < 5; k++) {
                        board.Data[j - 1, k] = next[k];
                    }
                }

                boards.Add(board);
            }
            
            Console.WriteLine(Part1(nums, boards));
            Console.WriteLine(Part2(nums, boards));
        }

        private static int Part1(int[] nums, List<Board> boards) {
            foreach (int n in nums) {
                foreach (Board board in boards) {
                    if (board.Set(n)) {
                        return board.Remaining() * n;
                    }

                }
            }

            throw new InvalidOperationException("no solution");
        }
        
        private static int Part2(int[] nums, List<Board> boards) {
            foreach (int n in nums) {
                for (int i = boards.Count - 1; i >= 0; i--) {
                    if (boards[i].Set(n)) {
                        if (boards.Count == 1) {
                            return boards[0].Remaining() * n;
                        }
                        
                        boards.RemoveAt(i);
                    }
                }
            }

            throw new InvalidOperationException("no solution");
        }
    }

    public class Board {
        public readonly int[,] Data = new int[5,5];

        public bool Set(int n) {
            for (int i = 0; i < 5; i++) {
                for (int j = 0; j < 5; j++) {
                    if (Data[i, j] == n) {
                        Data[i, j] = -1;
                        return Winning(i, j);
                    }
                }
            }

            return false;
        }

        public int Remaining() {
            int sum = 0;
            for (int i = 0; i < 5; i++) {
                for (int j = 0; j < 5; j++) {
                    if (Data[i, j] > 0) {
                        sum += Data[i, j];
                    }
                }
            }

            return sum;
        }

        public bool Winning(int h, int v) {
            bool allChecked = true;
            for (int i = 0; i < 5; i++) {
                allChecked &= Data[h, i] == -1;
            }
            if (allChecked) {
                return true;
            }
            
            allChecked = true;
            for (int i = 0; i < 5; i++) {
                allChecked &= Data[i, v] == -1;
            }
            if (allChecked) {
                return true;
            }

            // check diagonal
            if (h == 2 && v == 2) {
                allChecked = true;
                for (int i = 0; i < 5; i++) {
                    allChecked &= Data[i, i] != -1;
                }
                if (allChecked) {
                    return true;
                }
                
                allChecked = true;
                for (int i = 4; i >= 0; i--) {
                    allChecked &= Data[i, i] != -1;
                }
                if (allChecked) {
                    return true;
                }
            }
            
            return false;
        }
    }
}
