using UtilExtensions;

namespace Day11;

public class Monkey {
    public readonly int Id;
    public readonly int TestValue;
    public readonly List<long> Items;
    public long Throws;

    private readonly string[] operation;
    private readonly int trueCase;
    private readonly int falseCase;

    public Monkey(string data) {
        var list = data.StringList();
        Id = int.Parse(list[0].Extract(@"(\d+)")[0]);
        Items = list[1].Extract(@"(?:(\d+)[ ,]{0,2})+").Select(long.Parse).ToList();
        operation = list[2].Replace("Operation: ", "").Split(" ", StringSplitOptions.RemoveEmptyEntries);
        TestValue = int.Parse(list[3].Replace("Test: divisible by ", ""));
        trueCase = int.Parse(list[4].Replace("If true: throw to monkey ", ""));
        falseCase = int.Parse(list[5].Replace("If false: throw to monkey ", ""));
    }

    public IEnumerable<(int Id, long Value)> RunTest(Func<long, long> reducer) {
        foreach (long item in Items) {
            long newValue = reducer(RunOperation(item));
            if (newValue % TestValue == 0) {
                yield return (trueCase, newValue);
            } else {
                yield return (falseCase, newValue);
            }
            Throws++;
        }
        Items.Clear();
    }

    private long RunOperation(long old) {
        if (!long.TryParse(operation[2], out long left)) {
            left = old;
        }
        if (!long.TryParse(operation[4], out long right)) {
            right = old;
        }

        return operation[3] switch {
            "+" => left + right,
            "*" => left * right,
            _ => throw new ArgumentException($"Invalid operation: {operation} '{operation[1]}'"),
        };
    }

    public override string ToString() {
        return $"{Id}={Throws}: [{Items.PrettyString()}]";
    }
}
