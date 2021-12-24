using UtilExtensions;

namespace Day24 {
    // https://adventofcode.com/
    public static class Program {
        public static void Main() {
            string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");
            var input = File.ReadAllLines(file).Select(row => {
                if (row.StartsWith('#')) return default;

                string[] split = row.Split(" ");
                Instruction.Operation op;
                switch (split[0]) {
                    case "inp":
                        op = Instruction.Operation.Input;
                        break;
                    case "add":
                        op = Instruction.Operation.Add;
                        break;
                    case "mul":
                        op = Instruction.Operation.Multiply;
                        break;
                    case "div":
                        op = Instruction.Operation.Divide;
                        break;
                    case "mod":
                        op = Instruction.Operation.Mod;
                        break;
                    case "eql":
                        op = Instruction.Operation.Equals;
                        break;
                    case "set":
                        op = Instruction.Operation.Set;
                        break;
                    case "print":
                        op = Instruction.Operation.Print;
                        break;
                    default:
                        throw new ArgumentException($"invalid operation: {split[0]}");
                }

                string a = split[1];
                string b = null;
                if (split.Length > 2) {
                    b = split[2];
                }

                return new Instruction(op, a, b);
            }).Where(i => i != default).ToArray();

            //Cli(input);

            /*** Solution ***
             * w5 = w4
             * w6 = w3 + 4
             * w7 = w2 + 8
             * w10 = w9 + 6
             * w11 = w8 + 5
             * w12 = w1 - 6
             * w14 = w13 - 4
             ***************/
            Console.WriteLine(Part1(input, 91599994399395L));
            Console.WriteLine(Part2(input, 71111591176151L));
        }

        private static void Cli(Instruction?[] instructions) {
            while (true) {
                string input = Console.ReadLine() ?? string.Empty;
                if (string.IsNullOrEmpty(input)) continue;

                if (!long.TryParse(input, out long test)) {
                    Console.WriteLine("Quitting...");
                    return;
                }

                DefaultDictionary<string, long> output = Execute(instructions, test);

                Console.WriteLine($"Input: {input}");
                foreach ((string key, long value)in output) {
                    Console.WriteLine($"{key}: {value}");
                }

                if (output["z"] == 0) {
                    Console.WriteLine($"Valid solution: {input}");
                }

                Console.WriteLine("======================");
            }
        }

        public record Instruction(Instruction.Operation Op, string A, string B) {
            public enum Operation {
                Input,
                Add,
                Multiply,
                Divide,
                Mod,
                Equals,
                Set,
                Print,
            }

            public override string ToString() {
                return $"{Op} {A} {B}";
            }
        }

        private static DefaultDictionary<string, long> Execute(Instruction?[] instructions, long input) {
            int[] inputs = input.ToString().ToCharArray().Select(c => c - '0').ToArray();
            int i = 0;
            DefaultDictionary<string, long> variables = new();
            foreach ((Instruction.Operation op, string? a, string? b) in instructions) {
                switch (op) {
                    case Instruction.Operation.Input:
                        variables[a] = inputs[i];
                        i++;
                        break;
                    case Instruction.Operation.Add:
                        if (int.TryParse(b, out int addB)) {
                            variables[a] += addB;
                        } else {
                            variables[a] += variables[b];
                        }
                        break;
                    case Instruction.Operation.Multiply:
                        if (int.TryParse(b, out int mulB)) {
                            variables[a] *= mulB;
                        } else {
                            variables[a] *= variables[b];
                        }
                        break;
                    case Instruction.Operation.Divide:
                        if (int.TryParse(b, out int divB)) {
                            variables[a] /= divB;
                        } else {
                            variables[a] /= variables[b];
                        }
                        break;
                    case Instruction.Operation.Mod:
                        if (int.TryParse(b, out int modB)) {
                            variables[a] %= modB;
                        } else {
                            variables[a] %= variables[b];
                        }
                        break;
                    case Instruction.Operation.Equals:
                        if (int.TryParse(b, out int eqB)) {
                            variables[a] = variables[a] == eqB ? 1 : 0;
                        } else {
                            variables[a] = variables[a] == variables[b] ? 1 : 0;
                        }
                        break;
                    case Instruction.Operation.Set:
                        if (int.TryParse(b, out int setB)) {
                            variables[a] = setB;
                        } else {
                            variables[a] = variables[b];
                        }
                        break;
                    case Instruction.Operation.Print:
                        Console.WriteLine($"{a} is {variables[a]}");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("invalid operation");
                }
            }

            return variables;
        }

        private static bool Part1(Instruction?[] instructions, long num) {
            Console.WriteLine(num);
            DefaultDictionary<string, long> vars = Execute(instructions, num);
            return vars["z"] == 0;
        }

        private static bool Part2(Instruction?[] instructions, long num) {
            Console.WriteLine(num);
            DefaultDictionary<string, long> vars = Execute(instructions, num);
            return vars["z"] == 0;
        }
    }
}
