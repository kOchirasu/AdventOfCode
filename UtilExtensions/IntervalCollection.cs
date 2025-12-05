using System;
using System.Collections;
using System.Collections.Generic;

namespace UtilExtensions;

public class IntervalCollection : ICollection<Interval> {
    private SortedSet<Interval> intervals;

    public IntervalCollection() {
        intervals = new SortedSet<Interval>();
    }

    public IntervalCollection(IEnumerable<Interval> intervals) {
        this.intervals = new SortedSet<Interval>(intervals);
    }

    public void Reduce() {
        var stack = new Stack<Interval>();
        using SortedSet<Interval>.Enumerator enumerator = intervals.GetEnumerator();
        if (!enumerator.MoveNext()) {
            return;
        }
        stack.Push(enumerator.Current);

        while (enumerator.MoveNext()) {
            Interval current = enumerator.Current;
            Interval top = stack.Peek();

            if (top.End + 1 == current.Start || current.End + 1 == top.Start) {
                stack.Pop();
                stack.Push(top with {End = current.End});
            } else if (top.End.CompareTo(current.Start) < 0) {
                stack.Push(current);
            } else if (top.End.CompareTo(current.End) <= 0) {
                stack.Pop();
                stack.Push(top with {End = current.End});
            }
        }

        intervals = new SortedSet<Interval>(stack);
    }

    public void Clamp(long min, long max) {
        var result = new SortedSet<Interval>();
        foreach (Interval interval in intervals) {
            if (interval.End < min || interval.Start > max) {
                continue;
            }

            long start = Math.Max(interval.Start, min);
            long end = Math.Min(interval.End, max);
            result.Add(new Interval(start, end));
        }

        intervals = result;
    }

    public IEnumerator<Interval> GetEnumerator() => intervals.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void Add(long start, long end) => Add(new Interval(start, end));
    public void Add(Interval item) => intervals.Add(item);

    public void Clear() => intervals.Clear();

    public bool Contains(Interval item) => intervals.Contains(item);

    public bool Contains(long value) {
        foreach (Interval interval in intervals) {
            if (interval.Contains(value)) {
                return true;
            }
        }

        return false;
    }

    public void CopyTo(Interval[] array, int arrayIndex) => intervals.CopyTo(array, arrayIndex);

    public bool Remove(Interval remove) {
        var result = new SortedSet<Interval>();
        bool modified = false;
        foreach (Interval interval in intervals) {
            if (remove.Contains(interval)) {
                // Interval:  |---|
                // Remove:  |-------|
                // Result:
                modified = true;
                continue;
            }
            if (!interval.Overlaps(remove)) {
                // Interval:      |-----|
                // Remove:  |---|
                // Result:        |-----|
                result.Add(interval);
                continue;
            }

            if (remove.Start.CompareTo(interval.Start) <= 0) {
                // Interval:   |-----|
                // Remove:  |----|
                // Result:        |--|
                result.Add(interval with {Start = remove.End + 1});
            } else if (remove.End.CompareTo(interval.End) >= 0) {
                // Interval: |-----|
                // Remove:       |----|
                // Result:   |--|
                result.Add(interval with {End = remove.Start - 1});
            } else {
                // Interval: |---------|
                // Remove:      |--|
                // Result:   |-|    |--|
                result.Add(interval with {End = remove.Start - 1});
                result.Add(interval with {Start = remove.End + 1});
            }
        }

        intervals = result;
        return modified;
    }

    public int Count => intervals.Count;
    public bool IsReadOnly => false;
}

public record struct Interval(long Start, long End) : IComparable<Interval> {
    public long Size => End - Start + 1;

    public static implicit operator Interval(long n) {
        return new Interval(n, n);
    }

    public int CompareTo(Interval other) {
        int startComparison = Start.CompareTo(other.Start);
        return startComparison != 0 ? startComparison : End.CompareTo(other.End);
    }

    public bool Contains(Interval other) {
        return Start.CompareTo(other.Start) <= 0 && End.CompareTo(other.End) >= 0;
    }

    public bool Overlaps(Interval other) {
        return Start.CompareTo(other.End) <= 0 && End.CompareTo(other.Start) >= 0;
    }
}
