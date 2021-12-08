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
                var rmap = new Dictionary<int, string>();

                var map235 = new HashSet<string>();
                var map069 = new HashSet<string>();

                foreach (string r in a) {
                    string row = string.Concat(r.OrderBy(c => c));
                    switch (row.Length) {
                        case 2:
                            map[row] = 1;
                            rmap[1] = row;
                            break;
                        case 4:
                            map[row] = 4;
                            rmap[4] = row;
                            break;
                        case 3:
                            map[row] = 7;
                            rmap[7] = row;
                            break;
                        case 5: // 2, 3, 5, 
                            map235.Add(row);
                            break;
                        case 6: // 0, 6, 9
                            map069.Add(row);
                            break;
                        case 7:
                            map[row] = 8;
                            rmap[8] = row;
                            break;
                    }
                }

                rmap[2] = map235.First(s => CountIntersect(s, rmap[4]) == 2);
                map[rmap[2]] = 2;
                rmap[3] = map235.First(s => CountIntersect(s, rmap[1]) == 2);
                map[rmap[3]] = 3;
                rmap[5] = map235.First(s => CountIntersect(s, rmap[4]) == 3 && CountIntersect(s, rmap[7]) == 2);
                map[rmap[5]] = 5;
                
                rmap[0] = map069.First(s => CountIntersect(s, rmap[7]) == 3 && CountIntersect(s, rmap[4]) == 3);
                map[rmap[0]] = 0;
                rmap[6] = map069.First(s => CountIntersect(s, rmap[4]) == 3 && CountIntersect(s, rmap[1]) == 1);
                map[rmap[6]] = 6;
                rmap[9] = map069.First(s => CountIntersect(s, rmap[4]) == 4);
                map[rmap[9]] = 9;

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

        private static int CountIntersect(string a, string b) {
            return a.ToCharArray().Intersect(b.ToCharArray()).Count();
        }
    }
}
