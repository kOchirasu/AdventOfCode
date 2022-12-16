using UtilExtensions;

namespace Day16;

// https://adventofcode.com/
public static class Program {
    private static readonly Dictionary<string, int> rates = new();
    private static readonly Dictionary<string, string[]> tunnels = new();
    
    private static readonly Dictionary<string, int> solveCache = new();
    private static readonly Dictionary<int, int> minuteMaxRate = new();

    public static void Main() {
        string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");

        foreach (string line in File.ReadAllLines(file)) {
            string[] splits = line.Split(";");

            string valve = splits[0].Extract(@"([A-Z]{2})").First();
            int rate = int.Parse(splits[0].Extract(@"(\d+)").First());
            string[] connected = splits[1].Extract(@"(?:([A-Z]{2})[, ]{0,2})+");

            rates.Add(valve, rate);
            tunnels.Add(valve, connected);
        }

        Console.WriteLine(Part1(new HashSet<string>(), "AA", 30, 0));
        Console.WriteLine(Part2(new HashSet<string>(), "AA", "AA", 26, 0));
    }

    private static int Part1(HashSet<string> opened, string current, int minute, int rate) {
        string key = $"{current}|{minute}|{rate}";
        if (solveCache.TryGetValue(key, out int result)) {
            return result;
        }
        result = Solve(opened, current, minute, rate);
        solveCache.Add(key, result);
        return result;

        // Local solve function
        int Solve(HashSet<string> opened, string current, int minute, int rate) {
            if (minute <= 0) {
                return 0;
            }

            int max = 0;
            // If there isn't over 1 minute left, there's no benefit to opening a valve.
            if (minute > 1) {
                bool canOpenCurrent = !opened.Contains(current) && rates[current] > 0;
                // Open current
                if (canOpenCurrent) {
                    opened.Add(current); // Open
                    max = Math.Max(max, rate + Part1(opened, current, minute - 1, rate + rates[current]));
                    opened.Remove(current); // Undo
                }
            }

            // Move without opening
            foreach (string tunnel in tunnels[current]) {
                if (tunnel[0] == tunnel[1]) continue;
                max = Math.Max(max, rate + Part1(opened, tunnel, minute - 1, rate));
            }

            return max;
        }
    }

    private static int Part2(HashSet<string> opened, string current, string elephant, int minute, int rate) {
        string key = $"{current}|{elephant}|{minute}|{rate}";
        if (solveCache.TryGetValue(key, out int result)) {
            return result;
        }
        result = Solve(opened, current, elephant, minute, rate);
        solveCache.Add(key, result);
        return result;

        // Local solve function
        int Solve(HashSet<string> opened, string current, string elephant, int minute, int rate) {
            if (minute <= 0) {
                return 0;
            }

            if (minuteMaxRate.TryGetValue(minute, out int maxMinuteRate)) {
                minuteMaxRate[minute] = Math.Max(rate, maxMinuteRate);

                // Arbitrary cutoff ratio to reduce search space.
                if (rate < maxMinuteRate * 0.75) {
                    return 0;
                }
            } else {
                minuteMaxRate[minute] = rate;
            }

            int max = 0;
            // If there isn't over 1 minute left, there's no benefit to opening a valve.
            if (minute > 1) {
                bool canOpenCurrent = !opened.Contains(current) && rates[current] > 0;
                if (current != elephant) {
                    bool canOpenElephant = !opened.Contains(elephant) && rates[elephant] > 0;
                    
                    // Open elephant
                    if (canOpenElephant) {
                        opened.Add(elephant); // Open
                        foreach (string tunnel in tunnels[current]) {
                            max = Math.Max(max, rate + Part2(opened, tunnel, elephant, minute - 1, rate + rates[elephant]));
                        }
                        opened.Remove(elephant); // Undo
                    }

                    // Open both
                    if (canOpenCurrent && canOpenElephant) {
                        opened.Add(current); // Open
                        opened.Add(elephant); // Open
                        max = Math.Max(max, rate + Part2(opened, current, elephant, minute - 1, rate + rates[current] + rates[elephant]));
                        opened.Remove(current); // Undo
                        opened.Remove(elephant); // Undo
                    }
                }

                // Open current
                if (canOpenCurrent) {
                    opened.Add(current); // Open
                    foreach (string tunnel in tunnels[elephant]) {
                        max = Math.Max(max, rate + Part2(opened, current, tunnel, minute - 1, rate + rates[current]));
                    }
                    opened.Remove(current); // Undo
                }
            }

            // Move without opening
            foreach (string[] tunnel in Itertools.Product(tunnels[current], tunnels[elephant])) {
                if (tunnel[0] == tunnel[1]) continue;
                max = Math.Max(max, rate + Part2(opened, tunnel[0], tunnel[1], minute - 1, rate));
            }

            return max;
        }
    }
}
