using UtilExtensions;

namespace Day17;

// https://adventofcode.com/
public static class Program {


    public static void Main() {
        string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");

        string[] groups = File.ReadAllText(file).Groups();
        long[] registers = groups[0].ExtractAll<long>(@"(\d+)").SelectMany(x => x).ToArray();
        int[,] ops = groups[1].ExtractAll<int>(@"(\d+),(\d+)").ToArray().UnJagged();

        Console.WriteLine(Part1(registers, ops));
        Console.WriteLine(Part2(ops));
    }

    private static long[] Simulate(long[] register, int[,] ops) {
        var output = new List<long>();
        for (int i = 0; i < ops.RowCount(); i++) {
            long combo = ops[i, 1] switch {
                0 or 1 or 2 or 3 => ops[i, 1],
                4 => register[0],
                5 => register[1],
                6 => register[2],
                _ => throw new ArgumentException($"INVALID COMBO OP: {ops[i, 1]}")
            };

            switch (ops[i, 0]) {
                // The adv instruction (opcode 0) performs division.
                // - The numerator is the value in the A register.
                // - The denominator is found by raising 2 to the power of the instruction's combo operand.
                //   (So, an operand of 2 would divide A by 4 (2^2); an operand of 5 would divide A by 2^B.)
                // - The result of the division operation is truncated to an integer and then written to the A register.
                case 0:
                    register[0] = (long) (register[0] / Math.Pow(2, combo));
                    break;
                // The bxl instruction (opcode 1) calculates the bitwise XOR of register B and the instruction's literal operand,
                // then stores the result in register B.
                case 1:
                    register[1] ^= ops[i, 1];
                    break;
                // The bst instruction (opcode 2) calculates the value of its combo operand modulo 8 (thereby keeping only its lowest 3 bits),
                // then writes that value to the B register.
                case 2:
                    register[1] = combo % 8;
                    break;
                // The jnz instruction (opcode 3) does nothing if the A register is 0.
                // However, if the A register is not zero, it jumps by setting the instruction pointer to the value of its literal operand;
                // if this instruction jumps, the instruction pointer is not increased by 2 after this instruction.
                case 3:
                    if (register[0] == 0) {
                        break;
                    }

                    i = ops[i, 1] - 1;
                    break;
                // The bxc instruction (opcode 4) calculates the bitwise XOR of register B and register C,
                // then stores the result in register B.
                // (For legacy reasons, this instruction reads an operand but ignores it.)
                case 4:
                    register[1] ^= register[2];
                    break;
                // The out instruction (opcode 5) calculates the value of its combo operand modulo 8, then outputs that value.
                // (If a program outputs multiple values, they are separated by commas.)
                case 5:
                    output.Add(combo % 8);
                    break;
                // The bdv instruction (opcode 6) works exactly like the adv instruction except that the result is stored in the B register.
                // (The numerator is still read from the A register.)
                case 6:
                    register[1] = (long) (register[0] / Math.Pow(2, combo));
                    break;
                // The cdv instruction (opcode 7) works exactly like the adv instruction except that the result is stored in the C register.
                // (The numerator is still read from the A register.)
                case 7:
                    register[2] = (long) (register[0] / Math.Pow(2, combo));
                    break;
            }
        }

        return output.ToArray();
    }

    private static string Part1(long[] register, int[,] ops) {
        return string.Join(",", Simulate(register, ops));
    }

    private static bool ArrayMatch(long[] result, long[] target, int count) {
        if (count < 0 || count >= result.Length || count >= target.Length) {
            return false;
        }

        for (int i = 0; i < count; i++) {
            if (result[i] != target[i]) {
                return false;
            }
        }

        return true;
    }

    private static long TruncateOctDigits(long value, int digits) {
        return value & (1L << 3 * digits) - 1;
    }

    private static long Part2(int[,] ops) {
        long[] target = ops.Flatten()
            .Select(n => (long) n)
            .ToArray();

        var prevValues = new HashSet<long> {0};
        for (int i = 0; i < target.Length; i++) {
            var nextValues = new HashSet<long>();
            foreach (long prevValue in prevValues) {
                // Since there can be a shift of up to 7 bits, we must check in groups of 8 bits.
                for (long j = 0; j <= 0b11111111; j++) {
                    long tryVal = j << 3 * i | prevValue;

                    long[] result = Simulate([tryVal, 0, 0], ops);
                    if (ArrayMatch(result, target, i)) {
                        // We truncate here to preemptively dedupe.
                        nextValues.Add(TruncateOctDigits(tryVal, i + 1));
                    }
                }
            }

            prevValues.Clear();
            foreach (long nextValue in nextValues) {
                prevValues.Add(nextValue);
            }
        }

        // Filter valid results and find minimum value.
        return prevValues.Where(value => {
            long[] result = Simulate([value, 0, 0], ops);
            return target.SequenceEqual(result);
        }).Min();
    }
}
