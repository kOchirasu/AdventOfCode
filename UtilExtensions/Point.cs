using System;

namespace UtilExtensions;

public record struct Point(int Row, int Col) {
    public int X => Col;
    public int Y => Row;

    public static implicit operator Point((int Row, int Col) point) {
        return new Point(point.Row, point.Col);
    }

    public float Length() {
        return (float) Math.Sqrt(X * X + Y * Y);
    }

    public float Distance(Point other) {
        float dX = X - other.X;
        float dY = Y - other.Y;
        return (float) Math.Sqrt(dX * dX + dY * dY);
    }

    public int ManhattanDistance(Point other) {
        return Math.Abs(X - other.X) + Math.Abs(Y - other.Y);
    }

    public Point ManhattanDelta(Point other) {
        return (Row - other.Row, Col - other.Col);
    }

    public static Point operator +(in Point a, in Point b) => new(a.Row + b.Row, a.Col + b.Col);
    public static Point operator -(in Point a, in Point b) => new(a.Row - b.Row, a.Col - b.Col);
    public static Point operator -(in Point p) => new(-p.Row, -p.Col);
    public static Point operator *(in Point p, in Point b) => new(p.Row * b.Row, p.Col * b.Col);
    public static Point operator *(in Point p, int value) => new(p.Row * value, p.Col * value);
    public static Point operator /(in Point p, in Point b) => new(p.Row / b.Row, p.Col / b.Col);
    public static Point operator /(in Point p, int value) => new(p.Row / value, p.Col / value);
    public static Point operator %(in Point p, in Point other) => new(p.Row.PositiveMod(other.Row), p.Col.PositiveMod(other.Col));
    public static Point operator %(in Point p, int value) => new(p.Row.PositiveMod(value), p.Col.PositiveMod(value));

    public override string ToString() => $"({Row}, {Col})";
}
