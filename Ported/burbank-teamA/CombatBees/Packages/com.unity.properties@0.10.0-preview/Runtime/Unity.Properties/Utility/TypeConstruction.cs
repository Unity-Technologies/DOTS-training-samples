using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine.Assertions;

namespace Unity.Properties
{
    public static class TypeConstruction
    {
        private static readonly MethodInfo HasParameterLessConstructorMethod;

        static TypeConstruction()
        {
            HasParameterLessConstructorMethod = typeof(TypeConstruction)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .First(m => m.Name == nameof(HasParameterLessConstructor)
                            && m.IsGenericMethod
                            && m.GetGenericArguments().Length == 1);
            Assert.IsNotNull(HasParameterLessConstructorMethod);

            RegisterBuiltInConstructors();
        }

        private static void RegisterBuiltInConstructors()
        {
            TypeConstructionCache<string>.ExplicitConstruction = () => string.Empty;
        }

        public static bool HasParameterLessConstructor(Type type)
        {
            return (bool)HasParameterLessConstructorMethod
                .MakeGenericMethod(type)
                .Invoke(null, null);
        }

        public static bool HasParameterLessConstructor<T>()
        {
            return TypeConstructionCache<T>.HasParameterLessConstructor;
        }

        public static bool HasConstructionDelegate<T>()
        {
            return TypeConstructionCache<T>.HasExplicitConstructor;
        }

        public static bool CanBeConstructed<T>()
        {
            return TypeConstructionCache<T>.CanBeConstructed();
        }

        public static bool CanBeConstructedFromDerivedType<T>()
        {
            return TypeConstructionCache<T>.ConstructibleTypes.Count >
                   (TypeConstructionCache<T>.CanBeConstructed() ? 1 : 0);
        }

        public static IReadOnlyList<Type> GetAllConstructibleTypes<T>()
        {
            return TypeConstructionCache<T>.ConstructibleTypes;
        }

        public static TType Construct<TType>()
        {
            return TypeConstructionCache<TType>.Construct();
        }

        public static bool TryConstruct<TType>(out TType value)
        {
            return TypeConstructionCache<TType>.TryConstruct(out value);
        }

        public static TType Construct<TType>(Type derivedType)
        {
            if (typeof(TType).IsAssignableFrom(derivedType))
            {
                return TypeConstructionCache<TType>.Construct(derivedType);
            }

            throw new ArgumentException($"Could not create instance of type `{derivedType.Name}` and convert to `{typeof(TType).Name}`: given type is not assignable to target type.");
        }

        public static bool TryConstruct<TType>(Type derivedType, out TType value)
        {
            if (typeof(TType).IsAssignableFrom(derivedType))
            {
                return TypeConstructionCache<TType>.TryConstruct(derivedType, out value);
            }

            value = default;
            return false;
        }

        public static void SetExplicitConstructionMethod<TType>(Func<TType> constructor)
        {
            TypeConstructionCache<TType>.ExplicitConstruction = constructor;
        }

        public static void UnsetExplicitConstructionMethod<TType>(Func<TType> constructor)
        {
            if (TypeConstructionCache<TType>.ExplicitConstruction == constructor)
            {
                TypeConstructionCache<TType>.ExplicitConstruction = null;
            }
        }

        internal static class TypeConstructionCache<TType>
        {
            public static Func<TType> ExplicitConstruction;

            class ConstructInfo
            {
                public readonly Type Type;
                public readonly Func<bool> Constructible;
                public readonly Func<TType> Construct;

                public ConstructInfo(Type type, Func<bool> del, Func<TType> construct)
                {
                    Type = type;
                    Constructible = del;
                    Construct = construct;
                }
            }
            private static readonly List<ConstructInfo> k_PotentialConstructibleTypes = new List<ConstructInfo>();

            public static bool HasParameterLessConstructor { get; }
            public static bool HasExplicitConstructor => null != ExplicitConstruction;
            public static bool CanBeConstructed() => HasExplicitConstructor || typeof(UnityEngine.ScriptableObject).IsAssignableFrom(typeof(TType)) || HasParameterLessConstructor;
            public static IReadOnlyList<Type> ConstructibleTypes
                => k_PotentialConstructibleTypes.Where(ci => ci.Constructible()).Select(ci => ci.Type).ToList();

