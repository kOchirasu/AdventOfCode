namespace Day22;

public class PathReader {
    private readonly string path;
    private int index;

    public PathReader(string path) {
        this.path = path;

        Reset();
    }

    public int Remaining => path.Length - index - 1;

    public bool NextInt(out int distance) {
        int start = index;
        while (char.IsDigit(path[index])) {
            index++;
        }

        if (start == index) {
            distance = 0;
            return false;
        }

        return int.TryParse(path[start..index], out distance);
    }

    public bool NextRotation(out char rotation) {
        if (path[index] is 'L' or 'R') {
            rotation = path[index++];
            return true;
        }

        rotation = '\0';
        return false;
    }

    public void Reset() {
        index = 0;
    }
}
