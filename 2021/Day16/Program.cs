using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Day16 {
    // https://adventofcode.com/
    public static class Program {
        private enum Op {
            Add = 0,
            Product = 1,
            Min = 2,
            Max = 3,
            Integer = 4,
            Greater = 5,
            Less = 6,
            Equal = 7,
        }

        public static void Main() {
            string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");
            var input = File.ReadAllLines(file)[0].ToCharArray().Select(c => {
                if (c is >= '0' and <= '9') {
                    return Convert.ToString(c - '0', 2).PadLeft(4, '0');
                }
                if (c is >= 'A' and <= 'F') {
                    return Convert.ToString(c - 'A' + 10, 2).PadLeft(4, '0');
                }

                throw new ArgumentException($"invalid char '{c}'");
            }).ToArray();

            var r = new PacketReader(string.Join("", input));
            long result = ParsePacket(r);

            Console.WriteLine(versionSum); // Part1
            Console.WriteLine(result);     // Part2
        }

        private static long versionSum;

        private static long ParsePacket(PacketReader r) {
            long version = r.ReadBits(3);
            versionSum += version;
            var type = (Op) r.ReadBits(3);

            var nums = new List<long>();
            switch (type) {
                case Op.Integer:
                    return r.ReadVarInt();
                default:
                    switch (r.ReadBits()) {
                        case 0: {
                            long length = r.ReadBits(15);
                            long start = r.Index;
                            while (r.Index - start < length) {
                                nums.Add(ParsePacket(r));
                            }
                            break;
                        }
                        case 1: {
                            long subPackets = r.ReadBits(11);
                            for (int i = 0; i < subPackets; i++) {
                                nums.Add(ParsePacket(r));
                            }
                            break;
                        }
                    }
                    break;
            }

            return type switch {
                Op.Add => nums.Sum(),
                Op.Product => nums.Aggregate<long, long>(1, (a, b) => a * b),
                Op.Max => nums.Max(),
                Op.Min => nums.Min(),
                Op.Greater => nums[0] > nums[1] ? 1 : 0,
                Op.Less => nums[0] < nums[1] ? 1 : 0,
                Op.Equal => nums[0] == nums[1] ? 1 : 0,
                _ => throw new ArgumentException($"Invalid op: {type}")
            };
        }
    }

    public class PacketReader {
        public int Index { get; private set; }
        private readonly string bits;

        public PacketReader(string bits) {
            this.bits = bits;
            Index = 0;
        }

        public long ReadBits(int n = 1) {
            string next = bits.Substring(Index, n);
            Index += n;
            return Convert.ToInt64(next, 2);
        }

        public long ReadVarInt() {
            var builder = new StringBuilder();
            long next;
            do {
                next = ReadBits(5);
                builder.Append(Convert.ToString(next & 0b1111, 2).PadLeft(4, '0'));
            } while (next >> 4 == 1);

            return Convert.ToInt64(builder.ToString(), 2);
        }
    }
}
