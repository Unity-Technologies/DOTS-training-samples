using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Unity.Coding.Utils
{
    public class ReadOnlyDictionary
    {
        class EmptyDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>
        {
            public static readonly IReadOnlyDictionary<TKey, TValue> instance = new EmptyDictionary<TKey, TValue>();

            public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => Enumerable.Empty<KeyValuePair<TKey, TValue>>().GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
            public int Count => 0;
            public bool ContainsKey(TKey key) => false;
            public bool TryGetValue(TKey key, out TValue value) { value = default; return false; }
            public TValue this[TKey key] => throw new KeyNotFoundException();
            public IEnumerable<TKey> Keys => Enumerable.Empty<TKey>();
            public IEnumerable<TValue> Values => Enumerable.Empty<TValue>();
        }

        public static IReadOnlyDictionary<TKey, TValue> Empty<TKey, TValue>()
            => EmptyDictionary<TKey, TValue>.instance;
    }

    public static class Dictionary
    {
        [NotNull]
        public static Dictionary<TKey, TValue> Create<TKey, TValue>(params(TKey key, TValue value)[] items)
        => items.ToDictionary();
    }

    public static class DictionaryExtensions
    {
        [NotNull]
        public static IReadOnlyDictionary<TKey, TValue> OrEmpty<TKey, TValue>([CanBeNull] this IReadOnlyDictionary<TKey, TValue> @this)
            => @this ?? ReadOnlyDictionary.Empty<TKey, TValue>();

        public static TValue GetValueOr<TKey, TValue>([NotNull] this IDictionary<TKey, TValue> @this, TKey key, TValue defaultValue = default)
            => @this.TryGetValue(key, out var value) ? value : defaultValue;

        public static TValue GetOrAdd<TKey, TValue>([NotNull] this IDictionary<TKey, TValue> @this, TKey key, [NotNull] Func<TKey, TValue> createFunc)
        {
            if (@this.TryGetValue(key, out var found))
                return found;

            found = createFunc(key);
            @this.Add(key, found);
            return found;
        }

        public static IDictionary<TKey, TValue> AddRangeOverride<TKey, TValue>([NotNull] this IDictionary<TKey, TValue> @this, [NotNull] IDictionary<TKey, TValue> other)
        {
            foreach (var item in other)
                @this[item.Key] = item.Value;

            return @this;
        }
    }
}
