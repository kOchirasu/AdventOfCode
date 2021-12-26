using System.Collections.Generic;

namespace UtilExtensions {
    public class DefaultDictionary<TK, TV> : Dictionary<TK, TV> {
        private readonly TV @default;

        public DefaultDictionary(TV @default = default) {
            this.@default = @default;
        }

        public new TV this[TK key] {
            get {
                if (!ContainsKey(key)) {
                    Add(key, @default);
                }

                return base[key];
            }
            set => base[key] = value;
        }
    }
}
