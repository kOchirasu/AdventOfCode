using UtilExtensions;

namespace Day13;

// https://adventofcode.com/
public static class Program {
    public static void Main() {
        string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");

        List<(Packet, Packet)> packets = new();
        foreach (string group in File.ReadAllText(file).Groups()) {
            string[] splits = group.Split("\n");
            packets.Add((new Packet(splits[0]), new Packet(splits[1])));
        }

        Console.WriteLine(Part1(packets));
        Console.WriteLine(Part2(packets));
    }

    private static int Part1(IList<(Packet, Packet)> packets) {
        int total = 0;
        for (int i = 0; i < packets.Count; i++) {
            Packet left = packets[i].Item1;
            Packet right = packets[i].Item2;

            if (left.CompareTo(right) < 0) {
                total += i + 1;
            }
        }

        return total;
    }

    private static int Part2(IList<(Packet, Packet)> packets) {
        List<Packet> list = packets.SelectMany(p => new[] {p.Item1, p.Item2}).ToList();
        list.Add(new Packet("[[2]]") {IsLocator = true});
        list.Add(new Packet("[[6]]") {IsLocator = true});
        list.Sort();

        int result = 1;
        for (int i = 0; i < list.Count; i++) {
            if (list[i].IsLocator) {
                result *= (i + 1);
            }
        }

        return result;
    }
}
