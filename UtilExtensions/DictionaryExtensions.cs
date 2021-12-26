using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilExtensions {
    public static class DictionaryExtensions {
        public static TV GetOrCreate<TK, TV>(this IDictionary<TK, TV> dict, TK key, TV @default = default) {
            if (!dict.TryGetValue(key, out TV value)) {
                value = @default;
                dict[key] = value;
            }

            return value;
        }

        public static TK GetByValue<TK, TV>(this IDictionary<TK, TV> dict, TV get) {
            foreach ((TK key, TV value) in dict) {
                if (EqualityComparer<TV>.Default.Equals(get, value)) {
                    return key;
                }
            }

            return default;
        }

        public static bool OneToOne<TK, TV>(this IDictionary<TK ,TV> dict) {
            var values = new HashSet<TV>();
            foreach ((TK _, TV value) in dict) {
                if (values.Contains(value)) {
                    return false;
                }

                values.Add(value);
            }

            return true;
        }

        public static Dictionary<TV, TK> Reverse<TK, TV>(this IDictionary<TK, TV> dict) {
            if (!dict.OneToOne()) {
                throw new ArgumentException("Dictionary must be one-to-one to reverse.");
            }

            var result = new Dictionary<TV, TK>();
            foreach ((TK key, TV value) in dict) {
                result[value] = key;
            }

            return result;
        }

        public static IEnumerable<KeyValuePair<TK, TV>> Sort<TK, TV>(this IDictionary<TK, TV> dict) {
            return dict.OrderBy(pair => pair.Key);
        }

        public static IEnumerable<KeyValuePair<TK, TV>> SortByValue<TK, TV>(this IDictionary<TK, TV> dict) {
            return dict.OrderBy(pair => pair.Value);
        }

        public static string PrettyPrint<TK, TV>(this IDictionary<TK, TV> dict) {
            var builder = new StringBuilder();
            foreach ((TK k, TV v) in dict) {
                builder.AppendLine($"{k}: {v}");
            }

            return builder.ToString();
        }
    }
}
