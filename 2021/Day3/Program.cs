using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day3 {
    // https://adventofcode.com/
    public static class Program {
        public static void Main() {
            string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");
            string[] input = File.ReadAllLines(file);
            
            Console.WriteLine(Part1(input));
            Console.WriteLine(Part2(input));
        }

        private static int Part1(string[] input) {
            int[] counts = new int[input[0].Length];
            foreach (string row in input) {
                int i = 0;
                foreach (char c in row) {
                    if (c == '1') {
                        counts[i]++;
                    }
                    i++;
                }
            }

            string g = "";
            string e = "";
            foreach (int i in counts) {
                if (i > input.Length / 2) {
                    g += '1';
                    e += '0';
                } else {
                    g += '0';
                    e += '1';
                }
            }
            
            return Convert.ToInt32(g, 2) * Convert.ToInt32(e, 2);
        }
        
        private static int Part2(string[] input) {
            List<string> next = input.ToList();
            string ogr = "";
            for (int i = 0; i < next[0].Length; i++) {
                bool want1 = WantOne(next, i);
                var newNext = new List<string>();
                foreach (string row in next) {
                    if ((want1 && row[i] == '1') || (!want1 && row[i] == '0')) {
                        newNext.Add(row);
                    }
                }

                if (newNext.Count == 1) {
                    ogr = newNext[0];
                    break;
                }

                next = newNext;
            }
            
            next = input.ToList();
            string csr = "";
            for (int i = 0; i < next[0].Length; i++) {
                bool want1 = WantOne(next, i);
                var newNext = new List<string>();
                foreach (string row in next) {
                    if ((!want1 && row[i] == '1') || (want1 && row[i] == '0')) {
                        newNext.Add(row);
                    }
                }

                if (newNext.Count == 1) {
                    csr = newNext[0];
                    break;
                }

                next = newNext;
            }
            
            return Convert.ToInt32(ogr, 2) * Convert.ToInt32(csr, 2);
        }

        private static bool WantOne(ICollection<string> input, int index) {
            int total = input.Count;
            int counts = 0;
            foreach (string row in input) {
                if (row[index] == '1') {
                    counts++;
                }
            }
            
            return counts >= Math.Ceiling(total / 2.0);
        }
    }
}
