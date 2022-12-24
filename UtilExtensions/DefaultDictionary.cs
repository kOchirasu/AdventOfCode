using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace UtilExtensions;

public class DefaultDictionary<TK, TV> : Dictionary<TK, TV> {
    private readonly Func<TV> @default;

    public DefaultDictionary(TV @default = default(TV)) {
        this.@default = () => @default;
    }

    public DefaultDictionary(Func<TV> @default) {
        this.@default = @default;
    }

    public new TV this[TK key] {
        [CollectionAccess(CollectionAccessType.Read | CollectionAccessType.UpdatedContent)]
        get {
            if (!ContainsKey(key)) {
                Add(key, @default());
            }

            return base[key];
        }
        [CollectionAccess(CollectionAccessType.UpdatedContent)]
        set => base[key] = value;
    }
}
