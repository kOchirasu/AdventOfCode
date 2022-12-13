using System.Diagnostics;
using System.Text;

namespace Day13;

public class Packet : IComparable<Packet> {
    private readonly bool isInt;
    private readonly int intValue;
    private readonly List<Packet> subPackets = new();

    public bool IsLocator = false;

    private Packet(Packet packet) {
        Debug.Assert(packet.isInt, "converting list to packet");
        subPackets.Add(packet);
    }

    public Packet(string input) {
        if (int.TryParse(input, out int value)) {
            isInt = true;
            intValue = value;
        } else {
            foreach (string sub in Parse(input)) {
                subPackets.Add(new Packet(sub));
            }
        }
    }

    private static IEnumerable<string> Parse(string input) {
        if (int.TryParse(input, out int _)) {
            yield break;
        }

        var currentNum = new StringBuilder();
        for (int i = 0; i < input.Length; i++) {
            if (input[i] == ',') {
                yield return currentNum.ToString();
                currentNum.Clear();
            } else if (char.IsDigit(input[i])) {
                currentNum.Append(input[i]);
            } else if (input[i] == '[') {
                int start = i + 1;
                int brackets = 0;
                do {
                    if (input[i] == '[') {
                        brackets++;
                    } else if (input[i] == ']') {
                        brackets--;
                    }
                    i++;
                } while (brackets > 0);
                int end = i - 1;
                yield return input[start..end];
            }
        }
        if (currentNum.Length > 0) {
            yield return currentNum.ToString();
        }
    }

    public int CompareTo(Packet? other) {
        if (other == null) {
            throw new ArgumentException("Invalid compare to null");
        }

        if (isInt && other.isInt) {
            return intValue.CompareTo(other.intValue);
        }
        if (!isInt && !other.isInt) {
            int total = Math.Max(subPackets.Count, other.subPackets.Count);
            for (int i = 0; i < total; i++) {
                if (i >= subPackets.Count) {
                    return -1;
                }
                if (i >= other.subPackets.Count) {
                    return 1;
                }
                int compareResult = subPackets[i].CompareTo(other.subPackets[i]);
                if (compareResult == 0) {
                    continue;
                }
                return compareResult;
            }
            return 0; // Continue;
        }

        if (isInt) {
            var packet = new Packet(this);
            return packet.CompareTo(other);
        } else {
            var packet = new Packet(other);
            return CompareTo(packet);
        }
    }

    public override string ToString() {
        if (isInt) {
            return $"{intValue}";
        }

        return $"[{string.Join(",", subPackets)}]";
    }
}
