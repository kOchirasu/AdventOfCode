using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day8 {
    // https://adventofcode.com/
    public static class Program {
        public static void Main() {
            string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");
            var lines = File.ReadAllLines(file).Select(row => {
                string[] x = row.Split("|");
                string[] a = x[0].Split(" ", StringSplitOptions.RemoveEmptyEntries).ToArray();
                string[] b = x[1].Split(" ", StringSplitOptions.RemoveEmptyEntries).ToArray();
                return (a, b);
            }).ToArray();
            
            Console.WriteLine(Part1(lines));
            Console.WriteLine(Part2(lines));
        }

        private static int Part1((string[], string[])[] input) {
            int count = 0;
            foreach ((string[] _, string[] b) in input) {
                foreach (string r in b) {
                    string row = string.Concat(r.OrderBy(c => c));
                    switch (row.Length) {
                        case 2:
                            count++;
                            break;
                        case 4:
                            count++;
                            break;
                        case 3:
                            count++;
                            break;
                        case 7:
                            count++;
                            break;
                    }
                }
            }

            return count;
        }

        private static int Part2((string[], string[])[] input) {
            int count = 0;
            foreach ((string[] a, string[] b) in input) {
                var map = new Dictionary<string, int>();
                var nums = new Number[10];

                var map235 = new HashSet<string>();
                var map069 = new HashSet<string>();

                foreach (string r in a) {
                    string row = string.Concat(r.OrderBy(c => c));
                    switch (row.Length) {
                        case 2:
                            map[row] = 1;
                            nums[1] = new Number(1, new []{row});
                            break;
                        case 4:
                            map[row] = 4;
                            nums[4] = new Number(4, new []{row});
                            break;
                        case 3:
                            map[row] = 7;
                            nums[7] = new Number(7, new []{row});
                            break;
                        case 5: // 2, 3, 5, 
                            map235.Add(row);
                            break;
                        case 6: // 0, 6, 9
                            map069.Add(row);
                            break;
                        case 7:
                            map[row] = 8;
                            nums[8] = new Number(8, new []{row});
                            break;
                    }
                }

                nums[2] = new Number(2, map235);
                nums[3] = new Number(3, map235);
                nums[5] = new Number(5, map235);
                nums[0] = new Number(0, map069);
                nums[6] = new Number(6, map069);
                nums[9] = new Number(9, map069);
                
                nums[0].AddCheck((1, 2), (2, 4), (3, 4), (4, 3), (5, 4), (6, 5), (7, 3), (8, 6), (9, 5));
                nums[1].AddCheck((0, 2), (2, 1), (3, 2), (4, 2), (5, 1), (6, 1), (7, 2), (8, 2), (9, 2));
                nums[2].AddCheck((0, 4), (1, 1), (3, 4), (4, 2), (5, 3), (6, 4), (7, 2), (8, 5), (9, 4));
                nums[3].AddCheck((0, 4), (1, 2), (2, 4), (4, 3), (5, 4), (6, 4), (7, 3), (8, 5), (9, 5));
                nums[4].AddCheck((0, 3), (1, 2), (2, 2), (3, 3), (5, 3), (6, 3), (7, 2), (8, 4), (9, 4));
                nums[5].AddCheck((0, 4), (1, 1), (2, 3), (3, 4), (4, 3), (6, 5), (7, 2), (8, 5), (9, 5));
                nums[6].AddCheck((0, 6), (1, 1), (2, 4), (3, 4), (4, 3), (5, 5), (7, 2), (8, 6), (9, 5));
                nums[7].AddCheck((0, 3), (1, 2), (2, 2), (3, 3), (4, 2), (5, 2), (6, 2), (8, 3), (9, 3));
                nums[8].AddCheck((0, 6), (1, 2), (2, 5), (3, 5), (4, 4), (5, 5), (6, 6), (7, 3), (9, 6));
                nums[9].AddCheck((0, 5), (1, 2), (2, 4), (3, 5), (4, 4), (5, 5), (6, 5), (7, 3), (8, 6));

                var remaining = new List<Number>(nums);
                while (remaining.Count > 0) {
                    bool mutated = false;
                    for (int i = remaining.Count - 1; i >= 0; i--) {
                        if (remaining[i].Found()) {
                            map[remaining[i].Value()] = remaining[i].Digit;
                            remaining.RemoveAt(i);
                            mutated = true;
                            continue;
                        }
                        
                        foreach (Number n in nums) {
                            mutated |= remaining[i].Check(n);
                        }
                    }

                    if (!mutated) {
                        throw new InvalidOperationException("No distinct solution.");
                    }
                }

                int num = 0;
                foreach (string row in b) {
                    num *= 10;
                    num += map[string.Concat(row.OrderBy(c => c))];
                }

                //Console.WriteLine(string.Join(" ", b) + " - " + num);
                count += num;
            }

            return count;
        }
    }

    public class Number {
        public readonly int Digit;
        public readonly List<string> Values;
        public readonly Dictionary<int, int> ToCheck;
        
        public Number(int digit, IEnumerable<string> values) {
            Digit = digit;
            Values = new List<string>(values);
            ToCheck = new Dictionary<int, int>();
        }

        public bool Found() {
            return Values.Count == 1;
        }

        public string Value() {
            if (!Found()) {
                return null;
            }

            return Values.First();
        }

        public void AddCheck(params (int, int)[] t) {
            foreach ((int n, int count) in t) {
                ToCheck[n] = count;
            }
        }

        public bool Check(Number n) {
            if (!n.Found() || !ToCheck.ContainsKey(n.Digit)) {
                return false;
            }

            bool removed = false;
            for (int i = Values.Count - 1; i >= 0; i--) {
                if (CountIntersect(Values[i], n.Value()) != ToCheck[n.Digit]) {
                    Values.RemoveAt(i);
                    removed = true;
                }
            }

            return removed;
        }
        
        private static int CountIntersect(string a, string b) {
            return a.ToCharArray().Intersect(b.ToCharArray()).Count();
        }
    }
}
