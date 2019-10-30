using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Unity.Coding.Utils
{
    [PublicAPI]
    public static class EnumUtility
    {
        // about `where T : struct, IComparable, IConvertible, IFormattable`
        //
        // this is how we try to detect enums, since c# doesn't let us `where T : Enum`. works ok, not perfect.

        public static IReadOnlyList<string> GetNames<T>()
            where T : struct, IComparable, IConvertible, IFormattable
            => s_NameCache.GetValueOr(typeof(T)) ?? (s_NameCache[typeof(T)] = Enum.GetNames(typeof(T)));

        public static IReadOnlyList<string> GetLowercaseNames<T>()
            where T : struct, IComparable, IConvertible, IFormattable
            => GetOrAddLowerNameCache(typeof(T));

        public static IReadOnlyList<T> GetValues<T>()
            where T : struct, IComparable, IConvertible, IFormattable
        {
            var found =
                   s_ValueCache.GetValueOr(typeof(T))
                ?? (s_ValueCache[typeof(T)] = Enum.GetValues(typeof(T)));
            return (IReadOnlyList<T>)found;
        }

        public static T TryParseNoCase<T>(string enumName, T defaultValue = default)
            where T : struct, IComparable, IConvertible, IFormattable
            => Enum.TryParse(enumName, true, out T value) ? value : defaultValue;

        public static T TryParse<T>(string enumName, T defaultValue = default)
            where T : struct, IComparable, IConvertible, IFormattable
            => Enum.TryParse(enumName, false, out T value) ? value : defaultValue;

        public static T? TryParseNoCaseOr<T>(string enumName)
            where T : struct, IComparable, IConvertible, IFormattable
            => Enum.TryParse(enumName, true, out T value) ? (T?)value : null;

        public static T? TryParseOr<T>(string enumName)
            where T : struct, IComparable, IConvertible, IFormattable
            => Enum.TryParse(enumName, false, out T value) ? (T?)value : null;

        static IReadOnlyList<string> GetNameCache(Type enumType)
            => s_NameCache.GetOrAdd(enumType, Enum.GetNames);

        static IReadOnlyList<string> GetOrAddLowerNameCache(Type enumType)
        {
            return s_LowercaseNameCache.GetOrAdd(enumType, t =>
            {
                var names = GetNameCache(t);
                var lowerNames = names.ToLower().Distinct().ToArray();

                // EnumUtility doesn't care about this, but it is probably a sign of a bug elsewhere
                if (names.Count != lowerNames.Length)
                    throw new Exception($"Unexpected case insensitive duplicates found in enum {enumType.FullName}");

                return lowerNames;
            });
        }

        static Dictionary<Type, IReadOnlyList<string>> s_NameCache = new Dictionary<Type, IReadOnlyList<string>>();
        static Dictionary<Type, IReadOnlyList<string>> s_LowercaseNameCache = new Dictionary<Type, IReadOnlyList<string>>();
        static Dictionary<Type, object> s_ValueCache = new Dictionary<Type, object>();
    }
}
