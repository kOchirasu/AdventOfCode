using System.Collections.Generic;
using System.Linq;

namespace UtilExtensions;

public class EHashSet<T> : HashSet<T> {
    public EHashSet(IEnumerable<T> enumerable) : base(enumerable) { }

    public static EHashSet<T> operator |(EHashSet<T> left, IEnumerable<T> right) {
        return new EHashSet<T>(left.Union(right));
    }

    public static EHashSet<T> operator -(EHashSet<T> left, IEnumerable<T> right) {
        return new EHashSet<T>(left.Except(right));
    }

    public static EHashSet<T> operator &(EHashSet<T> left, IEnumerable<T> right) {
        return new EHashSet<T>(left.Intersect(right));
    }

    public static EHashSet<T> operator ^(EHashSet<T> left, IEnumerable<T> right) {
        return (left | right) - (left & right);
    }

    public static bool operator ==(EHashSet<T> left, IEnumerable<T> right) {
        return left.SetEquals(right);
    }

    public static bool operator !=(EHashSet<T> left, IEnumerable<T> right) {
        return !(left == right);
    }
}
