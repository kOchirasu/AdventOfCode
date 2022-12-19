using UtilExtensions;

namespace Day19;

// https://adventofcode.com/
public static class Program {
    public static void Main() {
        string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");

        var blueprints = new Dictionary<int, Blueprint>();
        foreach (string line in File.ReadAllLines(file)) {
            var blueprint = new Blueprint(line);
            blueprints.Add(blueprint.Id, blueprint);
        }

        Console.WriteLine(Part1(blueprints));
        Console.WriteLine(Part2(blueprints));
    }

    private static int Part1(Dictionary<int, Blueprint> blueprints) {
        return blueprints.Values.AsParallel()
            .Select(blueprint => blueprint.Id * Simulate(blueprint, 24))
            .Sum();
    }

    private static int Part2(Dictionary<int, Blueprint> blueprints) {
        return blueprints.Values.Take(3)
            .Select(blueprint => Simulate(blueprint, 32))
            .Aggregate((a, b) => a * b);
    }

    private static int Simulate(Blueprint blueprint, byte minute) {
        var initialState = new State(0, 0, 0, 0, 1, 0, 0, 0, minute);
        var states = new HashSet<State>();

        var queue = new Queue<State>();
        queue.Enqueue(initialState);

        int max = 0;
        while (queue.TryDequeue(out State state)) {
            if (state.Minute <= 0) {
                max = Math.Max(max, state.Geode);
                continue;
            }

            if (states.Contains(state)) {
                continue;
            }
            states.Add(state);

            State nextState = state with {
                Ore = (short) (state.Ore + state.OreRobot),
                Clay = (short) (state.Clay + state.ClayRobot),
                Obsidian = (short) (state.Obsidian + state.ObsidianRobot),
                Geode = (short) (state.Geode + state.GeodeRobot),
                Minute = (byte) (state.Minute - 1),
            };
            if (state.Ore >= blueprint.GeodeCost.Ore && state.Obsidian >= blueprint.GeodeCost.Obsidian) {
                queue.Enqueue(nextState with {
                    Ore = (short) (nextState.Ore - blueprint.GeodeCost.Ore),
                    Obsidian = (short) (nextState.Obsidian - blueprint.GeodeCost.Obsidian),
                    GeodeRobot = (byte) (nextState.GeodeRobot + 1),
                });
            }
            if (state.OreRobot < blueprint.MaxOreCost && state.Ore >= blueprint.OreCost) {
                queue.Enqueue(nextState with {
                    Ore = (byte) (nextState.Ore - blueprint.OreCost),
                    OreRobot = (byte) (nextState.OreRobot + 1),
                });
            }
            if (state.ClayRobot < blueprint.ObsidianCost.Clay && state.Ore >= blueprint.ClayCost) {
                queue.Enqueue(nextState with {
                    Ore = (short) (nextState.Ore - blueprint.ClayCost),
                    ClayRobot = (byte) (nextState.ClayRobot + 1),
                });
            }
            if (state.ObsidianRobot < blueprint.GeodeCost.Obsidian && state.Ore >= blueprint.ObsidianCost.Ore && state.Clay >= blueprint.ObsidianCost.Clay) {
                queue.Enqueue(nextState with {
                    Ore = (short) (nextState.Ore - blueprint.ObsidianCost.Ore),
                    Clay = (short) (nextState.Clay - blueprint.ObsidianCost.Clay),
                    ObsidianRobot = (byte) (nextState.ObsidianRobot + 1),
                });
            }
            queue.Enqueue(nextState);
        }

        return max;
    }

    private record struct State(short Ore, short Clay, short Obsidian, short Geode, byte OreRobot, byte ClayRobot, byte ObsidianRobot, byte GeodeRobot, byte Minute);

    private class Blueprint {
        public readonly int Id;
        public readonly int OreCost;
        public readonly int ClayCost;
        public readonly (int Ore, int Clay) ObsidianCost;
        public readonly (int Ore, int Obsidian) GeodeCost;

        public readonly int MaxOreCost;

        public Blueprint(string line) {
            string[] splits = line.Split(":", StringSplitOptions.RemoveEmptyEntries);
            Id = splits[0].Extract(@"Blueprint (\d+)").Select(int.Parse).First();

            string[] descriptions = splits[1].Split(".", StringSplitOptions.RemoveEmptyEntries).Select(str => str.Trim()).ToArray();
            foreach (string description in descriptions) {
                string[] args = description.Split(" ");
                switch (args[1]) {
                    case "ore":
                        OreCost = int.Parse(args[4]);
                        break;
                    case "clay":
                        ClayCost = int.Parse(args[4]);
                        break;
                    case "obsidian":
                        ObsidianCost = (int.Parse(args[4]), int.Parse(args[7]));
                        break;
                    case "geode":
                        GeodeCost = (int.Parse(args[4]), int.Parse(args[7]));
                        break;
                }
            }

            MaxOreCost = Math.Max(Math.Max(OreCost, ClayCost), Math.Max(ObsidianCost.Ore, GeodeCost.Ore));
        }
    }
}
