using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day10 {
    // https://adventofcode.com/
    public static class Program {
        public static void Main() {
            string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");
            var input = File.ReadAllLines(file);
            
            Console.WriteLine(Part1(input));
            Console.WriteLine(Part2(input));
        }

        private static int Part1(string[] input) {
            int score = 0;
            foreach (string row in input) {
                score += ProcessRow(row);
            }
            
            return score;
        }

        private static int ProcessRow(string row) {
            var stack = new Stack<char>();
            foreach (char c in row) {
                switch (c) {
                    case '(': case '[': case '{': case '<':
                        stack.Push(c);
                        break;
                    case ')':
                        if (stack.TryPeek(out char aNext) && aNext != '(') {
                            return 3;
                        }

                        stack.TryPop(out char _);
                        break;
                    case ']':
                        if (stack.TryPeek(out char bNext) && bNext != '[') {
                            return 57;
                        }

                        stack.TryPop(out char _);
                        break;
                    case '}':
                        if (stack.TryPeek(out char cNext) && cNext != '{') {
                            return 1197;
                        }

                        stack.TryPop(out char _);
                        break;
                    case '>':
                        if (stack.TryPeek(out char dNext) && dNext != '<') {
                            return 25137;
                        }

                        stack.TryPop(out char _);
                        break;
                }
            }

            return 0;
        }
        
        private static long Part2(string[] input) {
            var scores = new List<long>();
            foreach (string row in input) {
                long result = ProcessRow2(row);
                if (result >= 0) {
                    scores.Add(result);
                }
            }

            scores.Sort();
            return scores[scores.Count / 2];
        }
        
        private static long ProcessRow2(string row) {
            var stack = new Stack<char>();

            foreach (char c in row) {
                switch (c) {
                    case '(': case '[': case '{': case '<':
                        stack.Push(c);
                        break;
                    case ')':
                        stack.TryPop(out char aNext);
                        if (aNext != '(') {
                            return -1;
                        }
                        break;
                    case ']':
                        stack.TryPop(out char bNext);
                        if (bNext != '[') {
                            return -1;
                        }
                        break;
                    case '}':
                        stack.TryPop(out char cNext);
                        if (cNext != '{') {
                            return -1;
                        }
                        break;
                    case '>':
                        stack.TryPop(out char dNext);
                        if (dNext != '<') {
                            return -1;
                        }
                        break;
                }
            }

            long score = 0;
            while (stack.TryPop(out char need)) {
                score *= 5;
                switch (need) {
                    case '(':
                        score += 1;
                        break;
                    case '[':
                        score += 2;
                        break;
                    case '{':
                        score += 3;
                        break;
                    case '<':
                        score += 4;
                        break;
                }
            }
            
            return score;
        }
    }
}
