using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UtilExtensions;

namespace Day14 {
    // https://adventofcode.com/
    public static class Program {
        public static void Main() {
            string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");
            var input = File.ReadAllLines(file);

            string template = input[0];
            var rules = new Dictionary<string, string>();
            for (int i = 2; i < input.Length; i++) {
                string[] mapping = input[i].Split(" -> ");
                rules.Add(mapping[0], mapping[1]);
            }

            Console.WriteLine(Part1(template, rules));
            Console.WriteLine(Part2(template, rules));
        }

        private static int Part1(string template, Dictionary<string, string> rules) {
            for (int i = 0; i < 10; i++) {
                var next = new StringBuilder();
                for (int j = 0; j < template.Length - 1; j++) {
                    string rule = "" + template[j] + template[j + 1];
                    if (rules.TryGetValue(rule, out string insert)) {
                        next.Append(template[j]);
                        next.Append(insert);
                    }
                }

                next.Append(template[^1]);
                template = next.ToString();
            }

            var counts = new DefaultDictionary<char, int>();
            foreach (char c in template) {
                counts[c] += 1;
            }

            List<int> list = counts.Values.ToList();
            return list.Max() - list.Min();
        }

        private static long Part2(string template, Dictionary<string, string> rules) {
            char first = template[0];
            var pairs = new DefaultDictionary<string, long>();
            for (int i = 0; i < template.Length - 1; i++) {
                string pair = template.Substring(i, 2);
                pairs[pair]++;
            }

            for (int i = 0; i < 40; i++) {
                var next = new DefaultDictionary<string, long>();
                foreach ((string rule, string insert) in rules) {
                    if (pairs.TryGetValue(rule, out long count)) {
                        pairs.Remove(rule);
                        string pair1 = rule[0] + insert;
                        string pair2 = insert + rule[1];

                        next[pair1] += count;
                        next[pair2] += count;
                    }
                }

                // Unmapped pairs
                foreach ((string k, long v) in pairs) {
                    next.Add(k, v);
                }

                pairs = next;
            }

            var counts = new DefaultDictionary<char, long>();
            foreach ((string k, long v) in pairs) {
                counts[k[1]] += v;
            }
            // Since we only take 2nd char of each pair, we need to add first.
            counts[first]++;

            List<long> list = counts.Values.ToList();
            return list.Max() - list.Min();
        }
    }
}
