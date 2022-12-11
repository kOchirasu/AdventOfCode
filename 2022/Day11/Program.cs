using UtilExtensions;

namespace Day11;

// https://adventofcode.com/
public static class Program {
    public static void Main() {
        string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");
        string[] groups = File.ReadAllText(file).Groups();

        Console.WriteLine(Part1(groups));
        Console.WriteLine(Part2(groups));
    }

    private static long Part1(string[] groups) {
        var monkeys = new Dictionary<long, Monkey>();
        foreach (string group in groups) {
            var monkey = new Monkey(group);
            monkeys.Add(monkey.Id, monkey);
        }

        for (long i = 0; i < 20; i++) {
            foreach (Monkey monkey in monkeys.Values) {
                // Pass item to monkey
                foreach ((int id, long value) in monkey.RunTest(n => n / 3)) {
                    monkeys[id].Items.Add(value);
                }
            }
        }

        return monkeys.Values.Select(m => m.Throws)
            .OrderDescending()
            .Take(2)
            .Aggregate((a, b) => a * b);
    }

    private static long Part2(string[] groups) {
        var monkeys = new Dictionary<long, Monkey>();
        foreach (string group in groups) {
            var monkey = new Monkey(group);
            monkeys.Add(monkey.Id, monkey);
        }

        int lcm = MathUtils.Lcm(monkeys.Values.Select(m => m.TestValue).ToArray());
        for (long i = 0; i < 10000; i++) {
            foreach (Monkey monkey in monkeys.Values) {
                // Pass item to monkey
                foreach ((int id, long value) in monkey.RunTest(n => n % lcm)) {
                    monkeys[id].Items.Add(value);
                }
            }
        }

        return monkeys.Values.Select(m => m.Throws)
            .OrderDescending()
            .Take(2)
            .Aggregate((a, b) => a * b);
    }
}