            static TypeConstructionCache()
            {
                var type = typeof(TType);
                k_PotentialConstructibleTypes.Add(new ConstructInfo(type, CanBeConstructed, Construct));
                if (type.IsValueType)
                {
                    HasParameterLessConstructor = true;
                }
                else
                {
                    HasParameterLessConstructor = null != type.GetConstructor(Array.Empty<Type>());
#if UNITY_EDITOR
                    foreach (var typeInfo in UnityEditor.TypeCache.GetTypesDerivedFrom<TType>())
                    {
                        var other = typeof(TypeConstructionCache<>).MakeGenericType(typeInfo);
                        System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(other.TypeHandle);

                        var constructibleMethod = other.GetMethods(BindingFlags.Public | BindingFlags.Static)
                            .First(m => m.Name == nameof(CanBeConstructed)
                                        && !m.IsGenericMethod
                                        && m.GetParameters().Length == 0);

                        var constructibleDelegate = (Func<bool>)Delegate.CreateDelegate(typeof(Func<bool>), constructibleMethod);

                        var constructMethod = other.GetMethods(BindingFlags.Public | BindingFlags.Static)
                            .First(m => m.Name == nameof(Construct)
                                        && !m.IsGenericMethod
                                        && m.GetParameters().Length == 0);

                        var constructMethodMonoSafe = typeof(TypeConstructionCache<TType>).GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
                            .First(m => m.Name == nameof(GetDelegateMonoSafe))
                            .MakeGenericMethod(typeInfo);
                        var constructDelegate = (Func<TType>)constructMethodMonoSafe.Invoke(null, new object[] { constructMethod });
                        k_PotentialConstructibleTypes.Add(new ConstructInfo(typeInfo, constructibleDelegate, constructDelegate));
                    }
#endif
                }
            }

            private static Func<TType> GetDelegateMonoSafe<T>(MethodInfo constructMethod)
                where T : TType
            {
                var del = (Func<T>)Delegate.CreateDelegate(typeof(Func<T>), constructMethod);
                return () => del();
            }

            public static TType Construct()
            {
                if (null != ExplicitConstruction)
                {
                    return ExplicitConstruction();
                }

                if (typeof(UnityEngine.ScriptableObject).IsAssignableFrom(typeof(TType)))
                {
                    return (TType)(object)UnityEngine.ScriptableObject.CreateInstance(typeof(TType));
                }

                if (HasParameterLessConstructor)
                {
                    return Activator.CreateInstance<TType>();
                }

                throw new InvalidOperationException($"Type `{typeof(TType).Name}` could not be constructed. A parameter-less constructor or an explicit construction method is required.");
            }

            public static bool TryConstruct(out TType value)
            {
                if (null != ExplicitConstruction)
                {
                    try
                    {
                        value = ExplicitConstruction();
                    }
                    catch
                    {
                        value = default;
                        return false;
                    }
                    return true;
                }

                if (typeof(UnityEngine.ScriptableObject).IsAssignableFrom(typeof(TType)))
                {
                    try
                    {
                        value = (TType)(object)UnityEngine.ScriptableObject.CreateInstance(typeof(TType));
                    }
                    catch
                    {
                        value = default;
                        return false;
                    }
                }

                if (HasParameterLessConstructor)
                {
                    try
                    {
                        value = Activator.CreateInstance<TType>();
                    }
                    catch
                    {
                        value = default;
                        return false;
                    }
                    return true;
                }

                value = default;
                return false;
            }

            public static TType Construct(Type type)
            {
                var info = k_PotentialConstructibleTypes.FirstOrDefault(ci => ci.Type == type);
                if (null == info || !info.Constructible())
                {
                    throw new ArgumentException($"Type `{type.Name}` could not be constructed and converted to type `{typeof(TType).Name}`. A parameter-less constructor or an explicit construction method is required.");
                }
                return info.Construct();
            }

            public static bool TryConstruct(Type type, out TType value)
            {
                try
                {
                    var info = k_PotentialConstructibleTypes.FirstOrDefault(ci => ci.Type == type);
                    if (null == info || !info.Constructible())
                    {
                        value = default;
                        return false;
                    }
                    value = info.Construct();
                }
                catch
                {
                    value = default;
                    return false;
                }

                return true;
            }
        }
    }
}
