namespace Day1;

// https://adventofcode.com/
public static class Program {
    public static void Main() {
        string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");

        var list = new List<int>();
        int sum = 0;
        foreach (string row in File.ReadAllLines(file)) {
            if (int.TryParse(row, out int i)) {
                sum += i;
            } else {
                list.Add(sum);
                sum = 0;
            }
        }

        Console.WriteLine(Part1(list));
        Console.WriteLine(Part2(list));
    }

    private static int Part1(IList<int> list) {
        return list.Max();
    }

    private static int Part2(IList<int> list) {
        return list.OrderByDescending(i => i).Take(3).Sum();
    }
}
