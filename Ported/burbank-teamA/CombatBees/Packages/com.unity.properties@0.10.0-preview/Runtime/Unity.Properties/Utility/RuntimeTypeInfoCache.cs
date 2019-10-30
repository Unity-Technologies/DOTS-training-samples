using System;
using System.Reflection;

namespace Unity.Properties
{
    struct RuntimeTypeInfoCache
    {
        public static bool IsContainerType(Type type) => !(type.IsPrimitive || type.IsEnum || type == typeof(string) || type == typeof(Guid));
    }

    /// <summary>
    /// Helper class to avoid paying the cost of runtime type lookups.
    ///
    /// This is also used to abstract underlying type info in the runtime (e.g. RuntimeTypeHandle vs StaticTypeReg)
    /// </summary>
    struct RuntimeTypeInfoCache<T>
    {
        static readonly bool s_IsPrimitive;
        static readonly bool s_IsValueType;
        static readonly bool s_IsInterface;
        static readonly bool s_IsAbstract;
        static readonly bool s_IsEnum;
        static readonly bool s_IsEnumFlags;
        static readonly bool s_IsArray;
        static readonly bool s_IsString;
        static readonly bool s_IsGuid;
        

        static RuntimeTypeInfoCache()
        {
            var type = typeof(T);
            s_IsPrimitive = type.IsPrimitive;
            s_IsValueType = type.IsValueType;
            s_IsInterface = type.IsInterface;
            s_IsAbstract = type.IsAbstract;
            s_IsArray = type.IsArray;
            s_IsEnum = type.IsEnum;
            s_IsEnumFlags = s_IsEnum && null != type.GetCustomAttribute<FlagsAttribute>();
            s_IsString = typeof(T) == typeof(string);
            s_IsGuid = typeof(T) == typeof(Guid);
        }

        public static bool IsValueType() => s_IsValueType;

        public static bool IsInterface() => s_IsInterface;

        public static bool IsAbstract() => s_IsAbstract;

        public static bool IsArray() => s_IsArray;

        public static bool IsContainerType() => !(s_IsPrimitive || s_IsEnum || s_IsString || s_IsGuid);

        public static bool IsAbstractOrInterface() => s_IsAbstract || s_IsInterface;

        public static bool IsFlagsEnum() => s_IsEnumFlags;
    }
}
