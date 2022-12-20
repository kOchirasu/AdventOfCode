using UtilExtensions;

namespace Day20;

// https://adventofcode.com/
public static class Program {
    public static void Main() {
        string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");

        IList<Reference> references = File.ReadAllText(file)
            .IntList()
            .Select((value, i) => new Reference(value, i))
            .ToList();

        Console.WriteLine(Part1(references));
        Console.WriteLine(Part2(references));
    }

    private static long Part1(IList<Reference> references) {
        // Reset indices
        for (int i = 0; i < references.Count; i++) {
            references[i].Index  = i;
        }
        IList<Reference> file = new List<Reference>(references);

        int cycleLength = file.Count - 1;
        foreach (Reference reference in references) {
            long shift = (reference.Value % cycleLength + cycleLength) % cycleLength;
            for (int i = 0; i < shift; i++) {
                int next = (reference.Index + 1 + file.Count) % file.Count;
                (file[reference.Index], file[next]) = (file[next], file[reference.Index]);

                file[reference.Index].Index = reference.Index;
                file[next].Index = next;
            }
        }

        int zeroIndex = FindZeroIndex(file);
        return file[(zeroIndex + 1000) % file.Count].Value
               + file[(zeroIndex + 2000) % file.Count].Value
               + file[(zeroIndex + 3000) % file.Count].Value;
    }

    private static long Part2(IList<Reference> references) {
        // Reset indices
        for (int i = 0; i < references.Count; i++) {
            references[i].Value *= 811589153;
            references[i].Index  = i;
        }
        IList<Reference> file = new List<Reference>(references);

        int cycleLength = file.Count - 1;
        for (int i = 0; i < 10; i++) {
            foreach (Reference reference in references) {
                long shift = (reference.Value % cycleLength + cycleLength) % cycleLength;
                for (int j = 0; j < shift; j++) {
                    int next = (reference.Index + 1 + file.Count) % file.Count;
                    (file[reference.Index], file[next]) = (file[next], file[reference.Index]);

                    file[reference.Index].Index = reference.Index;
                    file[next].Index = next;
                }
            }
        }

        int zeroIndex = FindZeroIndex(file);
        return file[(zeroIndex + 1000) % file.Count].Value
               + file[(zeroIndex + 2000) % file.Count].Value
               + file[(zeroIndex + 3000) % file.Count].Value;
    }

    private class Reference {
        public long Value;
        public int Index;

        public Reference(long value, int index) {
            Value = value;
            Index = index;
        }
    }

    private static int FindZeroIndex(IList<Reference> references) {
        foreach (Reference reference in references) {
            if (reference.Value == 0) {
                return reference.Index;
            }
        }
        throw new ArgumentException("0 not found in list");
    }
}
