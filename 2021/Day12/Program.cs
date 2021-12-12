using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UtilExtensions;

namespace Day12 {
    // https://adventofcode.com/
    public static class Program {
        public static void Main() {
            string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");
            var input = File.ReadAllLines(file).Select(row => {
                var split = row.Split("-");
                return (split[0], split[1]);
            }).ToArray();

            var nodes = new Dictionary<string, Node>();
            foreach ((string a, string b) in input) {
                Node nodeA = nodes.GetOrCreate(a, new Node(a));
                Node nodeB = nodes.GetOrCreate(b, new Node(b));
                nodeA.Connect(nodeB);
            }

            Console.WriteLine(Part1(nodes));
            Console.WriteLine(Part2(nodes));
        }

        private static int Part1(Dictionary<string, Node> nodes) {
            var visited = new Dictionary<string, int>();
            foreach (Node node in nodes.Values) {
                if (node.Small) {
                    visited[node.Name] = 0;
                }
            }

            return Search(nodes["start"], visited, 1);
        }

        private static int Part2(Dictionary<string, Node> nodes) {
            var visited = new Dictionary<string, int>();
            foreach (Node node in nodes.Values) {
                if (node.Small) {
                    visited[node.Name] = 0;
                }
            }

            return Search(nodes["start"], visited, 2);
        }

        private static int Search(Node current, Dictionary<string, int> visited, int limit) {
            if (current.Small) {
                if (current.Name is "start" or "end" && visited[current.Name] >= 1) {
                    return 0;
                }
                if (visited[current.Name] >= limit) {
                    return 0;
                }
            }
            if (current.Name == "end") {
                return 1;
            }

            int count = 0;
            if (current.Small) {
                visited[current.Name]++;
                // Part 2
                if (visited[current.Name] == 2) {
                    limit = 1;
                }
            }
            foreach (Node child in current.Connected.Values) {
                count += Search(child, visited, limit);
            }
            if (current.Small) {
                visited[current.Name]--;
            }

            return count;
        }
    }

    public class Node {
        public readonly string Name;
        public readonly bool Small;
        public readonly Dictionary<string, Node> Connected;

        public Node(string name) {
            Name = name;
            Small = IsSmall(name);
            Connected = new Dictionary<string, Node>();
        }

        public void Connect(Node node) {
            Connected[node.Name] = node;
            node.Connected[Name] = this;
        }

        private static bool IsSmall(string name) {
            bool allSmall = true;
            foreach (char c in name) {
                allSmall &= char.IsLower(c);
            }

            return allSmall;
        }
    }
}
