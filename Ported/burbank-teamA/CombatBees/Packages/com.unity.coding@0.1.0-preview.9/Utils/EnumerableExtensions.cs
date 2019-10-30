using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Enumerable = System.Linq.Enumerable;

namespace Unity.Coding.Utils
{
    public static class EnumerableExtensions
    {
        // should instead be an IReadOnlyList<T> (jetbrains apparently uses this type to detect "safe to double-walk",
        // but our framework doesn't have that type yet. so we have to hack it with ICollection, which is unfortunately
        // read-write. extra bad because we're "unwrapping" what should be a safe enumerable. compromises.. :(
        [NotNull]
        public static ICollection<T> UnDefer<T>([NotNull] this IEnumerable<T> @this)
            => @this as ICollection<T> ?? @this.ToList(); // don't use ToArray, it does extra work

        [NotNull]
        public static IEnumerable<T> WhereNotNull<T>([NotNull] this IEnumerable<T> @this) where T : class
            => @this.Where(item => !ReferenceEquals(item, null));

        [NotNull]
        public static IEnumerable<T> OrEmpty<T>([CanBeNull] this IEnumerable<T> @this)
            => @this ?? Enumerable.Empty<T>();

        public static T SingleOr<T>([NotNull] this IEnumerable<T> @this, T defaultValue)
        {
            using (var e = @this.GetEnumerator())
            {
                if (!e.MoveNext())
                    return defaultValue;
                var value = e.Current;
                if (e.MoveNext())
                    throw new InvalidOperationException("Sequence contains more than one element");
                return value;
            }
        }

        public static T SingleOr<T>([NotNull] this IEnumerable<T> @this, Func<T> defaultValueGenerator)
        {
            using (var e = @this.GetEnumerator())
            {
                if (!e.MoveNext())
                    return defaultValueGenerator();
                var value = e.Current;
                if (e.MoveNext())
                    throw new InvalidOperationException("Sequence contains more than one element");
                return value;
            }
        }

        [NotNull]
        public static HashSet<T> ToHashSet<T>([NotNull] this IEnumerable<T> @this, IEqualityComparer<T> comparer)
            => new HashSet<T>(@this, comparer);

        [NotNull]
        public static HashSet<T> ToHashSet<T>([NotNull] this IEnumerable<T> @this)
            => new HashSet<T>(@this);

        [NotNull]
        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>([NotNull] this IEnumerable<(TKey key, TValue value)> @this)
        => @this.ToDictionary(item => item.key, item => item.value);

        #if DOTNET_GREATER_EQUAL_4_71
        public static IEnumerable<T> Append<T>([NotNull] this IEnumerable<T> @this, T value)
        {
            foreach (var i in @this)
                yield return i;
            yield return value;
        }
        #endif

        #if DOTNET_GREATER_EQUAL_4_71
        public static IEnumerable<T> Prepend<T>([NotNull] this IEnumerable<T> @this, T value)
        {
            yield return value;
            foreach (var i in @this)
                yield return i;
        }

        #endif

        public static bool IsNullOrEmpty<T>([CanBeNull] this IEnumerable<T> @this)
        => @this == null || !@this.Any();

        public static IEnumerable<T> SelectMany<T>([NotNull] this IEnumerable<IEnumerable<T>> @this)
        => @this.SelectMany(_ => _);
    }
}
