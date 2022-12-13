using System.Diagnostics;
using System.Text;
using UtilExtensions.Trees;

namespace Day13;

public class Packet : TreeNode<int>, IComparable<Packet> {
    public bool IsLocator = false;

    private Packet(Packet packet) {
        Debug.Assert(packet.Value >= 0, "converting list to packet");
        Value = -1;
        AddChild(packet);
    }

    public Packet(string input) {
        if (int.TryParse(input, out int value)) {
            Value = value;
        } else {
            Value = -1;
            foreach (string sub in Parse(input)) {
                AddChild(new Packet(sub));
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

        if (Value >= 0 && other.Value >= 0) {
            return Value.CompareTo(other.Value);
        }
        if (Value < 0 && other.Value < 0) {
            int total = Math.Max(Children.Count, other.Children.Count);
            for (int i = 0; i < total; i++) {
                if (i >= Children.Count) {
                    return -1;
                }
                if (i >= other.Children.Count) {
                    return 1;
                }
                if (Children[i] is not Packet packet) {
                    throw new ArgumentException("Compare invalid packet");
                }
                int compareResult = packet.CompareTo(other.Children[i] as Packet);
                if (compareResult == 0) {
                    continue;
                }
                return compareResult;
            }
            return 0; // Continue;
        }

        if (Value >= 0) {
            var packet = new Packet(this);
            return packet.CompareTo(other);
        } else {
            var packet = new Packet(other);
            return CompareTo(packet);
        }
    }
}
