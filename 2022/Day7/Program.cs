using System.Text;
using UtilExtensions;

namespace Day7;

// https://adventofcode.com/
public static class Program {
    public static void Main() {
        string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");

        var sizes = new DefaultDictionary<string, int>();
        var paths = new Stack<string>();
        foreach (string command in File.ReadAllLines(file)) {
            string[] args = command.Split(" ");
            if (command.StartsWith("$ cd")) {
                switch (args[2]) {
                    case "/":
                        paths.Clear();
                        paths.Push("/");
                        break;
                    case "..":
                        paths.Pop();
                        break;
                    default:
                        paths.Push(args[2]);
                        break;
                }
            } else if (int.TryParse(args[0], out int size)) {
                // Add file size to current dir and all parents
                foreach (string p in AllPaths(paths)) {
                    sizes[p] += size;
                }
            }
        }

        Console.WriteLine(Part1(sizes));
        Console.WriteLine(Part2(sizes));
    }

    private static IEnumerable<string> AllPaths(Stack<string> path) {
        StringBuilder builder = new();
        foreach (string str in path.Reverse()) {
            if (builder.Length == 0) {
                builder.Append($"{str}");
            } else {
                builder.Append($"{str}/");
            }
            yield return builder.ToString();
        }
    }

    private static int Part1(IDictionary<string, int> sizes) {
        return sizes.Values.Where(size => size <= 100000).Sum();
    }

    private static int Part2(IDictionary<string, int> sizes) {
        int freeSpace = 70000000 - sizes["/"];
        return sizes.OrderBy(e => e.Value).First(deleteFile => freeSpace + deleteFile.Value > 30000000).Value;
    }
}
