namespace Day9;

// https://adventofcode.com/
public static class Program {
    public static void Main() {
        string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");

        int[] values = File.ReadAllText(file).Select(c => c - '0').ToArray();

        var disk = new List<int>();
        var files = new Stack<(int Size, int Index)>();
        int id = 0;
        for (int i = 0; i < values.Length; i++) {
            for (int j = 0; j < values[i]; j++) {
                disk.Add( i % 2 == 0 ? id : -1);
            }

            if (i % 2 == 0) {
                files.Push((values[i], disk.Count - values[i]));
                id++;
            }
        }

        Console.WriteLine(Part1(disk.ToArray()));
        Console.WriteLine(Part2(disk.ToArray(), files));
    }

    private static long Part1(int[] disk) {
        int lo = 0;
        int hi = disk.Length - 1;
        while (true) {
            while (disk[lo] != -1) lo++;
            while (disk[hi] == -1) hi--;
            if (lo >= hi) break;

            (disk[lo], disk[hi]) = (disk[hi], disk[lo]);
        }

        long checksum = 0;
        for (int i = 0; i < disk.Length; i++) {
            if (disk[i] == -1) {
                break;
            }

            checksum += i * disk[i];
        }

        return checksum;
    }

    private static long Part2(int[] disk, Stack<(int Size, int Index)> files) {
        while (files.Count > 0) {
            (int Size, int Index) last = files.Pop();
            for (int i = 0; i < last.Index; i++) {
                int size = 0;
                while (disk[i + size] == -1) {
                    size++;
                }

                if (size >= last.Size) {
                    Array.Fill(disk, disk[last.Index], i, last.Size);
                    Array.Fill(disk, -1, last.Index, last.Size);
                    break;
                }
            }
        }

        long checksum = 0;
        for (int i = 0; i < disk.Length; i++) {
            if (disk[i] == -1) {
                continue;
            }

            checksum += i * disk[i];
        }

        return checksum;
    }
}
