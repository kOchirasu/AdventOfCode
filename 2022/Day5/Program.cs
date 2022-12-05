using UtilExtensions;

namespace Day5;

// https://adventofcode.com/
public static class Program {
    public static void Main() {
        string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");

        string[] groups = File.ReadAllText(file).Groups();
        string[] board = groups[0].StringList();
        int size = board.Last()
            .Split(" ", StringSplitOptions.RemoveEmptyEntries)
            .Length;

        var stacks = new List<Stack<char>>();
        for (int i = 0; i < size; i++) {
            stacks.Add(new Stack<char>());
        }
        foreach (string line in board.Reverse().Skip(1)) {
            int i = 0;
            foreach (string block in line.SplitEveryN(1, pad: 3, offset: 1)) {
                if (block[0] != ' ') {
                    stacks[i].Push(block[0]);
                }
                i++;
            }
        }

        var operations = new List<(int, int, int)>();
        foreach (string line in groups[1].StringList()) {
            if (string.IsNullOrWhiteSpace(line)) {
                continue;
            }

            string[] splits = line.Split(" ");
            int amt = int.Parse(splits[1]);
            int src = int.Parse(splits[3]) - 1;
            int dst = int.Parse(splits[5]) - 1;

            operations.Add((amt, src, dst));
        }

        Console.WriteLine(Part1(stacks.DeepCopy(), operations));
        Console.WriteLine(Part2(stacks.DeepCopy(), operations));
    }

    private static string Part1(IList<Stack<char>> stacks, IList<(int, int, int)> operations) {
        foreach ((int amt, int src, int dst) in operations) {
            for (int i = 0; i < amt; i++) {
                if (stacks[src].Count == 0) {
                    break;
                }

                stacks[dst].Push(stacks[src].Pop());
            }
        }

        return string.Join("", stacks.Select(stack => stack.Pop()));
    }

    private static string Part2(IList<Stack<char>> stacks, IList<(int, int, int)> operations) {
        foreach ((int amt, int src, int dst) in operations) {
            var temp = new Stack<char>();
            for (int i = 0; i < amt; i++) {
                if (stacks[src].Count == 0) {
                    break;
                }

                temp.Push(stacks[src].Pop());
            }

            foreach (char t in temp) {
                stacks[dst].Push(t);
            }
        }

        return string.Join("", stacks.Select(stack => stack.Pop()));
    }

    private static void PrintStacks(IList<Stack<char>> stacks) {
        for (int i = 0; i < stacks.Count; i++) {
            Console.WriteLine($"{i+1}: {string.Join(" ", stacks[i])} = {(stacks[i].Count > 0 ? stacks[i].Peek() : "")}");
        }
    }
}
