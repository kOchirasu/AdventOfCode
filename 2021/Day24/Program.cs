using System.Text.RegularExpressions;
using UtilExtensions;

namespace Day24 {
    // https://adventofcode.com/
    public static class Program {
        public static void Main() {
            string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");
            string[] blocks = File.ReadAllText(file).Split("inp w").Skip(1).ToArray();

            var rules = new (int, int)[14];
            var stack = new Stack<(int, int)>();
            for (int i = 0; i < 14; i++) {
                string step = blocks[i];
                if (step.Contains("div z 26")) { // Reduce Z step
                    Match match = Regex.Match(step, "add x (-?\\d+)");
                    int addX = int.Parse(match.Groups[1].Value);
                    (int index, int addY) = stack.Pop();

                    int offset = addY + addX;
                    rules[index] = (i, offset);
                    rules[i] = (index, -offset);
                } else { // Increase Z step
                    Match match = Regex.Match(step, "add y w\r\nadd y (-?\\d+)");
                    int addY = int.Parse(match.Groups[1].Value);
                    stack.Push((i, addY));
                }
            }

            Console.WriteLine(Part1(rules));
            Console.WriteLine(Part2(rules));
        }

        private static long Part1((int, int)[] rules) {
            long number = 0;
            for (int i = 0; i < 14; i++) {
                number *= 10;
                number += Math.Min(9 - rules[i].Item2, 9);
            }

            return number;
        }

        private static long Part2((int, int)[] rules) {
            long number = 0;
            for (int i = 0; i < 14; i++) {
                number *= 10;
                number += Math.Max(1 - rules[i].Item2, 1);
            }

            return number;
        }

        // Interpreter
        private static void Cli() {
            string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");
            Instruction[] instructions = File.ReadAllLines(file)
                .Where(row => !row.StartsWith("#"))
                .Select(row => {
                    string[] split = row.Split(" ");
                    Operation op = split[0] switch {
                        "inp" => Operation.Input,
                        "add" => Operation.Add,
                        "mul" => Operation.Multiply,
                        "div" => Operation.Divide,
                        "mod" => Operation.Mod,
                        "eql" => Operation.Equals,
                        "set" => Operation.Set,
                        "print" => Operation.Print,
                        _ => throw new ArgumentException($"invalid operation: {split[0]}")
                    };

                    string a = split[1];
                    string b = string.Empty;
                    if (split.Length > 2) {
                        b = split[2];
                    }

                    return new Instruction(op, a, b);
                }).ToArray();

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

        private record Instruction(Operation Op, string A, string B);

        private enum Operation {
            Input,
            Add,
            Multiply,
            Divide,
            Mod,
            Equals,
            Set,
            Print,
        }

        private static DefaultDictionary<string, long> Execute(Instruction[] instructions, long input) {
            int[] inputs = input.ToString().ToCharArray().Select(c => c - '0').ToArray();
            int i = 0;
            DefaultDictionary<string, long> variables = new();
            foreach ((Operation op, string a, string b) in instructions) {
                long bVal = long.TryParse(b, out long val) ? val : variables[b];
                switch (op) {
                    case Operation.Input:
                        variables[a] = inputs[i];
                        i++;
                        break;
                    case Operation.Add:
                        variables[a] += bVal;
                        break;
                    case Operation.Multiply:
                        variables[a] *= bVal;
                        break;
                    case Operation.Divide:
                        variables[a] /= bVal;
                        break;
                    case Operation.Mod:
                        variables[a] %= bVal;
                        break;
                    case Operation.Equals:
                        variables[a] = variables[a] == bVal ? 1 : 0;
                        break;
                    case Operation.Set:
                        variables[a] = bVal;
                        break;
                    case Operation.Print:
                        Console.WriteLine($"{a}: {variables[a]}");
                        break;
                    default:
                        throw new ArgumentException($"Invalid instruction: {op}");
                }
            }

            variables.Remove(string.Empty); // Useless placeholder
            return variables;
        }
    }
}
