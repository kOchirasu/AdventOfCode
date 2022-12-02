namespace Day2;

// https://adventofcode.com/
public static class Program {
    public static void Main() {
        string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");

        (int, int)[] input = File.ReadAllLines(file).Select(row => {
            string[] splits = row.Split(" ");
            return (splits[0][0] - 'A', splits[1][0] - 'X');
        }).ToArray();

        Console.WriteLine(Part1(input));
        Console.WriteLine(Part2(input));
    }

    private static int Part1((int, int)[] input) {
        int score = 0;
        foreach ((int a, int b) in input) {
            score += b + 1;
            if (a == b) {
                score += 3;
            } else if ((a + 1) % 3 == b) {
                score += 6;
            }
        }

        return score;
    }

    private static int Part2((int, int)[] input) {
        int score = 0;
        foreach ((int a, int b) in input) {
            switch (b) {
                case 0:
                    score += (a + 2) % 3 + 1;
                    break;
                case 1:
                    score += a + 1;
                    score += 3;
                    break;
                case 2:
                    score += (a + 1) % 3 + 1;
                    score += 6;
                    break;
            }
        }

        return score;
    }
}
