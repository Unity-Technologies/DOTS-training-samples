using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace Unity.Coding.Utils
{
    public static class LegacyExtensions
    {
        public static IEnumerable<Match> FixLegacy([NotNull] this MatchCollection @this)
            => @this.Cast<Match>();
    }

    public static class ObjectExtensions
    {
        // fluent operators - note that we're limiting to ref types where needed to avoid accidental boxing

        public static T ToBase<T>(this T @this) => @this; // sometimes you need an inline upcast

        public static T To<T>(this object @this) where T : class => (T)@this;
        public static T As<T>(this object @this) where T : class => @this as T;
        public static bool Is<T>(this object @this) where T : class => @this is T;
        public static bool IsNot<T>(this object @this) where T : class => !(@this is T);
    }

    public static class RefTypeExtensions
    {
        [NotNull]
        public static IEnumerable<T> WrapInEnumerable<T>(this T @this)
        { yield return @this; }

        [NotNull]
        public static IEnumerable<T> WrapInEnumerableOrEmpty<T>([CanBeNull] this T @this) where T : class
            => ReferenceEquals(@this, null) ? Enumerable.Empty<T>() : WrapInEnumerable(@this);
    }

    public static class TypeExtensions
    {
        public static object GetDefaultValue([NotNull] this Type @this)
        {
            object defaultValue = null;
            if (@this.IsValueType && @this != typeof(void))
                defaultValue = Activator.CreateInstance(@this);
            return defaultValue;
        }
    }

    public static class ComparableExtensions
    {
        public static T Clamp<T>([NotNull] this T @this, T min, T max) where T : IComparable<T>
        {
            if (min.CompareTo(max) > 0)
                throw new ArgumentException("'min' cannot be greater than 'max'", nameof(min));

            if (@this.CompareTo(min) < 0) return min;
            if (@this.CompareTo(max) > 0) return max;
            return @this;
        }
    }

    public static class ByteArrayExtensions
    {
        // if you want to speed this up, see https://stackoverflow.com/q/311165/14582
        public static string ToHexString([NotNull] this byte[] @this)
            => BitConverter.ToString(@this).Replace("-", "");
    }

    public static class ListExtensions
    {
        public static void SetRange<T>([NotNull] this List<T> @this, IEnumerable<T> collection)
        {
            @this.Clear();
            @this.AddRange(collection);
        }

        public static T PopBack<T>([NotNull] this IList<T> @this)
        {
            var item = @this[@this.Count - 1];
            @this.DropBack();
            return item;
        }

        public static void DropBack<T>([NotNull] this IList<T> @this)
        {
            @this.RemoveAt(@this.Count - 1);
        }
    }

    public static class StringCollectionExtensions
    {
        public static void AddRange([NotNull] this StringCollection @this, IEnumerable<string> collection)
        {
            foreach (var item in collection)
                @this.Add(item);
        }
    }
}
